using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Bingyan.Editor
{
    [CustomEditor(typeof(ClipConfig))]
    public class ClipConfigEditorInspector : UnityEditor.Editor
    {
        public static bool EditInWindow;
        private static List<bool> show;
        public override void OnInspectorGUI()
        {
            if (EditInWindow) return;

            if (GUILayout.Button("Edit in Window"))
            {
                ClipConfigEditorWindow.Init(serializedObject);
                EditInWindow = true;
            }

            if (GUILayout.Button("Generate C# Class"))
                ClipConfigEditorWindow.GenerateCode(serializedObject);

            var groups = serializedObject.FindProperty("groups");
            show ??= new();
            while (show.Count < groups.arraySize) show.Add(true);
            while (show.Count > groups.arraySize) show.RemoveAt(show.Count - 1);

            for (int i = 0; i < groups.arraySize; i++)
            {
                var group = groups.GetArrayElementAtIndex(i);
                show[i] = EditorGUILayout.Foldout(show[i], group.FindPropertyRelative("Name").stringValue, EditorStyles.foldoutHeader);
                if (show[i])
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