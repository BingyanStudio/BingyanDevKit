using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Bingyan
{
    public static class ColorUtils
    {
        #region Utils

        /// <summary>
        /// 将标准的 HEX 颜色码转为 Color
        /// </summary>
        /// <param name="hex">Hex 码，格式为 #rgb, #rgba, #rrggbb 或 #rrggbbaa</param>
        /// <returns>颜色</returns>
        /// <exception cref="ArgumentException">当输入颜色码不合法时抛出异常</exception>
        public static Color FromHex(string hex)
        {
            if (ColorUtility.TryParseHtmlString(hex, out var color)) return color;
            else throw new ArgumentException($"Invalid hex string: {hex}");
        }

        #endregion

        #region Extensions

        /// <summary>
        /// 返回一个仅修改了 r 值的新颜色
        /// </summary>
        /// <param name="r">新的 r 值</param>
        /// <returns>修改后的颜色副本</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color WithR(this Color color, float r)
        {
            color.r = r;
            return color;
        }

        /// <summary>
        /// 返回一个仅修改了 g 值的新颜色
        /// </summary>
        /// <param name="g">新的 g 值</param>
        /// <returns>修改后的颜色副本</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color WithG(this Color color, float g)
        {
            color.g = g;
            return color;
        }

        /// <summary>
        /// 返回一个仅修改了 b 值的新颜色
        /// </summary>
        /// <param name="b">新的 b 值</param>
        /// <returns>修改后的颜色副本</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color WithB(this Color color, float b)
        {
            color.b = b;
            return color;
        }

        /// <summary>
        /// 返回一个仅修改了 a 值的新颜色
        /// </summary>
        /// <param name="a">新的 a 值</param>
        /// <returns>修改后的颜色副本</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color WithA(this Color color, float a)
        {
            color.a = a;
            return color;
        }

        #endregion
    }
}