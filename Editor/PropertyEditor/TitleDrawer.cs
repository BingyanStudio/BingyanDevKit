using UnityEngine;
using UnityEditor;
using System.Linq;

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

            switch (property.propertyType)
            {
                case SerializedPropertyType.Generic:
                    showChild = EditorGUI.PropertyField(pos, property, label, false);
                    if (property.hasVisibleChildren && showChild)
                    {
                        EditorGUI.indentLevel++;
                        var depth = property.depth;
                        property.NextVisible(true);
                        do
                        {
                            if (property.depth == depth) break;
                            Next();
                            DrawProperty(property);
                        } while (property.NextVisible(false));
                        EditorGUI.indentLevel--;
                    }
                    break;

                case SerializedPropertyType.String:
                default:
                    EditorGUI.PropertyField(position, property, label);
                    break;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String
                    && fieldInfo.GetCustomAttributes(false)
                                .Where(i => i is MultilineAttribute)
                                .FirstOrDefault() is MultilineAttribute attr)
                return (lineHeight + spacing) * attr.lines;
            return EditorGUI.GetPropertyHeight(property);
        }

        private void DrawProperty(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Generic:
                    showChild = EditorGUI.PropertyField(pos, property, false);
                    if (property.hasVisibleChildren && showChild)
                    {
                        EditorGUI.indentLevel++;
                        var depth = property.depth;
                        property.NextVisible(true);
                        while (property.NextVisible(false))
                        {
                            if (property.depth == depth) break;
                            Next();
                            EditorGUI.PropertyField(pos, property);
                        }
                        EditorGUI.indentLevel--;
                    }
                    break;

                case SerializedPropertyType.String:
                default:
                    EditorGUI.PropertyField(pos, property);
                    break;
            }
        }
    }
}