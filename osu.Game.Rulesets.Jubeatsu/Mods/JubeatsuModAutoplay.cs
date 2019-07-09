// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Beatmaps;
using osu.Game.Rulesets.Jubeatsu.Objects;
using osu.Game.Rulesets.Jubeatsu.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Scoring;
using osu.Game.Users;

namespace osu.Game.Rulesets.Jubeatsu.Mods
{
    public class JubeatsuModAutoplay : ModAutoplay<JubeatsuHitObject>
    {
        public override Score CreateReplayScore(IBeatmap beatmap) => new Score
        {
            ScoreInfo = new ScoreInfo { User = new User { Username = "Autoplay" } },
            Replay = new JubeatsuAutoGenerator(beatmap).Generate()
        };
    }
}
