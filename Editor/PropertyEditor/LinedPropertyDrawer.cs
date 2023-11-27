using UnityEngine;
using UnityEditor;

namespace Bingyan.Editor
{
    /// <summary>
    /// 对PropertyDrawer的一次封装，增加了一些方便绘制的方法
    /// </summary>
    public abstract class LinedPropertyDrawer : PropertyDrawer
    {
        protected const float SPACING = 2;
        protected const float TAB = 10;

        protected static float lineHeight => EditorGUIUtility.singleLineHeight;
        protected Rect pos;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            pos = new Rect(position.x, position.y, position.width, lineHeight);
        }

        protected void Next(float scale = 1)
        {
            pos.y += scale * (lineHeight + SPACING);
        }

        protected void AddTab()
        {
            pos.x += TAB;
            pos.width -= TAB;
        }

        protected void ReduceTab()
        {
            pos.x -= TAB;
            pos.width += TAB;
        }
    }
}