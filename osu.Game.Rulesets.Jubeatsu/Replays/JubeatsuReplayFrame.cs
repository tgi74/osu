// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Replays.Legacy;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.Replays.Types;
using osuTK;

namespace osu.Game.Rulesets.Jubeatsu.Replays
{
    public class JubeatsuReplayFrame : ReplayFrame, IConvertibleReplayFrame
    {
        public Vector2 Position;
        public List<JubeatsuAction> Actions = new List<JubeatsuAction>();

        public JubeatsuReplayFrame()
        {
        }

        public JubeatsuReplayFrame(double time, Vector2 position, params JubeatsuAction[] actions)
            : base(time)
        {
            Position = position;
            Actions.AddRange(actions);
        }

        public void ConvertFrom(LegacyReplayFrame legacyFrame, IBeatmap beatmap, ReplayFrame lastFrame = null)
        {
            Position = legacyFrame.Position;
            if (legacyFrame.MouseLeft || legacyFrame.MouseRight) Actions.Add(JubeatsuAction.Hit);
        }
    }
}
