// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Primitives;
using osu.Game.Rulesets.Jubeatsu.Objects;
using osu.Game.Rulesets.Jubeatsu.Objects.Drawables;
using osu.Game.Rulesets.Jubeatsu.Scoring;
using osu.Game.Rulesets.Jubeatsu.UI;
using osu.Game.Tests.Visual;
using osuTK;

namespace osu.Game.Rulesets.Jubeatsu.Tests
{
    [TestFixture]
    public class TestPlayField : OsuTestScene
    {
        public override IReadOnlyList<Type> RequiredTypes => new[]
        {
            typeof(JubeatsuDrawableGrid),
            typeof(DrawableJubeatsuBox),
        };

        [BackgroundDependencyLoader]
        private void load()
        {
            //Child = new JubeatsuDrawableGrid();
            Child = new DrawableJubeatsuBox(new JubeatsuHitObject
            {
                Position = new Vector2I(1, 1),
                HitWindows = new JubeatsuHitWindows()
            })
            {
                Size = new Vector2(200)
            };
        }
    }
}
