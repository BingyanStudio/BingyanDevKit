using UnityEngine;
using UnityEditor;

namespace Bingyan
{
    public abstract class RangeDrawerBase : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rect = EditorGUI.PrefixLabel(position, label);
            var width = rect.width / 2f - 10f;
            var size = new Vector2(width, rect.height);
            EditorGUI.LabelField(new Rect(rect.position + Vector2.right * width, new(20f, rect.height)), " - ", new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter });

            var propMin = property.FindPropertyRelative("min");
            var propMax = property.FindPropertyRelative("max");

            DrawField(propMin, propMax,
                        new Rect(rect.position, size),
                        new Rect(rect.position + Vector2.right * (width + 20f), size));
        }

        protected abstract void DrawField(SerializedProperty propMin, SerializedProperty propMax, Rect min, Rect max);
    }
}