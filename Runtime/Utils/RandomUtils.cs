using UnityEngine;

namespace Bingyan
{
    public static class RandomUtils
    {
        public static T Choose<T>(params T[] choices)
            => choices[Random.Range(0, choices.Length)];
    }
}