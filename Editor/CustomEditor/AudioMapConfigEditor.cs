using System;
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


            var scPath = serializedObject.FindProperty("scriptPath");
            GUI.enabled = false;
            EditorGUILayout.TextField("生成脚本", scPath.stringValue);
            GUI.enabled = true;

            EditorGUILayout.BeginHorizontal(GUILayout.Height(24));
            if (GUILayout.Button("选择路径", GUILayout.Height(24)))
            {
                var path = EditorUtility.SaveFilePanelInProject("选择代码生成路径", "AudioMap.Generated", "cs", "请选择生成代码的路径");
                if (path.Length > 0) scPath.stringValue = path;
            }
            if (GUILayout.Button("生成", GUILayout.Height(24))) AudioMapUtils.GenerateCode(serializedObject);
            EditorGUILayout.EndHorizontal();


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