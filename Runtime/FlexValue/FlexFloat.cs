using UnityEngine;

namespace Bingyan
{
    public class LinearFloat : LinearValue<float>
    {
        public LinearFloat(float defaultValue, float speed)
            : base(defaultValue, speed) { }

        protected override void UpdateValue(
            float time,
            float current,
            float target,
            out float result,
            out bool changed,
            out bool targetReached)
        {
            if (current == target)
            {
                result = current;

                changed = false;
                targetReached = false;
            }
            else
            {
                result = Mathf.MoveTowards(current, target, speed * time);

                changed = true;
                targetReached = result == target;
            }
        }
    }

    public class LerpFloat : LerpValue<float>
    {
        public LerpFloat(float defaultValue, float speed)
            : base(defaultValue, speed) { }

        protected override void UpdateValue(
            float time,
            float current,
            float target,
            out float result,
            out bool changed,
            out bool targetReached)
        {
            if (current == target)
            {
                result = current;

                changed = false;
                targetReached = false;
            }
            else
            {
                result = Mathf.Lerp(current, target, ratio);
                if (Mathf.Abs(result - target) < error) result = target;

                changed = true;
                targetReached = result == target;
            }
        }
    }
}