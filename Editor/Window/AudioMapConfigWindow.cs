using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Bingyan.Editor
{
    [EditorWindowTitle(title = "音频配置")]
    public class AudioMapConfigWindow : EditorWindow
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

        private string search = string.Empty;

        private Vector2 scrollPos;

        [MenuItem("DevKit/音频配置")]
        public static void Create()
        {
            var window = GetWindow<AudioMapConfigWindow>(gameViewType);
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
                EditorGUILayout.HelpBox("你需要创建一个 AudioMap 配置!", MessageType.Info);
                if (GUILayout.Button("创建", GUILayout.Height(30)))
                {
                    var path = EditorUtility.SaveFilePanelInProject("创建 AudioMap", "AudioMap", "asset", "输入 AudioMap 配置的文件名");
                    AssetDatabase.CreateAsset(CreateInstance<AudioMapConfig>(), path);
                    AssetDatabase.Refresh();
                }
                return;
            }

            #region Target

            EditorGUILayout.BeginHorizontal();

            var scPath = target.FindProperty("scriptPath");
            EditorGUILayout.LabelField("生成脚本", GUILayout.Width(60));

            GUI.enabled = false;
            EditorGUILayout.TextField(scPath.stringValue);
            GUI.enabled = true;

            if (GUILayout.Button("选择路径", GUILayout.Width(80)))
            {
                var path = EditorUtility.SaveFilePanelInProject("选择代码生成路径", "AudioMap.Generated", "cs", "请选择生成代码的路径");
                if (path.Length > 0) scPath.stringValue = path;
            }
            if (GUILayout.Button("生成", GUILayout.Width(80))) AudioMapUtils.GenerateCode(target);

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
                {
                    groupVisibilities[i].Showing = true;
                    groupVisibilities[i].Infos = new bool[groups.GetArrayElementAtIndex(i)
                                                                            .FindPropertyRelative("Infos")
                                                                            .arraySize];
                    Array.Fill(groupVisibilities[i].Infos, true);
                }
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

            search = EditorGUILayout.TextField("搜索", search);
            #endregion

            #region Group Body
            for (int i = 0; i < groups.arraySize; i++)
            {
                var group = groups.GetArrayElementAtIndex(i);
                var infos = group.FindPropertyRelative("Infos");

                if (!group.FindPropertyRelative("Name").stringValue.Contains(search, StringComparison.OrdinalIgnoreCase)) continue;

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


                bool del = GUILayout.Button("删除组", GUILayout.Width(100));
                var groupToBeDel = groupName.stringValue;

                EditorGUILayout.EndHorizontal();

                #endregion

                #region Info Body

                bool addInfo = false;
                if (groupVisibilities[i].Showing)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(group.FindPropertyRelative("Bus"), new GUIContent("总线"));
                    GUILayout.Space(10);

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("+", GUILayout.Width(25)))
                    {
                        addInfo = true;
                        groupVisibilities[i].Showing = true;
                    }
                    EditorGUILayout.EndHorizontal();

                    for (int j = 0; j < infos.arraySize; j++)
                    {
                        var info = infos.GetArrayElementAtIndex(j);
                        var infoName = info.FindPropertyRelative("Name");

                        EditorGUILayout.BeginHorizontal();
                        // EditorGUILayout.LabelField("Info" + j.ToString(), GUILayout.Width(150));
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
                                else infoName.stringValue = newInfoName;
                            }

                            if (GUILayout.Button("确认", GUILayout.Width(100)))
                            {
                                renaming = false;

                                clickedGroupIdx = -1;
                                clickedInfoIdx = -1;
                            }
                        }
                        else if (GUILayout.Button(infoName.stringValue))
                        {
                            renaming = false;
                            if (i == clickedGroupIdx && j == clickedInfoIdx)
                            {
                                if (Time.realtimeSinceStartup - clickedTime < INTERVAL) renaming = true;
                                else clickedTime = Time.realtimeSinceStartup;
                            }
                            else
                            {
                                clickedGroupIdx = i;
                                clickedInfoIdx = j;
                                clickedTime = Time.realtimeSinceStartup;
                            }
                            curInfo = info;
                        }

                        if (GUILayout.Button("-", GUILayout.Width(25))
                            && DialogUtils.Show("删除音频", $"确定要删除音频 {infoName.stringValue} 吗？", isErr: false))
                        {
                            curInfo = null;
                            infos.DeleteArrayElementAtIndex(j);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUI.indentLevel--;
                }
                #endregion

                #region Late Update
                if (addInfo)
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

                    groupVisibilities[i].Infos ??= new bool[infos.arraySize];
                    if (groupVisibilities[i].Infos.Length < infos.arraySize)
                    {
                        var tmp = new bool[infos.arraySize + 4];
                        groupVisibilities[i].Infos.CopyTo(tmp, 0);
                        groupVisibilities[i].Infos = tmp;
                    }
                }
                if (del && DialogUtils.Show("删除音频", $"确定要删除音频组 {groupToBeDel} 吗？", isErr: false))
                {
                    curInfo = null;
                    groups.DeleteArrayElementAtIndex(i);
                }
                #endregion

                if (i < groups.arraySize - 1) EditorGUILayout.Space(20);
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
                EditorGUILayout.PropertyField(curInfo.FindPropertyRelative("Range"), new GUIContent("范围"));
                EditorGUILayout.PropertyField(curInfo.FindPropertyRelative("Pitch"), new GUIContent("音调偏移"));
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
            if (AudioMapUtils.TryGetSO(out target, out currentPath))
            {
                Repaint();
                return true;
            }
            return false;
        }
    }
}
