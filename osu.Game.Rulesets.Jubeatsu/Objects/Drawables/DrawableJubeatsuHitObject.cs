// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osuTK;

namespace osu.Game.Rulesets.Jubeatsu.Objects.Drawables
{
    public abstract class DrawableJubeatsuHitObject : DrawableHitObject<JubeatsuHitObject>
    {
        public override double LifetimeStart => HitObject.StartTime - HitObject.TimePreempt;
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

            RelativePositionAxes = Axes.Both;
            RelativeSizeAxes = Axes.Both;
        }

        protected sealed override void UpdateState(ArmedState state)
        {
            double transformTime = HitObject.StartTime - HitObject.TimePreempt;

            base.ApplyTransformsAt(transformTime, true);
            base.ClearTransformsAfter(transformTime, true);

            using (BeginAbsoluteSequence(transformTime, true))
            {
                UpdatePreemptState();

                var judgementOffset = Math.Min(HitObject.HitWindows.HalfWindowFor(HitResult.Miss), Result?.TimeOffset ?? 0);

                using (BeginDelayedSequence(HitObject.TimePreempt + judgementOffset, true))
                    UpdateCurrentState(state);
            }
        }

        protected virtual void UpdatePreemptState() => this.FadeIn(HitObject.TimeFadeIn);

        protected virtual void UpdateCurrentState(ArmedState state)
        {
        }

        public override void ClearTransformsAfter(double time, bool propagateChildren = false, string targetMember = null)
        {
        }

        public override void ApplyTransformsAt(double time, bool propagateChildren = false)
        {
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
