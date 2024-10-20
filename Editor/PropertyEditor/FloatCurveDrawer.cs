using UnityEngine;
using UnityEditor;
using System;

namespace Bingyan
{
    [CustomPropertyDrawer(typeof(FloatCurve))]
    public class FloatCurveDrawer : PropertyDrawer
    {
        private const float MODE_POPUP_WIDTH = 70;

        private static readonly string[] modes = new string[] { "常数", "曲线" };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var propMode = property.FindPropertyRelative("mode");
            var propValue = property.FindPropertyRelative("value");
            var propCurve = property.FindPropertyRelative("curve");

            propMode.enumValueIndex = (int)EditorGUI.Popup(
                                            new Rect(position.x + position.width - MODE_POPUP_WIDTH, position.y, MODE_POPUP_WIDTH, position.height),
                                            propMode.enumValueIndex,
                                            modes);

            switch ((FloatCurve.Mode)propMode.enumValueIndex)
            {
                case FloatCurve.Mode.Value:
                    EditorGUI.PropertyField(new Rect(position.x, position.y, position.width - MODE_POPUP_WIDTH, position.height), propValue, label);
                    break;

                case FloatCurve.Mode.Curve:
                    EditorGUI.PropertyField(new Rect(position.x, position.y, position.width - MODE_POPUP_WIDTH, position.height), propCurve, label);
                    break;
            }
        }
    }
}