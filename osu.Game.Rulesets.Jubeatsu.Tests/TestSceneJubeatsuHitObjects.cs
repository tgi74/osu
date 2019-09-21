// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Jubeatsu.Objects;
using osu.Game.Rulesets.Jubeatsu.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Jubeatsu.Tests
{
    public class TestSceneJubeatsuHitObjects : OsuGridTestScene
    {
        public override IReadOnlyList<Type> RequiredTypes => new[]
        {
            typeof(DrawableJubeatsuHitObject),
            typeof(DrawableJubeatsuBox),
        };

        public TestSceneJubeatsuHitObjects()
            : base(4, 4)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            AddStep("Add a box and trigger a Ok", addResultOk);
            AddStep("Add a box and trigger a miss", addResultMiss);
            AddStep("Add a box with no judgement trigger", addResultNone);
        }

        private void addResultNone() => addHit();

        private void addResultOk()
        {
            TestJubeatsuBox hit = addHit();
            Scheduler.AddDelayed(hit.TriggerJudgement, hit.HitObject.StartTime - Time.Current);
        }

        private void addResultMiss()
        {
            TestJubeatsuBox hit = addHit();
            Scheduler.AddDelayed(hit.TriggerJudgement, hit.HitObject.StartTime - (hit.HitObject.HitWindows?.WindowFor(HitResult.Miss) - 10 ?? 0) - Time.Current);
        }

        private int additionOffset;

        private TestJubeatsuBox addHit()
        {
            JubeatsuHitObject hit = new JubeatsuHitObject
            {
                StartTime = Time.Current + 1000
            };
            TestJubeatsuBox d;
            Cell(additionOffset++ % (Rows * Cols)).Child = d = new TestJubeatsuBox(hit, false);
            hit.ApplyDefaults(new ControlPointInfo(), new BeatmapDifficulty());
            return d;
        }
    }

    public class TestJubeatsuBox : DrawableJubeatsuBox
    {
        private readonly bool auto;

        public TestJubeatsuBox(JubeatsuHitObject hit, bool auto = true)
            : base(hit)
        {
            this.auto = auto;
        }

        public void TriggerJudgement() => UpdateResult(true);

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (auto && !userTriggered && timeOffset > 0)
            {
                // force success
                ApplyResult(r => r.Type = HitResult.Great);
            }
            else
                base.CheckForResult(userTriggered, timeOffset);
        }
    }
}
