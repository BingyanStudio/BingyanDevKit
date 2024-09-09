using UnityEngine;
using UnityEditor;

namespace Bingyan
{
    [CustomPropertyDrawer(typeof(FloatRange))]
    public class FloatRangeDrawer : RangeDrawerBase
    {
        protected override void DrawField(SerializedProperty propMin, SerializedProperty propMax, Rect min, Rect max)
        {
            var minVal = propMin.floatValue;
            var maxVal = propMax.floatValue;
            propMin.floatValue = EditorGUI.DelayedFloatField(min, propMin.floatValue);
            propMax.floatValue = EditorGUI.DelayedFloatField(max, propMax.floatValue);

            if (minVal != propMin.floatValue) propMax.floatValue = Mathf.Max(propMax.floatValue, propMin.floatValue);
            else if (maxVal != propMax.floatValue) propMin.floatValue = Mathf.Min(propMax.floatValue, propMin.floatValue);
        }
    }
}