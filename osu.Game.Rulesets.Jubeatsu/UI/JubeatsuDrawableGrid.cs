// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Jubeatsu.UI
{
    public class JubeatsuDrawableGrid : CompositeDrawable
    {
        public int GridHeight;
        public int GridWidth;

        public float ContentSize
        {
            get => InternalChildren[0].Size.X;
            set
            {
                foreach (var box in InternalChildren)
                    ((Container)box).Child.Size = new Vector2(value); //TODO:
            }
        }

        public JubeatsuDrawableGrid(int width = 4, int height = 4)
        {
            GridHeight = height;
            GridWidth = width;

            FillMode = FillMode.Fit;
            var size = new Vector2((float)1 / width, (float)1 / height);

            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                AddInternal(new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    RelativePositionAxes = Axes.Both,
                    Size = size,
                    Position = new Vector2((float)x / width, (float)y / height),
                    Child = new Box
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Colour = Color4.DarkBlue,
                        RelativeSizeAxes = Axes.Both
                    }
                });
            }
        }
    }
}
