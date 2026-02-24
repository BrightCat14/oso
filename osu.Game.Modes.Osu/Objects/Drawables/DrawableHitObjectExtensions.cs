using osu.Game.Modes.Objects.Drawables;
using System.Reflection;

namespace osu.Game.Modes.Osu.Objects.Drawables
{
    public static class DrawableHitObjectExtensions
    {
        public static void RaiseJudgement(this DrawableHitObject d, JudgementInfo j)
        {
            // Invoke the OnJudgement event via reflection to avoid changing DrawableHitObject visibility.
            var field = typeof(DrawableHitObject).GetField("OnJudgement", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            var handler = field?.GetValue(d) as System.Delegate;
            handler?.DynamicInvoke(d, j);
        }
    }
}
