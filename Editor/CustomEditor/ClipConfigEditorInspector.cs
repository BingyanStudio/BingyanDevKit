using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Bingyan.Editor
{
    [CustomEditor(typeof(ClipConfig))]
    public class ClipConfigEditorInspector : UnityEditor.Editor
    {
        private static bool[] showing;

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("编辑", GUILayout.Height(30)))
                ClipConfigEditorWindow.Create();

            GUILayout.Space(10);

            if (GUILayout.Button("生成 C# 代码", GUILayout.Height(30)))
                ClipConfigEditorWindow.GenerateCode(serializedObject);

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