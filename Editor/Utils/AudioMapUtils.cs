using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;

namespace Bingyan.Editor
{
    public static class AudioMapUtils
    {
        public static bool TryGetSO(out SerializedObject target, out string path)
        {
            foreach (var item in Selection.assetGUIDs.Union(AssetDatabase.FindAssets("t:AudioMapConfig")))
            {
                path = AssetDatabase.GUIDToAssetPath(item);
                if (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path)
                                is AudioMapConfig clip)
                {
                    target = new SerializedObject(clip);
                    return true;
                }
            }
            path = string.Empty;
            target = null;
            return false;
        }

        public async static void GenerateCode(SerializedObject audioMap)
        {
            #region Code Generate

            var scriptPath = audioMap.FindProperty("scriptPath").stringValue;
            var className = Path.GetFileNameWithoutExtension(scriptPath).Split('.')[0];

            StringBuilder code = new();
            code.Append("public static class ").Append(className).Append("\n{");
            var groups = audioMap.FindProperty("groups");
            for (int i = 0; i < groups.arraySize; i++)
            {
                var group = groups.GetArrayElementAtIndex(i);
                string groupName = group.FindPropertyRelative("Name").stringValue;
                code.Append("\n\tpublic static class ").Append(groupName).Append("\n\t{");
                var infos = group.FindPropertyRelative("Infos");
                for (int j = 0; j < infos.arraySize; j++)
                {
                    string infoName = infos.GetArrayElementAtIndex(j).FindPropertyRelative("Name").stringValue;
                    code.Append("\n\t\tpublic static readonly AudioPlayer ").Append(infoName).Append(" = new(\"").Append(groupName).Append("/").Append(infoName).Append("\");");
                }
                code.Append("\n\t}");
            }
            code.Append("\n}");
            await File.WriteAllTextAsync(scriptPath, code.ToString());
            AssetDatabase.ImportAsset(scriptPath);
            #endregion
        }
    }
}