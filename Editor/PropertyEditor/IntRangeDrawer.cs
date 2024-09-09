using UnityEngine;
using UnityEditor;

namespace Bingyan
{
    [CustomPropertyDrawer(typeof(IntRange))]
    public class IntRangeDrawer : RangeDrawerBase
    {
        protected override void DrawField(SerializedProperty propMin, SerializedProperty propMax, Rect min, Rect max)
        {
            var minVal = propMin.intValue;
            var maxVal = propMax.intValue;
            propMin.intValue = EditorGUI.DelayedIntField(min, propMin.intValue);
            propMax.intValue = EditorGUI.DelayedIntField(max, propMax.intValue);

            if (minVal != propMin.intValue) propMax.intValue = Mathf.Max(propMax.intValue, propMin.intValue);
            else if (maxVal != propMax.intValue) propMin.intValue = Mathf.Min(propMax.intValue, propMin.intValue);
        }
    }
}