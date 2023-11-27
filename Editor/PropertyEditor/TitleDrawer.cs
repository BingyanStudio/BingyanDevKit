using UnityEngine;
using UnityEditor;

namespace Bingyan.Editor
{
    /// <summary>
    /// 绘制 <see cref="TitleAttribute"/> 对属性框的更改
    /// <para>汉化神器，妈妈再也不用担心策划看不懂英文了！</para>
    /// <para>注意：尽量不要对【列表】类属性用这个Attr，会画不出来，很奇怪</para>
    /// <para>如果有必要，请使用 <see cref="HeaderAttribute"/></para>
    /// </summary>
    [CustomPropertyDrawer(typeof(TitleAttribute))]
    public class TitleDrawer : LinedPropertyDrawer
    {
        private bool showChild = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);
            label.text = ((TitleAttribute)attribute).Label;
            showChild = EditorGUI.PropertyField(pos, property, label, false);
            if (property.hasVisibleChildren && showChild)
            {
                EditorGUI.indentLevel++;
                var depth = property.depth;
                while (property.NextVisible(true))
                {
                    if (property.depth == depth) break;
                    Next();
                    EditorGUI.PropertyField(pos, property);
                }
                EditorGUI.indentLevel--;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (lineHeight + SPACING) * (showChild ? property.CountInProperty() : 1);
        }
    }
}