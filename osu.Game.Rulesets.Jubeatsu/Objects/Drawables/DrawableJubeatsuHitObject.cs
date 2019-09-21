// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK;

namespace osu.Game.Rulesets.Jubeatsu.Objects.Drawables
{
    public abstract class DrawableJubeatsuHitObject : DrawableHitObject<JubeatsuHitObject>
    {
        protected sealed override double InitialLifetimeOffset => HitObject.TimePreempt;
        protected readonly AnimationContainer ContentContainer;

        public float ContentSize
        {
            get => ContentContainer.Size.X;
            set => ContentContainer.Size = new Vector2(value);
        }

        protected DrawableJubeatsuHitObject(JubeatsuHitObject hit)
            : base(hit)
        {
            AddInternal(ContentContainer = new AnimationContainer
            {
                Size = new Vector2(1),
                RelativeSizeAxes = Axes.Both,
                RelativePositionAxes = Axes.Both,
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre
            });
            Alpha = 0;

            Position = hit.Position;
            FillMode = FillMode.Fit;

            RelativePositionAxes = Axes.Both;
            RelativeSizeAxes = Axes.Both;
        }

        protected override void UpdateStateTransforms(ArmedState state)
        {
            base.UpdateStateTransforms(state);

            switch (state)
            {
                case ArmedState.Idle:
                    // Manually set to reduce the number of future alive objects to a bare minimum.
                    LifetimeStart = HitObject.StartTime - HitObject.TimePreempt;
                    break;
            }
        }

        protected class AnimationContainer : CompositeDrawable
        {
            public new void AddInternal(Drawable d) => base.AddInternal(d);

            public AnimationContainer()
            {
                Masking = true;
            }
        }
    }
}
