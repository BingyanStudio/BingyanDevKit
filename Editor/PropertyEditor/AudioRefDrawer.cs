using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Bingyan.Editor
{
    [CustomPropertyDrawer(typeof(AudioRef))]
    public class AudioRefDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!AudioMapUtils.TryGetSO(out var map, out var _))
            {
                EditorGUI.LabelField(position, label.text, "未找到 AudioMap 配置");
                return;
            }

            var val = property.FindPropertyRelative("Name").stringValue;
            var groups = new List<string>() { "无" };

            var propGroups = map.FindProperty("groups");
            for (int i = 0; i < propGroups.arraySize; i++)
            {
                var group = propGroups.GetArrayElementAtIndex(i);
                var groupName = group.FindPropertyRelative("Name").stringValue;
                var infos = group.FindPropertyRelative("Infos");

                for (int j = 0; j < infos.arraySize; j++)
                {
                    var infoName = infos.GetArrayElementAtIndex(j).FindPropertyRelative("Name").stringValue;
                    groups.Add($"{groupName}/{infoName}");
                }
            }

            if (groups.Count == 0)
            {
                EditorGUI.LabelField(position, label.text, "没有配置任何音频");
                return;
            }

            var idx = groups.IndexOf(val);
            if (idx < 0)
            {
                idx = 0;
                if (val.Length > 0)
                    Log.W("AudioRef", $"ID 为 {val} 的音频未找到!");
            }
            idx = EditorGUI.Popup(position, label.text, idx, groups.ToArray());
            property.FindPropertyRelative("Name").stringValue = idx == 0 ? string.Empty : groups[idx];
        }
    }
}