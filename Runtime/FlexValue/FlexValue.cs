using System;

namespace Bingyan
{
    public abstract class FlexValue<T> 
    {
        private T value, target;
        private Action<T> onValueChanged, onTargetReached;

        protected FlexValue(T defaultValue)
        {
            value = target = defaultValue;
        }

        public FlexValue<T> ValueChanged(Action<T> callback)
        {
            onValueChanged = callback;
            return this;
        }

        public FlexValue<T> TargetReached(Action<T> callback)
        {
            onTargetReached = callback;
            return this;
        }

        public void Update(float time)
        {
            UpdateValue(time, value, target, out value, out var changed, out var targetReached);
            if (changed) onValueChanged?.Invoke(value);
            if (targetReached) onTargetReached?.Invoke(value);
        }

        protected abstract void UpdateValue(float time, T current, T target, out T result, out bool changed, out bool targetReached);

        public void SetValue(T value)
        {
            this.value = value;
            onValueChanged?.Invoke(value);
        }

        public void SetTarget(T target)
        {
            this.target = target;
        }

        public void SetImmediately() => SetImmediately(target);

        public void SetImmediately(T val)
        {
            value = target = val;
            onValueChanged?.Invoke(val);
        }

        public static implicit operator T(FlexValue<T> value) => value.value;
    }

    public abstract class LinearValue<T> : FlexValue<T>
    {
        protected float speed;

        public LinearValue(T defaultValue, float speed)
            : base(defaultValue)
        {
            this.speed = speed;
        }
    }

    public abstract class LerpValue<T> : FlexValue<T>
    {
        protected float ratio, error = 1e-3f;

        public LerpValue(T defaultValue, float ratio)
            : base(defaultValue)
        {
            this.ratio = ratio;
        }
    }
}