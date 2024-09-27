using System;
using UnityEngine;

namespace Bingyan
{
    public class LinearAnimatedFloat : LinearAnimatedValue<float>
    {
        public LinearAnimatedFloat(float defaultValue, float speed, Action<float> onValueChanged = null)
            : base(defaultValue, speed, onValueChanged) { }

        protected override bool UpdateValue(float time, float current, float target, out float result)
        {
            if (current == target)
            {
                result = current;
                return false;
            }

            result = Mathf.MoveTowards(current, target, speed * time);
            return true;
        }
    }

    public class LerpAnimatedFloat : LerpAnimatedValue<float>
    {
        public LerpAnimatedFloat(float defaultValue, float speed, Action<float> onValueChanged = null)
            : base(defaultValue, speed, onValueChanged) { }

        protected override bool UpdateValue(float time, float current, float target, out float result)
        {
            if (current == target)
            {
                result = current;
                return false;
            }

            result = Mathf.Lerp(current, target, ratio);
            if (Mathf.Abs(result - target) < 1e-4) result = target;

            return true;
        }
    }
}