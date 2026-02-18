// Copyright (c) 2007-2016 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

using System;
using System.Collections.Generic;
using System.Globalization;

namespace osu.Framework.Configuration
{
    public class Bindable<T> : IBindable
    {
        private T value;

        public T Default;

        public virtual bool IsDefault => Equals(value, Default);

        public event EventHandler ValueChanged;

        public virtual T Value
        {
            get { return value; }
            set
            {
                if (EqualityComparer<T>.Default.Equals(this.value, value)) return;

                this.value = value;

                TriggerChange();
            }
        }

        public Bindable(T value = default(T))
        {
            this.value = value;
        }

        public static implicit operator T(Bindable<T> value)
        {
            return value.Value;
        }

        /// <summary>
        /// Welds two bindables together such that they update each other and stay in sync.
        /// </summary>
        /// <param name="v">The foreign bindable to weld.</param>
        /// <param name="transferValue">Whether we should transfer the value from the foreign bindable on weld.</param>
        public virtual void Weld(Bindable<T> v, bool transferValue = true)
        {
            if (transferValue) Value = v.Value;

            ValueChanged += delegate { v.Value = Value; };
            v.ValueChanged += delegate { Value = v.Value; };
        }

        public virtual bool Parse(object s)
        {
            string str = s as string;
            if (str == null) return false;

            if (int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out int i))
            {
                Value = (T)(object)i;
                return true;
            }

            if (double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out double d))
            {
                int result;

                if (d >= 0 && d <= 1)
                    result = (int)Math.Round(d * 100);
                else
                    result = (int)Math.Round(d);

                Value = (T)(object)result;
                return true;
            }

            return false;
        }


        public void TriggerChange()
        {
            ValueChanged?.Invoke(this, null);
        }

        public void UnbindAll()
        {
            ValueChanged = null;
        }

        string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public override string ToString()
        {
            return value?.ToString() ?? string.Empty;
        }

        internal void Reset()
        {
            Value = Default;
        }
    }
}
