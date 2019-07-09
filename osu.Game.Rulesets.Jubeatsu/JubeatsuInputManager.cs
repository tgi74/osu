// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.ComponentModel;
using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Jubeatsu
{
    public class JubeatsuInputManager : RulesetInputManager<JubeatsuAction>
    {
        public JubeatsuInputManager(RulesetInfo ruleset)
            : base(ruleset, 0, SimultaneousBindingMode.Unique)
        {
        }
    }

    public enum JubeatsuAction
    {
        [Description("Hit")]
        Hit
    }
}
