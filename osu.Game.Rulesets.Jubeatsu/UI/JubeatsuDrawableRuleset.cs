// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Input;
using osu.Game.Beatmaps;
using osu.Game.Input.Handlers;
using osu.Game.Replays;
using osu.Game.Rulesets.Jubeatsu.Objects;
using osu.Game.Rulesets.Jubeatsu.Objects.Drawables;
using osu.Game.Rulesets.Jubeatsu.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Jubeatsu.UI
{
    public class JubeatsuDrawableRuleset : DrawableRuleset<JubeatsuHitObject>
    {
        public JubeatsuDrawableRuleset(Ruleset ruleset, IWorkingBeatmap workingBeatmap, IReadOnlyList<Mod> mods)
            : base(ruleset, workingBeatmap, mods)
        {
        }

        public override DrawableHitObject<JubeatsuHitObject> CreateDrawableRepresentation(JubeatsuHitObject h)
        {
            switch (h)
            {
                case JubeatsuHitObject hit:
                    return new DrawableJubeatsuBox(hit);
            }

            return null;
        }

        protected override PassThroughInputManager CreateInputManager() => new JubeatsuInputManager(Ruleset.RulesetInfo);

        public override PlayfieldAdjustmentContainer CreatePlayfieldAdjustmentContainer() => new JubeatsuPlayfieldAdjustmentContainer();

        protected override Playfield CreatePlayfield() => new JubeatsuPlayfield();

        protected override ReplayInputHandler CreateReplayInputHandler(Replay replay) => new JubeatsuFramedReplayInputHandler(replay);
    }
}
