// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Localisation;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Jubeatsu.Objects.Drawables
{
    public class DrawableJubeatsuBox : DrawableJubeatsuHitObject, IKeyBindingHandler<JubeatsuAction>
    {
        public override bool HandlePositionalInput => true;

        private readonly Vector2[] positions =
        {
            new Vector2(-0.5f, 0.5f),
            new Vector2(-0.5f, -0.5f),
            new Vector2(0.5f, -0.5f),
            new Vector2(0.5f, 0.5f),
        };

        private readonly Anchor[] anchors =
        {
            Anchor.TopLeft,
            Anchor.TopRight,
            Anchor.BottomRight,
            Anchor.BottomLeft,
        };

        private readonly List<Box> boxes = new List<Box>();
        private readonly List<SpriteText> touchTexts = new List<SpriteText>();

        public DrawableJubeatsuBox(JubeatsuHitObject hit)
            : base(hit)
        {
            for (int i = 0; i < 4; i++)
            {
                var b = new Box
                {
                    Colour = ColourInfo.GradientVertical(Color4.Gray, Color4.DarkGray),
                    RelativeSizeAxes = Axes.Both,
                    RelativePositionAxes = Axes.Both,
                    Size = new Vector2(1),
                    Position = positions[i],
                    Origin = anchors[i],
                    Anchor = Anchor.Centre
                };
                boxes.Add(b);
                ContentContainer.AddInternal(b);
            }

            for (int i = 0; i < 2; i++)
            {
                var text = new SpriteText
                {
                    Text = new LocalisedString("TOUCH"),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Y = i == 1 ? -0.5f : 0.5f,
                    RelativePositionAxes = Axes.Both,
                    Font = FontUsage.Default.With(size: 48f),
                    Alpha = 0
                };
                touchTexts.Add(text);
                AddInternal(text);
            }
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (!userTriggered)
            {
                if (!HitObject.HitWindows.CanBeHit(timeOffset))
                    ApplyResult(r => r.Type = HitResult.Miss);

                return;
            }

            var result = HitObject.HitWindows.ResultFor(timeOffset);

            if (result == HitResult.None)
                return;

            ApplyResult(r => r.Type = result);
        }

        protected override void UpdatePreemptState()
        {
            base.UpdatePreemptState();

            foreach (var b in boxes)
            {
                b.FadeColour(ColourInfo.GradientVertical(Color4.Orange, Color4.OrangeRed), HitObject.TimePreempt);
                b.MoveTo(Vector2.Zero, HitObject.TimePreempt);
            }

            foreach (var b in touchTexts)
            {
                b.MoveTo(Vector2.Zero, HitObject.TimePreempt);
                b.FadeIn(HitObject.TimePreempt);
            }
        }

        protected override void UpdateCurrentState(ArmedState state)
        {
            switch (state)
            {
                case ArmedState.Idle:
                    this.FadeOut(200).Then().Expire();
                    break;

                case ArmedState.Hit:
                    this.FadeOut().Then().Expire();
                    break;

                case ArmedState.Miss:
                    this.FlashColour(Color4.Red, 200, Easing.InBounce);
                    this.FadeOut(200).Then().Expire();
                    break;
            }
        }

        public bool OnPressed(JubeatsuAction action)
        {
            switch (action)
            {
                case JubeatsuAction.Hit:
                    if (IsHovered)
                    {
                        if (AllJudged)
                            return false;

                        UpdateResult(true);
                        return true;
                    }

                    break;
            }

            return false;
        }

        public bool OnReleased(JubeatsuAction action) => false;
    }
}
