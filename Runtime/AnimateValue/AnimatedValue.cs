using System;
using UnityEngine;

namespace Bingyan
{
    public abstract class AnimatedValue<T>
    {
        private T value, target;
        private readonly Action<T> onValueChanged;

        protected AnimatedValue(T defaultValue, Action<T> onValueChanged = null)
        {
            value = target = defaultValue;
            this.onValueChanged = onValueChanged;
        }

        public void Update(float time)
        {
            if (UpdateValue(time, value, target, out value))
                onValueChanged?.Invoke(value);
        }

        protected abstract bool UpdateValue(float time, T current, T target, out T result);

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

        public static implicit operator T(AnimatedValue<T> value) => value.value;
    }

    public abstract class LinearAnimatedValue<T> : AnimatedValue<T>
    {
        protected float speed;

        public LinearAnimatedValue(T defaultValue, float speed, Action<T> onValueChanged = null)
            : base(defaultValue, onValueChanged)
        {
            this.speed = speed;
        }
    }

    public abstract class LerpAnimatedValue<T> : AnimatedValue<T>
    {
        protected float ratio;

        public LerpAnimatedValue(T defaultValue, float ratio, Action<T> onValueChanged = null)
            : base(defaultValue, onValueChanged)
        {
            this.ratio = ratio;
        }
    }
}