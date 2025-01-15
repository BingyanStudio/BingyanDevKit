using System;
using UnityEngine;

namespace Bingyan
{
    /// <summary>
    /// 整数范围结构体，可向 Unity 暴露以更好地在 Inspector 中编辑<br/>
    /// 最大和最小值已经在编辑器拓展中被保证<br/>
    /// 直接作为整数使用时，会返回范围内的一个随机数
    /// </summary>
    [Serializable]
    public struct IntRange
    {
        [SerializeField, Title("最小")] private int min;
        [SerializeField, Title("最大")] private int max;

        public IntRange(int min, int max)
        {
            this.min = Mathf.Min(min, max);
            this.max = Mathf.Max(min, max);
        }

        /// <summary>
        /// 范围的最小值
        /// </summary>
        public readonly int Min => min;

        /// <summary>
        /// 范围的最大值
        /// </summary>
        public readonly int Max => max;

        /// <summary>
        /// 选取范围内的一个随机数
        /// </summary>
        /// <returns>随机数</returns>
        public readonly int Rand() => UnityEngine.Random.Range(min, max + 1);

        /// <summary>
        /// 将一个值钳制在最小-最小值范围内
        /// </summary>
        /// <param name="input">需要钳制的值</param>
        /// <returns>钳制后的结果</returns>
        public readonly int Clamp(int input) => Mathf.Clamp(input, min, max);

        public static implicit operator int(IntRange range) => range.Rand();
        public static IntRange operator *(IntRange left, int right) => new(left.Min * right, left.Max * right);
        public static IntRange operator *(int left, IntRange right) => new(left * right.Min, left * right.Max);
        public static FloatRange operator *(IntRange left, float right) => new(left.Min * right, left.Max * right);
        public static FloatRange operator *(float left, IntRange right) => new(left * right.Min, left * right.Max);
    }

    /// <summary>
    /// 浮点数范围结构体，可向 Unity 暴露以更好地在 Inspector 中编辑<br/>
    /// 最大和最小值已经在编辑器拓展中被保证<br/>
    /// 直接作为浮点数使用时，会返回范围内的一个随机数
    /// </summary>
    [Serializable]
    public struct FloatRange
    {
        [SerializeField, Title("最小")] private float min;
        [SerializeField, Title("最大")] private float max;

        public FloatRange(float min, float max)
        {
            this.min = Mathf.Min(min, max);
            this.max = Mathf.Max(min, max);
        }

        /// <summary>
        /// 范围的最小值
        /// </summary>
        public readonly float Min => min;

        /// <summary>
        /// 范围的最大值
        /// </summary>
        public readonly float Max => max;

        /// <summary>
        /// 选取范围内的一个随机数
        /// </summary>
        /// <returns>随机数</returns>
        public readonly float Rand() => UnityEngine.Random.Range(min, max);

        /// <summary>
        /// 输入一个比例，对这个范围取线性插值
        /// </summary>
        /// <param name="ratio">比例，应当在 0~1 范围内</param>
        /// <returns>插值</returns>
        public readonly float Lerp(float ratio) => Mathf.Lerp(min, max, ratio);

        /// <summary>
        /// 将一个值钳制在最大-最小值范围内
        /// </summary>
        /// <param name="input">需要钳制的值</param>
        /// <returns>钳制后的结果</returns>
        public readonly float Clamp(float input) => Mathf.Clamp(input, min, max);

        public static implicit operator float(FloatRange range) => range.Rand();
        public static FloatRange operator *(FloatRange left, float right) => new(left.Min * right, left.Max * right);
        public static FloatRange operator *(float left, FloatRange right) => new(left * right.Min, left * right.Max);
        public readonly float this[float ratio] => Lerp(ratio);
    }

    /// <summary>
    /// 表示长方型范围的类型，包含左下角和右上角的坐标
    /// </summary>
    [Serializable]
    public class Rectangle
    {
        [SerializeField, Title("左下角")] private Transform bottomLeftCorner;
        [SerializeField, Title("右上角")] private Transform topRightCorner;

        public Vector2 BottomLeftCorner => bottomLeftCorner.position;
        public Vector2 TopRightCorner => topRightCorner.position;
        public Vector2 Center => (TopRightCorner + BottomLeftCorner) / 2;

        /// <summary>
        /// 判断一个点是否在范围内
        /// </summary>
        /// <param name="point">点的位置</param>
        /// <returns>是否在长方形范围内</returns>
        public bool Contains(Vector2 point) => point.x >= BottomLeftCorner.x && point.x <= TopRightCorner.x && point.y >= BottomLeftCorner.y && point.y <= TopRightCorner.y;
    }

    /// <summary>
    /// 表示一个浮点数曲线。可以是一个固定值，也可以是一个曲线。
    /// </summary>
    [Serializable]
    public class FloatCurve
    {
        public static readonly FloatCurve Zero = new() { mode = Mode.Value, value = 0 };

        [SerializeField, HideInInspector] private Mode mode;
        [SerializeField, HideInInspector] private float value;
        [SerializeField, HideInInspector] private AnimationCurve curve;

        /// <summary>
        /// 对曲线采样。<br/>
        /// 如果模式是固定值，则返回该值<br>
        /// 如果模式是曲线，则返回曲线在 ratio 处的采样值
        /// </summary>
        public float this[float ratio] => mode switch
        {
            Mode.Value => value,
            Mode.Curve => curve.Evaluate(ratio),
            _ => throw new NotImplementedException()
        };

        public enum Mode
        {
            Value, Curve
        }
    }
}