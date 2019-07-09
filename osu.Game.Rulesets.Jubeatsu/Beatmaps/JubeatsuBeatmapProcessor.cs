// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Jubeatsu.Objects;
using osuTK;

namespace osu.Game.Rulesets.Jubeatsu.Beatmaps
{
    public class JubeatsuBeatmapProcessor : BeatmapProcessor
    {
        public JubeatsuBeatmapProcessor(IBeatmap beatmap)
            : base(beatmap)
        {
        }

        public override void PostProcess()
        {
            var jubeatsuBeatmap = (Beatmap<JubeatsuHitObject>)Beatmap;

            if (jubeatsuBeatmap.BeatmapInfo.Ruleset == new JubeatsuRuleset().RulesetInfo)
                return;

            foreach (var hit in jubeatsuBeatmap.HitObjects)
                processConvertedHit(jubeatsuBeatmap, hit);
        }

        private void processConvertedHit(Beatmap<JubeatsuHitObject> jubeatsuBeatmap, JubeatsuHitObject hit)
        {
            if (isPositionCorrect(jubeatsuBeatmap, hit))
                return;

            var originalPos = hit.Position;

            for (float y = 0; y < 1; y += 0.25f)
            for (float x = 0; x < 1; x += 0.25f)
            {
                hit.Position = new Vector2((originalPos.X + x) % 1, (originalPos.Y + y) % 1);

                if (isPositionCorrect(jubeatsuBeatmap, hit))
                    return;
            }
        }

        private bool isPositionCorrect(Beatmap<JubeatsuHitObject> jubeatsuBeatmap, JubeatsuHitObject hit)
        {
            if (hit.X < 0 || hit.Y < 0 || hit.X >= 1 || hit.Y >= 1)
                return false;

            foreach (var other in jubeatsuBeatmap.HitObjects)
                if (hit != other && other.Position == hit.Position && Math.Abs(other.StartTime - hit.StartTime) < hit.TimePreempt)
                    return false;

            return true;
        }
    }
}
