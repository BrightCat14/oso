// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Game.Modes.Objects.Drawables;
using osu.Game.Modes.Osu.Objects;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using OpenTK;
using osu.Framework.Input;

namespace osu.Game.Modes.Osu.Objects.Drawables
{
    class DrawableSpinner : DrawableOsuHitObject
    {
        private Spinner spinner;

        private bool isSpinning;

        public DrawableSpinner(Spinner s) : base(s)
        {
            spinner = s;
            AutoSizeAxes = Axes.Both;
        }

        protected override void UpdateState(ArmedState state)
        {
            // simple fade behaviour similar to other objects
            switch (state)
            {
                case ArmedState.Hit:
                    FadeOut(400);
                    break;
                case ArmedState.Miss:
                    FadeOut(400);
                    break;
            }
        }

        protected override void CheckJudgement(bool userTriggered)
        {
            // Basic spinner judgement: if the player is holding a mouse button or key at judgement time, count as hit.
            Judgement.Result = isSpinning ? HitResult.Hit : HitResult.Miss;
        }

        public override bool HandleInput => true;

        protected override bool OnMouseDown(InputState state, MouseDownEventArgs args)
        {
            isSpinning = true;
            return true;
        }

        protected override bool OnMouseUp(InputState state, MouseUpEventArgs args)
        {
            // update spinning based on whether main button still held
            isSpinning = state.Mouse.HasMainButtonPressed;
            return true;
        }

        protected override bool OnMouseMove(InputState state)
        {
            isSpinning = state.Mouse.HasMainButtonPressed;
            return false;
        }

        protected override bool OnKeyDown(InputState state, KeyDownEventArgs args)
        {
            // Keys are represented by OpenTK.Input.Key in this codebase. Use comparison via integer values.
            var k = (int)args.Key;
            // OpenTK Key.Z and Key.X are available in OpenTK.Input namespace; compare to their int values.
            if (k == (int)OpenTK.Input.Key.Z || k == (int)OpenTK.Input.Key.X)
            {
                isSpinning = true;
                return true;
            }

            return base.OnKeyDown(state, args);
        }

        protected override bool OnKeyUp(InputState state, KeyUpEventArgs args)
        {
            return base.OnKeyUp(state, args);
        }
    }
}
