using System;

namespace Bingyan
{
    public static class ActionExtensions
    {
        public static void OneShot(this Action action, Action callback)
        {
            void oneShotCallback()
            {
                action -= oneShotCallback;
                callback?.Invoke();
            }
            action += oneShotCallback;
        }

        public static void OneShot<T>(this Action<T> action, Action<T> callback)
        {
            void oneShotCallback(T arg)
            {
                action -= oneShotCallback;
                callback?.Invoke(arg);
            }
            action += oneShotCallback;
        }

        public static void OneShot<T1, T2>(this Action<T1, T2> action, Action<T1, T2> callback)
        {
            void oneShotCallback(T1 arg, T2 arg2)
            {
                action -= oneShotCallback;
                callback?.Invoke(arg, arg2);
            }
            action += oneShotCallback;
        }
    }
}