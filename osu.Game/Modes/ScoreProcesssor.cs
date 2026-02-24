// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using osu.Framework.Configuration;
using osu.Game.Modes.Objects.Drawables;

public enum DisplayScore
{
    Miss,
    Hit50,
    Hit100,
    Hit300
}

public class DisplayJudgement
{
    public DisplayScore Score;
    public ulong? ComboAtHit;
    public double TimeOffset;
}

public interface IHasDisplayJudgements
{
    List<DisplayJudgement> DisplayJudgements { get; }
}

namespace osu.Game.Modes
{
    public abstract class ScoreProcessor
    {
        public virtual Score GetScore() => new Score()
        {
            TotalScore = TotalScore,
            Combo = Combo,
            MaxCombo = HighestCombo,
            Accuracy = Accuracy,
            Health = Health,
            Rank = CalculateRank(),
        };

        public readonly BindableDouble TotalScore = new BindableDouble { MinValue = 0 };

        public readonly BindableDouble Accuracy = new BindableDouble { MinValue = 0, MaxValue = 1 };

        public readonly BindableDouble Health = new BindableDouble { MinValue = 0, MaxValue = 1 };

        public readonly BindableInt Combo = new BindableInt();

        /// <summary>
        /// Are we allowed to fail?
        /// </summary>
        protected bool CanFail => true;

        protected bool HasFailed { get; private set; }

        /// <summary>
        /// Called when we reach a failing health of zero.
        /// </summary>
        public event Action Failed;

        /// <summary>
        /// Keeps track of the highest combo ever achieved in this play.
        /// This is handled automatically by ScoreProcessor.
        /// </summary>
        public readonly BindableInt HighestCombo = new BindableInt();

        public readonly List<JudgementInfo> Judgements;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScoreProcessor"/> class.
        /// </summary>
        /// <param name="hitObjectCount">Number of HitObjects. It is used for specifying Judgements collection Capacity</param>
        public ScoreProcessor(int hitObjectCount = 0)
        {
            Combo.ValueChanged += delegate { HighestCombo.Value = Math.Max(HighestCombo.Value, Combo.Value); };
            Judgements = new List<JudgementInfo>(hitObjectCount);
        }

        public void AddJudgement(JudgementInfo judgement)
        {
            Judgements.Add(judgement);

            UpdateCalculations(judgement);

            judgement.ComboAtHit = (ulong)Combo.Value;
            if (Health.Value == Health.MinValue && !HasFailed)
            {
                HasFailed = true;
                Failed?.Invoke();
            }
        }

        /// <summary>
        /// Update any values that potentially need post-processing on a judgement change.
        /// </summary>
        /// <param name="newJudgement">A new JudgementInfo that triggered this calculation. May be null.</param>
        protected abstract void UpdateCalculations(JudgementInfo newJudgement);

        // oh yes, its custom rank processor time
        protected virtual string CalculateRank()
        {
            if (Judgements.Count == 0)
                return "D";

            var list = new List<DisplayJudgement>();
            if (this is IHasDisplayJudgements osu)
                list = osu.DisplayJudgements;

            int missCount = Judgements.Count(j => j.Result == HitResult.Miss);
            int hit50Count = list.Count(j => j.Score == DisplayScore.Hit50);
            int hit100Count = list.Count(j => j.Score == DisplayScore.Hit100);
            int hit300Count = list.Count(j => j.Score == DisplayScore.Hit300);

            bool fullCombo = missCount == 0;

            double acc = Accuracy.Value; // 0..1

            // нюансы с миссами и 50
            if (acc == 1.0 && fullCombo && hit50Count == 0)
                return "SS";

            if (acc >= 0.98 && fullCombo)
                return "S+";

            if (acc >= 0.95 && fullCombo)
                return "S";

            if (acc >= 0.93)
                return "A+";

            if (acc >= 0.90)
                return "A";

            if (acc >= 0.87)
                return "B+";

            if (acc >= 0.80)
                return "B";

            if (acc >= 0.75)
                return "C+";

            if (acc >= 0.67)
                return "C";

            return "D";
        }
    }
}
