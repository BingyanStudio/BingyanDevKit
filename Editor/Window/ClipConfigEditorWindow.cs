using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Bingyan.Editor
{
    [EditorWindowTitle(title = "音频配置")]
    public class ClipConfigEditorWindow : EditorWindow
    {
        private const string PATTERN_NAME = @"^[a-zA-Z]\w+$";
        private const string PATTERN_SAME = @"_\d+$";
        private const float INTERVAL = .2f;

        private readonly static Type gameViewType = Type.GetType("UnityEditor.GameView,UnityEditor.dll");

        private string currentPath;
        private SerializedObject target;

        private static SerializedProperty curInfo;

        private static GroupVisibility[] groupVisibilities;

        private float clickedTime;
        private int clickedGroupIdx = -1, clickedInfoIdx = -1;
        private bool renaming = false;

        private Vector2 scrollPos;

        public static void Create()
        {
            var window = GetWindow<ClipConfigEditorWindow>(gameViewType);
            window.GetSO();
        }

        private void OnEnable()
        {
            GetSO();
        }

        private void OnSelectionChange()
        {
            GetSO();
        }

        private void OnProjectChange()
        {
            GetSO();
        }

        public void OnGUI()
        {
            if (target is null || !target.targetObject)
            {
                EditorGUILayout.HelpBox("你需要创建一个 ClipConfig 配置!", MessageType.Info);
                return;
            }

            #region Target

            EditorGUILayout.BeginHorizontal();

            var scriptName = target.FindProperty("scriptName");
            string newScriptName = EditorGUILayout.DelayedTextField("脚本名称", scriptName.stringValue);
            if (Regex.IsMatch(newScriptName, PATTERN_NAME)) scriptName.stringValue = newScriptName;
            else DialogUtils.Show("无效的名称", "脚本名称应当仅包含字母、数字和下划线！", isErr: false);

            if (GUILayout.Button("生成脚本")) GenerateCode(target);

            EditorGUILayout.EndHorizontal();

            #endregion

            EditorGUILayout.BeginHorizontal();

            #region Left Panel
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.MinWidth(scrollPos.x));

            var groups = target.FindProperty("groups");
            if (groupVisibilities == null)
            {
                groupVisibilities = new GroupVisibility[groups.arraySize];
                for (int i = 0; i < groups.arraySize; i++)
                    groupVisibilities[i].Infos = new bool[groups.GetArrayElementAtIndex(i)
                                                            .FindPropertyRelative("Infos")
                                                            .arraySize];
            }

            #region Group Header
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("音频组", EditorStyles.boldLabel);
            if (GUILayout.Button("添加组", GUILayout.Width(100)))
            {
                int idx = 1;
                var check = true;
                var groupName = "Group_0";
                while (check)
                {
                    check = false;
                    for (int i = 0; i < groups.arraySize; i++)
                        if (groupName == groups.GetArrayElementAtIndex(i).FindPropertyRelative("Name").stringValue)
                        {
                            groupName = Regex.Replace(groupName, PATTERN_SAME, "_" + idx++.ToString());
                            check = true;
                        }
                }

                groups.arraySize++;
                var group = groups.GetArrayElementAtIndex(groups.arraySize - 1);
                group.FindPropertyRelative("Name").stringValue = groupName;
                group.FindPropertyRelative("Infos").arraySize = 0;

                if (groupVisibilities.Length < groups.arraySize)
                {
                    var tmp = new GroupVisibility[groups.arraySize + 4];
                    groupVisibilities.CopyTo(tmp, 0);
                    groupVisibilities = tmp;
                }
            }
            EditorGUILayout.EndHorizontal();
            #endregion

            #region Group Body
            for (int i = 0; i < groups.arraySize; i++)
            {
                var group = groups.GetArrayElementAtIndex(i);
                var infos = group.FindPropertyRelative("Infos");

                #region Info Header

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                groupVisibilities[i].Showing = EditorGUILayout.Foldout(groupVisibilities[i].Showing, GUIContent.none);
                var groupName = group.FindPropertyRelative("Name");
                string newGroupName = EditorGUILayout.DelayedTextField(groupName.stringValue, EditorStyles.whiteLargeLabel);

                if (!Regex.IsMatch(newGroupName, PATTERN_NAME)) DialogUtils.Show("无效的名称", "组名应当仅包含字母、数字和下划线！", isErr: false);
                else
                {
                    var match = false;
                    for (int j = 0; j < groups.arraySize; j++)
                        if (i == j) continue;
                        else if (groups.GetArrayElementAtIndex(j).FindPropertyRelative("Name").stringValue == newGroupName)
                            match = true;
                    if (match) DialogUtils.Show("重复的名称", "名称出现重复，请检查！", isErr: false);
                    else groupName.stringValue = newGroupName;
                }

                bool add = false;
                if (GUILayout.Button("+", GUILayout.Width(25)))
                {
                    add = true;
                    groupVisibilities[i].Showing = true;
                }
                bool del = GUILayout.Button("删除", GUILayout.Width(100));

                EditorGUILayout.EndHorizontal();

                #endregion

                #region Info Body

                if (groupVisibilities[i].Showing)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(group.FindPropertyRelative("Bus"));
                    for (int j = 0; j < infos.arraySize; j++)
                    {
                        var info = infos.GetArrayElementAtIndex(j);
                        var infoName = info.FindPropertyRelative("Name");

                        // groupStates[i].Infos[j].Name = infoName.stringValue;

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Info" + j.ToString(), GUILayout.Width(150));
                        if (renaming && i == clickedGroupIdx && j == clickedInfoIdx)
                        {
                            string newInfoName = EditorGUILayout.DelayedTextField(infoName.stringValue);

                            if (!Regex.IsMatch(newInfoName, PATTERN_NAME)) DialogUtils.Show("无效的名称", "组名应当仅包含字母、数字和下划线！", isErr: false);
                            else
                            {
                                var match = false;
                                for (int k = 0; k < infos.arraySize; k++)
                                    if (j == k) continue;
                                    else if (infos.GetArrayElementAtIndex(k).FindPropertyRelative("Name").stringValue == newInfoName)
                                        match = true;
                                if (match) DialogUtils.Show("重复的名称", "名称出现重复，请检查！", isErr: false);
                            }

                            if (GUILayout.Button("Commit", GUILayout.Width(100)))
                            {
                                renaming = false;
                                infoName.stringValue = newInfoName;
                            }
                        }
                        else if (GUILayout.Button(infoName.stringValue))
                        {
                            renaming = false;
                            if (i == clickedGroupIdx && j == clickedInfoIdx)
                            {
                                if (Time.realtimeSinceStartup - clickedTime < INTERVAL) renaming = true;
                            }
                            else
                            {
                                clickedGroupIdx = i;
                                clickedInfoIdx = j;
                                clickedTime = Time.realtimeSinceStartup;
                            }
                            curInfo = info;
                        }
                        if (GUILayout.Button("-", GUILayout.Width(25)))
                        {
                            curInfo = null;
                            infos.DeleteArrayElementAtIndex(j);
                            // groupVars[i].Info.RemoveAt(j);
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
                    var check = true;
                    var infoName = "Info_0";
                    while (check)
                    {
                        check = false;
                        for (int j = 0; j < infos.arraySize; j++)
                            if (infoName == infos.GetArrayElementAtIndex(j).FindPropertyRelative("Name").stringValue)
                            {
                                infoName = Regex.Replace(infoName, PATTERN_SAME, "_" + idx++.ToString());
                                check = true;
                            }
                    }

                    infos.arraySize++;
                    infos.GetArrayElementAtIndex(infos.arraySize - 1).FindPropertyRelative("Name").stringValue = infoName;

                    if (groupVisibilities[i].Infos.Length < infos.arraySize)
                    {
                        var tmp = new bool[infos.arraySize + 4];
                        groupVisibilities[i].Infos.CopyTo(tmp, 0);
                        groupVisibilities[i].Infos = tmp;
                    }
                }
                if (del)
                {
                    curInfo = null;
                    groups.DeleteArrayElementAtIndex(i);
                }
                #endregion
            }
            #endregion

            EditorGUILayout.EndVertical();
            GUILayout.EndScrollView();
            #endregion

            #region Right Panel
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.MinWidth(position.width / 2 - 7.5f));

            if (curInfo is null) GUILayout.Label("配置", EditorStyles.boldLabel);
            else
            {
                GUILayout.Label(curInfo.FindPropertyRelative("Name").stringValue, EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(curInfo.FindPropertyRelative("Clips"));
                EditorGUILayout.PropertyField(curInfo.FindPropertyRelative("Loop"), new GUIContent("循环"));
                EditorGUILayout.PropertyField(curInfo.FindPropertyRelative("Pitch"), new GUIContent("音调"));
                EditorGUILayout.PropertyField(curInfo.FindPropertyRelative("Stereo"), new GUIContent("3D"));
            }

            EditorGUILayout.EndVertical();
            #endregion

            EditorGUILayout.EndHorizontal();
            GUILayout.Label(currentPath);

            target.ApplyModifiedProperties();
        }

        private struct GroupVisibility
        {
            public bool Showing;
            public bool[] Infos;
        }

        private bool GetSO()
        {
            foreach (var item in Selection.assetGUIDs.Union(AssetDatabase.FindAssets("t:ClipConfig")))
            {
                var path = AssetDatabase.GUIDToAssetPath(item);
                if (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path)
                                is ClipConfig clip)
                {
                    target = new SerializedObject(clip);
                    currentPath = path;
                    Repaint();
                    return true;
                }
            }
            target = null;
            return false;
        }

        public async static void GenerateCode(SerializedObject so)
        {
            #region Code Generate
            string scriptName = so.FindProperty("scriptName").stringValue;
            string codePath = "Assets/Resources/Audio/" + scriptName + ".cs";
            StringBuilder code = new();
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
