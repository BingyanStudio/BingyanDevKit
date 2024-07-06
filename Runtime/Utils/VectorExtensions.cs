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
    }
}