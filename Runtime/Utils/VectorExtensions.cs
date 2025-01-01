using System;
using UnityEngine;

namespace Bingyan
{
    /// <summary>
    /// 允许直接对 <see cref="Vector2"/>, <see cref="Vector3"/> 与 <see cref="Color"/> 的分量进行修改<br/> 的拓展
    /// </summary>
    public static class VectorExtensions
    {
        /// <summary>
        /// 将 <see cref="Vector3"/> 转为 <see cref="Vector2"/>, 抛弃 z 轴分量<br/>
        /// 比强转型方便!
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector2 Vec2(this Vector3 vec) => vec;

        /// <summary>
        /// 将 <see cref="Vector2"/> 转为 <see cref="Vector3"/>, 添加 z 轴分量为 0 <br/>
        /// 比强转型方便!
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector3 Vec3(this Vector2 vec) => vec;

        /// <summary>
        /// 返回一个仅修改了原向量 x 轴数值的向量
        /// </summary>
        /// <param name="x">新的 x 值</param>
        /// <returns>修改后的向量副本</returns>
        public static Vector3 SetX(this Vector3 vec, float x)
        {
            vec.x = x;
            return vec;
        }

        /// <summary>
        /// 返回一个仅修改了原向量 y 轴数值的向量
        /// </summary>
        /// <param name="y">新的 y 值</param>
        /// <returns>修改后的向量副本</returns>
        public static Vector3 SetY(this Vector3 vec, float y)
        {
            vec.y = y;
            return vec;
        }

        /// <summary>
        /// 返回一个仅修改了原向量 z 轴数值的向量
        /// </summary>
        /// <param name="z">新的 z 值</param>
        /// <returns>修改后的向量副本</returns>
        public static Vector3 SetZ(this Vector3 vec, float z)
        {
            vec.z = z;
            return vec;
        }

        /// <summary>
        /// 针对向量的每个分量进行操作
        /// </summary>
        /// <param name="action">操作</param>
        /// <returns>操作后的向量</returns>
        public static Vector3 ForEach(this Vector3 vec, Func<float, float> action)
        {
            vec.x = action.Invoke(vec.x);
            vec.y = action.Invoke(vec.y);
            vec.z = action.Invoke(vec.z);
            return vec;
        }

        /// <summary>
        /// 将向量的 X 分量翻转
        /// </summary>
        /// <returns>翻转后的新向量</returns>
        public static Vector3 FlipX(this Vector3 vec)
        {
            vec.x = -vec.x;
            return vec;
        }

        /// <summary>
        /// 将向量的 Y 分量翻转
        /// </summary>
        /// <returns>翻转后的新向量</returns>
        public static Vector3 FlipY(this Vector3 vec)
        {
            vec.y = -vec.y;
            return vec;
        }

        /// <summary>
        /// 将向量的 Z 分量翻转
        /// </summary>
        /// <returns>翻转后的新向量</returns>
        public static Vector3 FlipZ(this Vector3 vec)
        {
            vec.z = -vec.z;
            return vec;
        }

        /// <summary>
        /// 返回一个仅修改了原向量 x 轴数值的向量
        /// </summary>
        /// <param name="x">新的 x 值</param>
        /// <returns>修改后的向量副本</returns>
        public static Vector2 SetX(this Vector2 vec, float x)
        {
            vec.x = x;
            return vec;
        }

        /// <summary>
        /// 返回一个仅修改了原向量 y 轴数值的向量
        /// </summary>
        /// <param name="y">新的 y 值</param>
        /// <returns>修改后的向量副本</returns>
        public static Vector2 SetY(this Vector2 vec, float y)
        {
            vec.y = y;
            return vec;
        }

        /// <summary>
        /// 将向量的 X 分量翻转
        /// </summary>
        /// <returns>翻转后的新向量</returns>
        public static Vector2 FlipX(this Vector2 vec)
        {
            vec.x = -vec.x;
            return vec;
        }

        /// <summary>
        /// 若输入的数据小于 0 则将向量的 X 分量翻转
        /// </summary>
        /// <returns>翻转后的新向量</returns>
        public static Vector2 FlipX(this Vector2 vec, float dir)
        {
            if (dir < 0) vec.x = -vec.x;
            return vec;
        }

        /// <summary>
        /// 将向量的 Y 分量翻转
        /// </summary>
        /// <returns>翻转后的新向量</returns>
        public static Vector2 FlipY(this Vector2 vec)
        {
            vec.y = -vec.y;
            return vec;
        }

        /// <summary>
        /// 若输入的数据小于 0 则将向量的 Y 分量翻转
        /// </summary>
        /// <returns>翻转后的新向量</returns>
        public static Vector2 FlipY(this Vector2 vec, float dir)
        {
            if (dir < 0) vec.y = -vec.y;
            return vec;
        }

        /// <summary>
        /// 针对向量的每个分量进行操作
        /// </summary>
        /// <param name="action">操作</param>
        /// <returns>操作后的向量</returns>
        public static Vector2 ForEach(this Vector2 vec, Func<float, float> action)
        {
            vec.x = action.Invoke(vec.x);
            vec.y = action.Invoke(vec.y);
            return vec;
        }

        /// <summary>
        /// 返回一个仅修改了 r 值的新颜色
        /// </summary>
        /// <param name="r">新的 r 值</param>
        /// <returns>修改后的颜色副本</returns>
        public static Color SetR(this Color color, float r)
        {
            color.r = r;
            return color;
        }

        /// <summary>
        /// 返回一个仅修改了 g 值的新颜色
        /// </summary>
        /// <param name="g">新的 g 值</param>
        /// <returns>修改后的颜色副本</returns>
        public static Color SetG(this Color color, float g)
        {
            color.g = g;
            return color;
        }

        /// <summary>
        /// 返回一个仅修改了 b 值的新颜色
        /// </summary>
        /// <param name="b">新的 b 值</param>
        /// <returns>修改后的颜色副本</returns>
        public static Color SetB(this Color color, float b)
        {
            color.b = b;
            return color;
        }

        /// <summary>
        /// 返回一个仅修改了 a 值的新颜色
        /// </summary>
        /// <param name="a">新的 a 值</param>
        /// <returns>修改后的颜色副本</returns>
        public static Color SetA(this Color color, float a)
        {
            color.a = a;
            return color;
        }

        /// <summary>
        /// 获取方向相同, 长度为 length 的向量
        /// </summary>
        /// <param name="length">长度</param>
        /// <returns>新向量</returns>
        public static Vector2 SetLength(this Vector2 vec, float length)
            => vec.normalized * length;

        /// <summary>
        /// 等效于 Vector2.ClampMagnitude(vec, length)
        /// </summary>
        /// <param name="length">限制的最大长度</param>
        /// <returns>限制后的向量</returns>
        public static Vector2 LimitLength(this Vector2 vec, float length)
            => Vector2.ClampMagnitude(vec, length);

        /// <summary>
        /// 将向量旋转指定角度, 顺时针为正向
        /// </summary>
        /// <param name="angle">旋转的角度</param>
        /// <returns>旋转后的向量</returns>
        public static Vector2 Rotate(this Vector2 vec, float angle)
            => Quaternion.Euler(0, 0, angle) * vec;

        /// <summary>
        /// 对二维向量进行点乘
        /// </summary>
        public static float Dot(this Vector2 vec, Vector2 other)
            => Vector2.Dot(vec, other);

        /// <summary>
        /// 对三维向量进行点乘
        /// </summary>
        public static float Dot(this Vector3 vec, Vector3 other)
            => Vector3.Dot(vec, other);

        /// <summary>
        /// 对三维向量进行叉乘
        /// </summary>
        public static Vector3 Cross(this Vector3 vec, Vector3 other)
            => Vector3.Cross(vec, other);

        /// <summary>
        /// 对三维向量按元素相乘
        /// </summary>
        public static Vector3 Mul(this Vector3 vec, Vector3 other)
        {
            vec.x *= other.x;
            vec.y *= other.y;
            vec.z *= other.z;
            return vec;
        }
    }
}