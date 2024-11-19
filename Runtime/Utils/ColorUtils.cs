using System;
using UnityEngine;

namespace Bingyan
{
    public static class ColorUtils
    {
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
    }
}