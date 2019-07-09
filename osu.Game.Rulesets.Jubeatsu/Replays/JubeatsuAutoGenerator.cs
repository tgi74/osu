// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Replays;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Replays;
using osuTK;

namespace osu.Game.Rulesets.Jubeatsu.Replays
{
    public class JubeatsuAutoGenerator
    {
        private readonly IBeatmap beatmap;
        private readonly Replay replay = new Replay();

        public JubeatsuAutoGenerator(IBeatmap beatmap)
        {
            this.beatmap = beatmap;
        }

        public Replay Generate()
        {
            foreach (var hit in beatmap.HitObjects)
                if (hit is IHasPosition position)
                {
                    addFrameToReplay(new JubeatsuReplayFrame(hit.StartTime - 0.1, position.Position * 1024 + new Vector2(128)));
                    addFrameToReplay(new JubeatsuReplayFrame(hit.StartTime, position.Position * 1024 + new Vector2(128), JubeatsuAction.Hit));
                    addFrameToReplay(new JubeatsuReplayFrame(hit.StartTime + 0.1, position.Position * 1024 + new Vector2(128)));
                }

            return replay;
        }

        private class ReplayFrameComparer : IComparer<ReplayFrame>
        {
            public int Compare(ReplayFrame f1, ReplayFrame f2)
            {
                if (f1 == null) throw new ArgumentNullException(nameof(f1));
                if (f2 == null) throw new ArgumentNullException(nameof(f2));

                return f1.Time.CompareTo(f2.Time);
            }
        }

        private static readonly IComparer<ReplayFrame> replay_frame_comparer = new ReplayFrameComparer();

        private int findInsertionIndex(ReplayFrame frame)
        {
            int index = replay.Frames.BinarySearch(frame, replay_frame_comparer);

            if (index < 0)
            {
                index = ~index;
            }
            else
            {
                // Go to the first index which is actually bigger
                while (index < replay.Frames.Count && frame.Time == replay.Frames[index].Time)
                {
                    ++index;
                }
            }

            return index;
        }

        private void addFrameToReplay(ReplayFrame frame) => replay.Frames.Insert(findInsertionIndex(frame), frame);
    }
}
