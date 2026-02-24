// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Game.Modes.Objects.Drawables;
using osu.Game.Modes.Osu.Objects.Drawables;
using System;
using System.Collections.Generic;

namespace osu.Game.Modes.Osu
{
    class OsuScoreProcessor : ScoreProcessor, IHasDisplayJudgements
    {
        public OsuScoreProcessor(int hitObjectCount)
            : base(hitObjectCount)
        {
            Health.Value = 1;
        }

        public List<DisplayJudgement> DisplayJudgements { get; } = new List<DisplayJudgement>();
        protected override void UpdateCalculations(JudgementInfo judgement)
        {
            if (judgement != null)
            {
                switch (judgement.Result)
                {
                    case HitResult.Hit:
                        Combo.Value++;
                        Health.Value += 0.1f;
                        break;
                    case HitResult.Miss:
                        Combo.Value = 0;
                        Health.Value -= 0.2f;
                        break;
                }
            }

            int score = 0;
            int maxScore = 0;

            foreach (OsuJudgementInfo j in Judgements)
            {
                DisplayScore ds;

                double hitOffset = Math.Abs(judgement.TimeOffset);
                if (j.Score == OsuScoreResult.Hit300) ds = DisplayScore.Hit300;
                else if (j.Score == OsuScoreResult.Hit100) ds = DisplayScore.Hit100;
                else if (j.Score == OsuScoreResult.Hit50) ds = DisplayScore.Hit50;
                else ds = DisplayScore.Miss;

                DisplayJudgements.Add(new DisplayJudgement
                {
                    Score = ds,
                    ComboAtHit = judgement.ComboAtHit,
                    TimeOffset = judgement.TimeOffset
                });

                switch (j.Score)
                {
                    case OsuScoreResult.Miss:
                        maxScore += 300;
                        break;
                    case OsuScoreResult.Hit50:
                        score += 50;
                        maxScore += 300;
                        break;
                    case OsuScoreResult.Hit100:
                        score += 100;
                        maxScore += 300;
                        break;
                    case OsuScoreResult.Hit300:
                        score += 300;
                        maxScore += 300;
                        break;
                }
            }

            TotalScore.Value = score;
            Accuracy.Value = (double)score / maxScore;
        }
    }
}
