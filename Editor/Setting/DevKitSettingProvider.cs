using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace Bingyan.Editor
{
    /// <summary>
    /// DevKit 的设置UI绘制器。UI 可以在 Edit -> Project Settings 里找到。
    /// </summary>
    public static class DevKitSettingProvider
    {
        private static readonly Dictionary<string, bool> idListFolds = new();

        [SettingsProvider]
        public static SettingsProvider Configue()
            => new("Project/Bingyan/DevKit", SettingsScope.Project)
            {
                label = "DevKit",
                guiHandler = s =>
                {
                    var ids = DevKitSetting.instance.Ids;
                    var serConfig = DevKitSetting.instance.GetSerializedObject();
                    var idsProp = serConfig.FindProperty("ids_json");
                    LabelField("ID列表", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    foreach (var item in ids)
                    {
                        if (!idListFolds.ContainsKey(item.Key)) idListFolds.Add(item.Key, false);
                        idListFolds[item.Key] = BeginFoldoutHeaderGroup(idListFolds[item.Key], $"{item.Key}");
                        if (idListFolds[item.Key])
                        {
                            item.Value.Sort((l, r) => l[0] - r[0]);
                            foreach (var str in item.Value)
                            {
                                BeginHorizontal();
                                LabelField(str);
                                if (GUILayout.Button("删除", GUILayout.Width(100)))
                                {
                                    if (DialogUtils.Show("确认?", $"你确定要删除{item.Key}组的{str}吗?", isErr: false))
                                    {
                                        ids[item.Key].Remove(str);
                                        idsProp.stringValue = DevKitSetting.SetIdsDictToJson(ids);
                                        break;
                                    }
                                }
                                EndHorizontal();
                            }
                            Separator();
                        }
                        EndFoldoutHeaderGroup();
                    }
                    EditorGUI.indentLevel--;
                }
            };
    }
}