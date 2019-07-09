// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Game.Rulesets.Jubeatsu.Objects.Drawables;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Jubeatsu.UI
{
    public class JubeatsuPlayfield : Playfield
    {
        private readonly JubeatsuDrawableGrid grid;

        public JubeatsuPlayfield()
        {
            InternalChildren = new Drawable[]
            {
                grid = new JubeatsuDrawableGrid
                {
                    RelativeSizeAxes = Axes.Both,
                    ContentSize = 0.95f
                },
                HitObjectContainer
            };
        }

        public override void Add(DrawableHitObject h)
        {
            if (!(h is DrawableJubeatsuHitObject jubeatsuHit))
                return;

            jubeatsuHit.Size = new Vector2(1f / grid.GridWidth, 1f / grid.GridHeight);
            jubeatsuHit.ContentSize = 0.95f;

            base.Add(jubeatsuHit);
        }

        protected override GameplayCursorContainer CreateCursor() => new GameplayCursorContainer();

        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => HitObjectContainer.ReceivePositionalInputAt(screenSpacePos);
    }
}
