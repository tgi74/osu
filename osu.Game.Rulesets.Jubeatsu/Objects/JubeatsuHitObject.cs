// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Jubeatsu.Scoring;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Scoring;
using osuTK;

namespace osu.Game.Rulesets.Jubeatsu.Objects
{
    public class JubeatsuHitObject : HitObject, IHasPosition
    {
        public float X => Position.X;
        public float Y => Position.Y;
        public Vector2 Position { get; set; }

        public double TimePreempt = 500;

        public double TimeFadeIn = 100;

        public override Judgement CreateJudgement() => new Judgement();

        protected override HitWindows CreateHitWindows() => new JubeatsuHitWindows();
    }
}
