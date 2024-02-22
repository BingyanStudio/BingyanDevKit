using System.Reflection;
using UnityEngine;
using UnityEditor;
using static UnityEditor.EditorGUI;

namespace Bingyan.Editor
{
    /// <summary>
    /// 绘制 <see cref="AudioCallback"/> 的自定义编辑器
    /// </summary>
    [CustomPropertyDrawer(typeof(AudioCallback))]
    public class AudioCallbackDrawer : LinedPropertyDrawer
    {
        private GUIContent sourceLabel = new GUIContent("音源"), typeLabel = new GUIContent("播放方式");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);

            GUI.Box(new Rect(pos.position, new Vector2(pos.width, GetPropertyHeight(property, label))), "");
            pos.position += Vector2.one * spacing * 2;
            pos.size -= Vector2.right * spacing * 4;

            var attr = fieldInfo.GetCustomAttribute<TooltipAttribute>();
            var title = attr != null ? attr.tooltip : label.text;
            LabelField(pos, title);
            Next();
            var arr = property.FindPropertyRelative("elements");
            if (arr.arraySize == 0)
            {
                HelpBox(new Rect(pos.position, new Vector2(pos.width, lineHeight * 2)), "暂无绑定的音频", MessageType.Info);
                Next(2);
            }
            else
            {
                AddTab();
                for (int i = 0; i < arr.arraySize; i++)
                {
                    LabelField(pos, i + 1 + "");
                    if (GUI.Button(new Rect(pos.position + (pos.width - 80) * Vector2.right, new Vector2(80, pos.height)), "删除"))
                    {
                        arr.DeleteArrayElementAtIndex(i--);
                        continue;
                    }
                    Next();
                    var item = arr.GetArrayElementAtIndex(i);
                    PropertyField(pos, item.FindPropertyRelative("source"), sourceLabel);
                    Next();
                    PropertyField(pos, item.FindPropertyRelative("type"), typeLabel);
                    Next(1.5f);
                }
                ReduceTab();
            }
            if (GUI.Button(new Rect(pos.position + (pos.width - 100) * Vector2.right, new Vector2(100, pos.height)), "添加"))
                arr.arraySize++;
            property.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var h = lineHeight + spacing;
            var arr = property.FindPropertyRelative("elements");
            if (arr.arraySize == 0) return h * 4 + spacing * 4;
            else return h * (2 + arr.arraySize * 3.5f) + spacing * 4;
        }
    }
}