using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Bingyan.Editor
{
    public class ScriptableConfigBuildPipeline : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        private HashSet<Type> addedTypes;
        private HashSet<(Type, string)> multipleWarnings;

        public void OnPreprocessBuild(BuildReport report)
        {
            var preloads = new List<UnityEngine.Object>(PlayerSettings.GetPreloadedAssets());
            addedTypes = new();
            multipleWarnings = new();

            foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(i => i.GetTypes())
                .Where(i => i.IsSubclassOf(typeof(ScriptableConfig)) && !i.IsAbstract))
            {
                var assets = AssetDatabase.FindAssets($"t:{type.Name}");
                if (assets.Length == 0) continue;

                var path = AssetDatabase.GUIDToAssetPath(assets[0]);

                addedTypes.Add(type);
                if (assets.Length > 1) multipleWarnings.Add((type, path));

                preloads.Add(AssetDatabase.LoadAssetAtPath(path, type));
            }
            PlayerSettings.SetPreloadedAssets(preloads.ToArray());
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            if (addedTypes.Count > 0)
            {
                var sb = new System.Text.StringBuilder("导出的全局配置: ");
                foreach (var type in addedTypes) sb.AppendLine(type.Name);
                Log.I("ScriptableConfig", sb);

                foreach (var item in multipleWarnings)
                    Log.W("ScriptableConfig", $"配置 {item.Item1} 存在多个\n已采用{item.Item2}");

                var preloads = new List<UnityEngine.Object>(PlayerSettings.GetPreloadedAssets());
                preloads.RemoveAll(i => addedTypes.Contains(i.GetType()));
                PlayerSettings.SetPreloadedAssets(preloads.ToArray());
            }
        }
    }
}