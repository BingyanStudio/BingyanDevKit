using System;
using Bingyan;
using UnityEngine;

namespace Bingyan
{
    [Serializable]
    public struct IntRange
    {
        [SerializeField, Title("最小")] private int min;
        [SerializeField, Title("最大")] private int max;

        public readonly int Min => min;
        public readonly int Max => max;
        public readonly int Rand() => UnityEngine.Random.Range(min, max + 1);
        public readonly int Clamp(int input) => Mathf.Clamp(input, min, max);

        public static implicit operator int(IntRange range) => range.Rand();
    }

    [Serializable]
    public struct FloatRange
    {
        [SerializeField, Title("最小")] private float min;
        [SerializeField, Title("最大")] private float max;
        public readonly float Min => min;
        public readonly float Max => max;
        public readonly float Rand() => UnityEngine.Random.Range(min, max);
        public readonly float Lerp(float ratio) => Mathf.Lerp(min, max, ratio);
        public readonly float Clamp(float input) => Mathf.Clamp(input, min, max);

        public static implicit operator float(FloatRange range) => range.Rand();
        public readonly float this[float ratio] => Lerp(ratio);
    }

    [Serializable]
    public class Rectangle
    {
        [SerializeField, Title("左下角")] private Transform bottomLeftCorner;
        [SerializeField, Title("右上角")] private Transform topRightCorner;

        public Vector2 BottomLeftCorner => bottomLeftCorner.position;
        public Vector2 TopRightCorner => topRightCorner.position;
        public Vector2 Center => (TopRightCorner + BottomLeftCorner) / 2;
        public bool Contains(Vector2 point) => point.x >= BottomLeftCorner.x && point.x <= TopRightCorner.x && point.y >= BottomLeftCorner.y && point.y <= TopRightCorner.y;
    }
}