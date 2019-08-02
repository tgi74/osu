// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Jubeatsu.Beatmaps;
using osu.Game.Rulesets.Jubeatsu.Mods;
using osu.Game.Rulesets.Jubeatsu.Replays;
using osu.Game.Rulesets.Jubeatsu.UI;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Replays.Types;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Jubeatsu
{
    public class JubeatsuRuleset : Ruleset
    {
        public override string ShortName => "jubeatsu";
        public override string Description => "Jubeatsu";

        public override Drawable CreateIcon() => new SpriteIcon { Icon = FontAwesome.Solid.ShareSquare };

        public JubeatsuRuleset(RulesetInfo info = null)
            : base(info)
        {
        }

        public override IEnumerable<Mod> GetModsFor(ModType type)
        {
            switch (type)
            {
                case ModType.Automation:
                    return new Mod[]
                    {
                        new JubeatsuModAutoplay(),
                    };
            }

            return new Mod[] { };
        }

        public override DrawableRuleset CreateDrawableRulesetWith(WorkingBeatmap beatmap, IReadOnlyList<Mod> mods) => new JubeatsuDrawableRuleset(this, beatmap, mods);

        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap) => new JubeatsuBeatmapConverter(beatmap);

        public override IBeatmapProcessor CreateBeatmapProcessor(IBeatmap beatmap) => new JubeatsuBeatmapProcessor(beatmap);

        public override DifficultyCalculator CreateDifficultyCalculator(WorkingBeatmap beatmap) => null;

        public override IConvertibleReplayFrame CreateConvertibleReplayFrame() => new JubeatsuReplayFrame();

        public override IEnumerable<KeyBinding> GetDefaultKeyBindings(int variant = 0) => new[]
        {
            new KeyBinding(InputKey.Z, JubeatsuAction.Hit),
            new KeyBinding(InputKey.X, JubeatsuAction.Hit),
            new KeyBinding(InputKey.MouseLeft, JubeatsuAction.Hit),
            new KeyBinding(InputKey.MouseRight, JubeatsuAction.Hit),
        };
    }
}
