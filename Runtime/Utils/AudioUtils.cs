using UnityEngine;

namespace Bingyan
{
    public static class AudioUtils
    {
        public static float ValueToDb(float value)
            => Mathf.Log10(Mathf.Max(1e-4f, value)) * 20;

        public static float DbToValue(float db)
            => Mathf.Pow(10, db / 20);
    }
}