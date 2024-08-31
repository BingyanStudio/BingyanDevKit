using System;
using UnityEngine;

public static class ColorUtils
{
    public static Color FromHex(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out var color)) return color;
        else throw new ArgumentException($"Invalid hex string: {hex}");
    }
}