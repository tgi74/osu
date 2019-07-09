// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Jubeatsu.Objects;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osuTK;

namespace osu.Game.Rulesets.Jubeatsu.Beatmaps
{
    public class JubeatsuBeatmapConverter : BeatmapConverter<JubeatsuHitObject>
    {
        public JubeatsuBeatmapConverter(IBeatmap beatmap)
            : base(beatmap)
        {
        }

        protected override IEnumerable<Type> ValidConversionTypes => new[] { typeof(IHasPosition), typeof(IHasEndTime) };

        protected override IEnumerable<JubeatsuHitObject> ConvertHitObject(HitObject original, IBeatmap beatmap)
        {
            if (!(original is IHasPosition position))
                yield break;

            yield return new JubeatsuHitObject
            {
                StartTime = original.StartTime,
                Samples = original.Samples,
                Position = new Vector2((int)(position.X / 512f * 4) / 4f, (int)(position.Y / 384f * 4) / 4f)
            };

            if (!(original is IHasCurve curved))
                yield break;

            var endPosition = curved.CurvePositionAt(1);
            yield return new JubeatsuHitObject
            {
                StartTime = curved.EndTime,
                Samples = original.Samples,
                Position = new Vector2((int)((position.X + endPosition.X) / 512f * 4) / 4f, (int)((position.Y + endPosition.Y) / 384f * 4) / 4f)
            };
        }
    }
}
