using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Bingyan.Editor
{
    public class ClipConfigEditorWindow : EditorWindow
    {
        private const string PATTERN_NAME = @"^[a-zA-Z]\w+$";
        private const string PATTERN_SAME = @"_\d+$";
        private const float INTERVAL = .2f;

        private SerializedObject so;

        private static List<GroupVars> groupVars;
        private static SerializedProperty curInfo;

        private float preTime;
        private float deltaTime;

        private Vector2 scrollPos;

        public static void Init(SerializedObject so) => GetWindow<ClipConfigEditorWindow>("Clip Config").so = so;
        private void OnSelectionChange()
        {
            ClipConfigEditorInspector.EditInWindow = false;
            so = null;
        }
        public void OnGUI()
        {
            if (so is null) return;

            deltaTime = Time.realtimeSinceStartup - preTime;
            preTime = Time.realtimeSinceStartup;

            #region Target
            EditorGUILayout.BeginHorizontal();
            var scriptName = so.FindProperty("scriptName");
            string newScriptName = EditorGUILayout.DelayedTextField("Target Script Name", scriptName.stringValue);
            if (Regex.IsMatch(newScriptName, PATTERN_NAME)) scriptName.stringValue = newScriptName;
            if (GUILayout.Button("Generate C# Class")) GenerateCode(so);
            EditorGUILayout.EndHorizontal();
            #endregion


            EditorGUILayout.BeginHorizontal();

            #region Left Panel
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.MinWidth(scrollPos.x));

            var groups = so.FindProperty("groups");
            groupVars ??= new();

            #region Group Header
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Clip Groups", EditorStyles.boldLabel);
            if (GUILayout.Button("Add Group", GUILayout.Width(100)))
            {
                int idx = 1;
                groups.arraySize++;
                GroupVars newVars = new();
                while (groupVars.Any(v => v.Name == newVars.Name))
                    foreach (var vars in groupVars)
                        if (newVars.Name == vars.Name)
                            newVars.Name = Regex.Replace(newVars.Name, PATTERN_SAME, "_" + idx++.ToString());
                var group = groups.GetArrayElementAtIndex(groupVars.Count);
                group.FindPropertyRelative("Name").stringValue = newVars.Name;
                group.FindPropertyRelative("Infos").arraySize = 0;
                groupVars.Add(newVars);
            }
            EditorGUILayout.EndHorizontal();
            #endregion

            #region Group Body
            while (groupVars.Count < groups.arraySize) groupVars.Add(new());
            while (groupVars.Count > groups.arraySize) groupVars.RemoveAt(groupVars.Count - 1);

            for (int i = 0; i < groups.arraySize; i++)
            {
                var group = groups.GetArrayElementAtIndex(i);
                var infos = group.FindPropertyRelative("Infos");

                #region Info Header
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                groupVars[i].Show = EditorGUILayout.Foldout(groupVars[i].Show, GUIContent.none);
                var groupName = group.FindPropertyRelative("Name");
                groupVars[i].Name = groupName.stringValue;
                string newGroupName = EditorGUILayout.DelayedTextField(groupName.stringValue, EditorStyles.whiteLargeLabel);
                if (Regex.IsMatch(newGroupName, PATTERN_NAME) && !groupVars.Any(v => v.Name == newGroupName))
                    groupVars[i].Name = groupName.stringValue = newGroupName;
                bool add = false;
                if (GUILayout.Button("+", GUILayout.Width(25)))
                {
                    add = true;
                    groupVars[i].Show = true;
                }
                bool del = GUILayout.Button("Delete", GUILayout.Width(100));
                EditorGUILayout.EndHorizontal();
                #endregion

                #region Info Body
                while (groupVars[i].Info.Count < infos.arraySize) groupVars[i].Info.Add(new());
                while (groupVars[i].Info.Count > infos.arraySize) groupVars[i].Info.RemoveAt(groupVars.Count - 1);

                if (groupVars[i].Show)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(group.FindPropertyRelative("Bus"));
                    for (int j = 0; j < infos.arraySize; j++)
                    {

                        var info = infos.GetArrayElementAtIndex(j);
                        var infoName = info.FindPropertyRelative("Name");

                        groupVars[i].Info[j].Timer += deltaTime;
                        groupVars[i].Info[j].Name = infoName.stringValue;

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Info" + j.ToString(), GUILayout.Width(150));
                        if (groupVars[i].Info[j].Renaming)
                        {
                            string newInfoName = EditorGUILayout.TextField(infoName.stringValue);
                            if (Regex.IsMatch(newInfoName, PATTERN_NAME) && !groupVars[i].Info.Any(v => v.Name == newInfoName))
                                groupVars[i].Info[j].Name = infoName.stringValue = newInfoName;
                            if (GUILayout.Button("Commit", GUILayout.Width(100))) groupVars[i].Info[j].Renaming = false;
                        }
                        else if (GUILayout.Button(groupVars[i].Info[j].Name))
                        {
                            groupVars.ForEach(g => g.Info.ForEach(i => i.Renaming = false));
                            if (groupVars[i].Info[j].Timer < INTERVAL) groupVars[i].Info[j].Renaming = true;
                            else groupVars[i].Info[j].Timer = 0;
                            curInfo = info;
                        }
                        if (GUILayout.Button("-", GUILayout.Width(25)))
                        {
                            curInfo = null;
                            infos.DeleteArrayElementAtIndex(j);
                            groupVars[i].Info.RemoveAt(j);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUI.indentLevel--;
                }
                #endregion

                #region Late Update
                if (add)
                {
                    int idx = 1;
                    infos.arraySize++;
                    InfoVars newVars = new();
                    while (groupVars[i].Info.Any(v => v.Name == newVars.Name))
                        foreach (var vars in groupVars[i].Info)
                            if (newVars.Name == vars.Name)
                                newVars.Name = Regex.Replace(newVars.Name, PATTERN_SAME, "_" + idx++.ToString());
                    infos.GetArrayElementAtIndex(groupVars[i].Info.Count).FindPropertyRelative("Name").stringValue = newVars.Name;
                    groupVars[i].Info.Add(newVars);
                }
                if (del)
                {
                    curInfo = null;
                    groups.DeleteArrayElementAtIndex(i);
                    groupVars.RemoveAt(i);
                }
                #endregion
            }
            #endregion

            EditorGUILayout.EndVertical();
            GUILayout.EndScrollView();
            #endregion

            #region Right Panel
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.MinWidth(position.width / 2 - 7.5f));

            if (curInfo is null) GUILayout.Label("Clip Info", EditorStyles.boldLabel);
            else
            {
                GUILayout.Label(curInfo.FindPropertyRelative("Name").stringValue, EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(curInfo.FindPropertyRelative("Clips"));
                EditorGUILayout.PropertyField(curInfo.FindPropertyRelative("Loop"));
                #region Pitch
                var pitch = curInfo.FindPropertyRelative("Pitch");
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Pitch", GUILayout.Width(80));
                GUILayout.FlexibleSpace();
                var min = pitch.FindPropertyRelative("Min");
                EditorGUILayout.LabelField("Min", GUILayout.MaxWidth(60));
                min.floatValue = EditorGUILayout.DelayedFloatField(min.floatValue, GUILayout.MinWidth(40));
                var max = pitch.FindPropertyRelative("Max");
                EditorGUILayout.LabelField("Max", GUILayout.MaxWidth(60));
                max.floatValue = EditorGUILayout.DelayedFloatField(max.floatValue, GUILayout.MinWidth(40));
                if (max.floatValue < min.floatValue)
                {
                    float temp = min.floatValue;
                    min.floatValue = max.floatValue;
                    max.floatValue = temp;
                }
                EditorGUILayout.EndHorizontal();
                #endregion
                EditorGUILayout.PropertyField(curInfo.FindPropertyRelative("Stereo"));
            }

            EditorGUILayout.EndVertical();
            #endregion

            EditorGUILayout.EndHorizontal();

            so.ApplyModifiedProperties();
        }

        private class GroupVars
        {
            public bool Show = false;
            public string Name = "Group_0";
            public List<InfoVars> Info = new();
        }
        private class InfoVars
        {
            public string Name = "Info_0";
            public float Timer = 0;
            public bool Renaming = false;
        }

        public async static void GenerateCode(SerializedObject so)
        {
            #region Code Generate
            string scriptName = so.FindProperty("scriptName").stringValue;
            string codePath = "Assets/Resources/Audio/" + scriptName + ".cs";
            StringBuilder code = new StringBuilder();
            scriptName = so.FindProperty("scriptName").stringValue;
            code.Append("namespace Bingyan\n{\n\tpublic static class ").Append(scriptName).Append("\n\t{");
            var groups = so.FindProperty("groups");
            for (int i = 0; i < groups.arraySize; i++)
            {
                var group = groups.GetArrayElementAtIndex(i);
                string groupName = group.FindPropertyRelative("Name").stringValue;
                code.Append("\n\t\tpublic static class ").Append(groupName).Append("\n\t\t{");
                var infos = group.FindPropertyRelative("Infos");
                for (int j = 0; j < infos.arraySize; j++)
                {
                    string infoName = infos.GetArrayElementAtIndex(j).FindPropertyRelative("Name").stringValue;
                    code.Append("\n\t\t\tpublic static readonly ClipPlayer ").Append(infoName).Append(" = new(\"").Append(groupName).Append(" ").Append(infoName).Append("\");");
                }
                code.Append("\n\t\t}");
            }
            code.Append("\n\t}\n}");
            await File.WriteAllTextAsync(codePath, code.ToString());
            AssetDatabase.ImportAsset(codePath);
            #endregion
            #region Move Asset
            string srcPath = AssetDatabase.GetAssetPath(so.targetObject);
            string assetPath = "Assets/Config/Audio/" + scriptName + ".asset";
            if (srcPath != assetPath)
            {
                AssetDatabase.MoveAsset(srcPath, assetPath);
                so.targetObject.name = scriptName;
            }
            #endregion
        }
    }
}
