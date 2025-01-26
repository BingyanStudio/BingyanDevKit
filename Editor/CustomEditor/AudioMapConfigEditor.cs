using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Bingyan.Editor
{
    [CustomEditor(typeof(AudioMapConfig))]
    public class AudioMapConfigEditor : UnityEditor.Editor
    {
        private static bool[] showing;

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("编辑", GUILayout.Height(30)))
                AudioMapConfigWindow.Create();

            GUILayout.Space(10);


            var scriptName = serializedObject.FindProperty("scriptName");
            string newScriptName = EditorGUILayout.DelayedTextField("脚本名称", scriptName.stringValue);

            if (Regex.IsMatch(newScriptName, @"^[a-zA-Z]\w+$")) scriptName.stringValue = newScriptName;
            else DialogUtils.Show("无效的名称", "脚本名称应当仅包含字母、数字和下划线！", isErr: false);

            if (GUILayout.Button("生成 C# 代码", GUILayout.Height(24))) AudioMapConfigWindow.GenerateCode(serializedObject);

            var groups = serializedObject.FindProperty("groups");

            if (showing != null)
            {
                if (showing.Length < groups.arraySize)
                {
                    var t = new bool[groups.arraySize + 4];
                    showing?.CopyTo(t, 0);
                    showing = t;
                }
            }
            else showing = new bool[groups.arraySize];

            for (int i = 0; i < groups.arraySize; i++)
            {
                var group = groups.GetArrayElementAtIndex(i);
                showing[i] = EditorGUILayout.Foldout(showing[i], group.FindPropertyRelative("Name").stringValue, EditorStyles.foldoutHeader);
                if (showing[i])
                {
                    EditorGUI.indentLevel++;
                    var infos = group.FindPropertyRelative("Infos");
                    for (int j = 0; j < infos.arraySize; j++)
                    {
                        var info = infos.GetArrayElementAtIndex(j);
                        EditorGUILayout.LabelField(info.FindPropertyRelative("Name").stringValue, EditorStyles.label);
                    }
                    EditorGUI.indentLevel--;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}