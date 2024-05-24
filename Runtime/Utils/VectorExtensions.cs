using System;
using UnityEngine;

namespace Bingyan
{
    /// <summary>
    /// 允许直接对 <see cref="Vector2"/>, <see cref="Vector3"/> 与 <see cref="Color"/> 的分量进行修改<br/> 的拓展
    /// </summary>
    public static class VectorExtensions
    {
        public static Vector3 SetX(this Vector3 vec, float x)
        {
            vec.x = x;
            return vec;
        }

        public static Vector3 SetY(this Vector3 vec, float y)
        {
            vec.y = y;
            return vec;
        }

        public static Vector3 SetZ(this Vector3 vec, float z)
        {
            vec.z = z;
            return vec;
        }

        public static Vector3 ForEach(this Vector3 vec, Func<float, float> action)
        {
            vec.x = action.Invoke(vec.x);
            vec.y = action.Invoke(vec.y);
            vec.z = action.Invoke(vec.z);
            return vec;
        }

        public static Vector2 SetX(this Vector2 vec, float x)
        {
            vec.x = x;
            return vec;
        }

        public static Vector2 SetY(this Vector2 vec, float y)
        {
            vec.y = y;
            return vec;
        }

        public static Vector2 ForEach(this Vector2 vec, Func<float, float> action)
        {
            vec.x = action.Invoke(vec.x);
            vec.y = action.Invoke(vec.y);
            return vec;
        }

        public static Color SetR(this Color color, float r)
        {
            color.r = r;
            return color;
        }

        public static Color SetG(this Color color, float g)
        {
            color.g = g;
            return color;
        }

        public static Color SetB(this Color color, float b)
        {
            color.b = b;
            return color;
        }

        public static Color SetA(this Color color, float a)
        {
            color.a = a;
            return color;
        }
    }
}