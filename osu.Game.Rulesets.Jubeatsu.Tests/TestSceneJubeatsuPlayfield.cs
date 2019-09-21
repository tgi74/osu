// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Jubeatsu.Objects;
using osu.Game.Rulesets.Jubeatsu.Objects.Drawables;
using osu.Game.Rulesets.Jubeatsu.UI;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Tests.Visual;
using osuTK;

namespace osu.Game.Rulesets.Jubeatsu.Tests
{
    public class TestSceneJubeatsuPlayfield : OsuTestScene
    {
        public override IReadOnlyList<Type> RequiredTypes => new[]
        {
            typeof(JubeatsuDrawableGrid),
            typeof(DrawableJubeatsuHitObject),
            typeof(DrawableJubeatsuBox),
        };

        private JubeatsuDrawableRuleset drawableRuleset;
        private readonly Random random = new Random(74000);

        [BackgroundDependencyLoader]
        private void load()
        {
            var controlPointInfo = new ControlPointInfo();
            controlPointInfo.TimingPoints.Add(new TimingControlPoint());

            WorkingBeatmap beatmap = CreateWorkingBeatmap(new Beatmap
            {
                HitObjects = new List<HitObject> { new JubeatsuHitObject() },
                BeatmapInfo = new BeatmapInfo
                {
                    BaseDifficulty = new BeatmapDifficulty(),
                    Metadata = new BeatmapMetadata
                    {
                        Artist = @"Unknown",
                        Title = @"Sample Beatmap",
                        AuthorString = @"peppy",
                    },
                    Ruleset = new JubeatsuRuleset().RulesetInfo
                },
                ControlPointInfo = controlPointInfo
            });

            Add(new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Children = new[] { drawableRuleset = new JubeatsuDrawableRuleset(new JubeatsuRuleset(), beatmap, Array.Empty<Mod>()) }
            });

            AddStep("Add Object", addObject);
            AddStep("Add Multiple Objects", addMultipleObjects);
        }

        private void addObject()
        {
            addObject(random.Next(4) / 4f, random.Next(4) / 4f);
        }

        private void addObject(float x, float y, float whenInTheFuture = 1000)
        {
            JubeatsuHitObject h = new JubeatsuHitObject
            {
                StartTime = drawableRuleset.Playfield.Time.Current + whenInTheFuture,
                Position = new Vector2(x, y)
            };

            h.ApplyDefaults(new ControlPointInfo(), new BeatmapDifficulty());

            drawableRuleset.Playfield.Add(new TestJubeatsuBox(h));
        }

        private void addMultipleObjects()
        {
            int when = 1000;

            for (int y = 0; y < 4; y++)
            for (int x = 0; x < 4; x++)
            {
                addObject(x / 4f, y / 4f, when += 200);
            }
        }
    }
}
