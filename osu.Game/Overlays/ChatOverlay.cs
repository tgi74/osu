﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input;
using osu.Framework.Threading;
using osu.Game.Configuration;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.API;
using osu.Game.Online.API.Requests;
using osu.Game.Online.Chat;
using osu.Game.Overlays.Chat;

namespace osu.Game.Overlays
{
    public class ChatOverlay : OsuFocusedOverlayContainer, IOnlineComponent
    {
        private const float textbox_height = 60;
        private const float channel_selection_min_height = 0.3f;

        private ScheduledDelegate messageRequest;

        private readonly Container<DrawableChannel> currentChannelContainer;

        private readonly LoadingAnimation loading;

        private readonly FocusedTextBox textbox;

        private APIAccess api;

        private const int transition_length = 500;

        public const float DEFAULT_HEIGHT = 0.4f;

        public const float TAB_AREA_HEIGHT = 50;

        private GetMessagesRequest fetchReq;

        private readonly ChatTabControl channelTabs;

        private readonly Container chatContainer;
        private readonly Container tabsArea;
        private readonly Box chatBackground;
        private readonly Box tabBackground;

        private Bindable<double> chatHeight;

        private readonly Container channelSelectionContainer;
        private readonly ChannelSelectionOverlay channelSelection;

        public override bool Contains(Vector2 screenSpacePos) => chatContainer.ReceiveMouseInputAt(screenSpacePos) || channelSelection.State == Visibility.Visible && channelSelection.ReceiveMouseInputAt(screenSpacePos);

        public ChatOverlay()
        {
            RelativeSizeAxes = Axes.Both;
            RelativePositionAxes = Axes.Both;
            Anchor = Anchor.BottomLeft;
            Origin = Anchor.BottomLeft;

            const float padding = 5;

            Children = new Drawable[]
            {
                channelSelectionContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Height = 1f - DEFAULT_HEIGHT,
                    Masking = true,
                    Children = new[]
                    {
                        channelSelection = new ChannelSelectionOverlay
                        {
                            RelativeSizeAxes = Axes.Both,
                        },
                    },
                },
                chatContainer = new Container
                {
                    Name = @"chat container",
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    RelativeSizeAxes = Axes.Both,
                    Height = DEFAULT_HEIGHT,
                    Children = new[]
                    {
                        new Container
                        {
                            Name = @"chat area",
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Top = TAB_AREA_HEIGHT },
                            Children = new Drawable[]
                            {
                                chatBackground = new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                },
                                currentChannelContainer = new Container<DrawableChannel>
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Padding = new MarginPadding
                                    {
                                        Bottom = textbox_height
                                    },
                                },
                                new Container
                                {
                                    Anchor = Anchor.BottomLeft,
                                    Origin = Anchor.BottomLeft,
                                    RelativeSizeAxes = Axes.X,
                                    Height = textbox_height,
                                    Padding = new MarginPadding
                                    {
                                        Top = padding * 2,
                                        Bottom = padding * 2,
                                        Left = ChatLine.LEFT_PADDING + padding * 2,
                                        Right = padding * 2,
                                    },
                                    Children = new Drawable[]
                                    {
                                        textbox = new FocusedTextBox
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Height = 1,
                                            PlaceholderText = "type your message",
                                            Exit = () => State = Visibility.Hidden,
                                            OnCommit = postMessage,
                                            ReleaseFocusOnCommit = false,
                                            HoldFocus = true,
                                        }
                                    }
                                },
                                loading = new LoadingAnimation(),
                            }
                        },
                        tabsArea = new Container
                        {
                            Name = @"tabs area",
                            RelativeSizeAxes = Axes.X,
                            Height = TAB_AREA_HEIGHT,
                            Children = new Drawable[]
                            {
                                tabBackground = new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = Color4.Black,
                                },
                                channelTabs = new ChatTabControl
                                {
                                    RelativeSizeAxes = Axes.Both,
                                },
                            }
                        },
                    },
                },
            };

            channelTabs.Current.ValueChanged += newChannel => CurrentChannel = newChannel;
            channelTabs.ChannelSelectorActive.ValueChanged += value => channelSelection.State = value ? Visibility.Visible : Visibility.Hidden;
            channelSelection.StateChanged += (overlay, state) =>
            {
                channelTabs.ChannelSelectorActive.Value = state == Visibility.Visible;

                if (state == Visibility.Visible)
                {
                    textbox.HoldFocus = false;
                    if (1f - chatHeight.Value < channel_selection_min_height)
                    {
                        chatContainer.ResizeHeightTo(1f - channel_selection_min_height, 800, Easing.OutQuint);
                        channelSelectionContainer.ResizeHeightTo(channel_selection_min_height, 800, Easing.OutQuint);
                        channelSelection.Show();
                        chatHeight.Value = 1f - channel_selection_min_height;
                    }
                }
                else
                {
                    textbox.HoldFocus = true;
                }
            };
        }

        private double startDragChatHeight;
        private bool isDragging;

        protected override bool OnDragStart(InputState state)
        {
            isDragging = tabsArea.IsHovered;

            if (!isDragging)
                return base.OnDragStart(state);

            startDragChatHeight = chatHeight.Value;
            return true;
        }

        protected override bool OnDrag(InputState state)
        {
            if (isDragging)
            {
                Trace.Assert(state.Mouse.PositionMouseDown != null);

                chatHeight.Value = startDragChatHeight - (state.Mouse.Position.Y - state.Mouse.PositionMouseDown.Value.Y) / Parent.DrawSize.Y;
            }

            return true;
        }

        protected override bool OnDragEnd(InputState state)
        {
            isDragging = false;
            return base.OnDragEnd(state);
        }

        public void APIStateChanged(APIAccess api, APIState state)
        {
            switch (state)
            {
                case APIState.Online:
                    initializeChannels();
                    break;
                default:
                    messageRequest?.Cancel();
                    break;
            }
        }

        public override bool AcceptsFocus => true;

        protected override bool OnClick(InputState state) => true;

        protected override void OnFocus(InputState state)
        {
            //this is necessary as textbox is masked away and therefore can't get focus :(
            GetContainingInputManager().ChangeFocus(textbox);
            base.OnFocus(state);
        }

        protected override void PopIn()
        {
            this.MoveToY(0, transition_length, Easing.OutQuint);
            this.FadeIn(transition_length, Easing.OutQuint);

            textbox.HoldFocus = true;
            base.PopIn();
        }

        protected override void PopOut()
        {
            this.MoveToY(Height, transition_length, Easing.InSine);
            this.FadeOut(transition_length, Easing.InSine);

            textbox.HoldFocus = false;
            base.PopOut();
        }

        [BackgroundDependencyLoader]
        private void load(APIAccess api, OsuConfigManager config, OsuColour colours)
        {
            this.api = api;
            api.Register(this);

            chatHeight = config.GetBindable<double>(OsuSetting.ChatDisplayHeight);
            chatHeight.ValueChanged += h =>
            {
                chatContainer.Height = (float)h;
                channelSelectionContainer.Height = 1f - (float)h;
                tabBackground.FadeTo(h == 1 ? 1 : 0.8f, 200);
            };
            chatHeight.TriggerChange();

            chatBackground.Colour = colours.ChatBlue;
        }

        private long? lastMessageId;

        private readonly List<Channel> careChannels = new List<Channel>();

        private readonly List<DrawableChannel> loadedChannels = new List<DrawableChannel>();

        private void initializeChannels()
        {
            loading.Show();

            messageRequest?.Cancel();

            ListChannelsRequest req = new ListChannelsRequest();
            req.Success += delegate (List<Channel> channels)
            {
                Scheduler.Add(delegate
                {
                    addChannel(channels.Find(c => c.Name == @"#lazer"));
                    addChannel(channels.Find(c => c.Name == @"#osu"));
                    addChannel(channels.Find(c => c.Name == @"#lobby"));

                    channelSelection.OnRequestJoin = addChannel;
                    channelSelection.Sections = new[]
                    {
                        new ChannelSection
                        {
                            Header = "All Channels",
                            Channels = channels,
                        },
                    };
                });

                messageRequest = Scheduler.AddDelayed(fetchNewMessages, 1000, true);
            };

            api.Queue(req);
        }

        private Channel currentChannel;

        protected Channel CurrentChannel
        {
            get
            {
                return currentChannel;
            }

            set
            {
                if (currentChannel == value || value == null) return;

                currentChannel = value;

                textbox.Current.Disabled = currentChannel.ReadOnly;
                channelTabs.Current.Value = value;

                var loaded = loadedChannels.Find(d => d.Channel == value);
                if (loaded == null)
                {
                    currentChannelContainer.FadeOut(500, Easing.OutQuint);
                    loading.Show();

                    loaded = new DrawableChannel(currentChannel);
                    loadedChannels.Add(loaded);
                    LoadComponentAsync(loaded, l =>
                    {
                        if (currentChannel.Messages.Any())
                            loading.Hide();

                        currentChannelContainer.Clear(false);
                        currentChannelContainer.Add(loaded);
                        currentChannelContainer.FadeIn(500, Easing.OutQuint);
                    });
                }
                else
                {
                    currentChannelContainer.Clear(false);
                    currentChannelContainer.Add(loaded);
                }
            }
        }

        private void addChannel(Channel channel)
        {
            if (channel == null) return;

            var existing = careChannels.Find(c => c.Id == channel.Id);

            if (existing != null)
            {
                // if we already have this channel loaded, we don't want to make a second one.
                channel = existing;
            }
            else
            {
                careChannels.Add(channel);
                channelTabs.AddItem(channel);
            }

            // let's fetch a small number of messages to bring us up-to-date with the backlog.
            fetchInitialMessages(channel);

            if (CurrentChannel == null)
                CurrentChannel = channel;

            channel.Joined.Value = true;
        }

        private void fetchInitialMessages(Channel channel)
        {
            var req = new GetMessagesRequest(new List<Channel> { channel }, null);

            req.Success += delegate (List<Message> messages)
            {
                loading.Hide();
                channel.AddNewMessages(messages.ToArray());
                Debug.Write("success!");
            };
            req.Failure += delegate
            {
                Debug.Write("failure!");
            };

            api.Queue(req);
        }

        private void fetchNewMessages()
        {
            if (fetchReq != null) return;

            fetchReq = new GetMessagesRequest(careChannels, lastMessageId);

            fetchReq.Success += delegate (List<Message> messages)
            {
                foreach (var group in messages.Where(m => m.TargetType == TargetType.Channel).GroupBy(m => m.TargetId))
                    careChannels.Find(c => c.Id == group.Key)?.AddNewMessages(group.ToArray());

                lastMessageId = messages.LastOrDefault()?.Id ?? lastMessageId;

                Debug.Write("success!");
                fetchReq = null;
            };

            fetchReq.Failure += delegate
            {
                Debug.Write("failure!");
                fetchReq = null;
            };

            api.Queue(fetchReq);
        }

        private void postMessage(TextBox textbox, bool newText)
        {
            var postText = textbox.Text;

            textbox.Text = string.Empty;

            if (string.IsNullOrEmpty(postText))
                return;

            var target = currentChannel;

            if (target == null) return;

            if (!api.IsLoggedIn)
            {
                target.AddNewMessages(new ErrorMessage("Please login to participate in chat!"));
                return;
            }

            if (postText[0] == '/')
            {
                // TODO: handle commands
                target.AddNewMessages(new ErrorMessage("Chat commands are not supported yet!"));
                return;
            }

            var message = new LocalEchoMessage
            {
                Sender = api.LocalUser.Value,
                Timestamp = DateTimeOffset.Now,
                TargetType = TargetType.Channel, //TODO: read this from channel
                TargetId = target.Id,
                Content = postText
            };

            var req = new PostMessageRequest(message);

            target.AddLocalEcho(message);
            req.Failure += e => target.ReplaceMessage(message, null);
            req.Success += m => target.ReplaceMessage(message, m);

            api.Queue(req);
        }
    }
}
