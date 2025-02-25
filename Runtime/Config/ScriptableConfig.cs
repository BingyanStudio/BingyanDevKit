using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Bingyan
{
    public abstract class ScriptableConfig : ScriptableObject { }

    public abstract class ScriptableConfig<T> : ScriptableConfig where T : ScriptableConfig<T>
    {
        public static T Instance
        {
            get
            {
                if (!instance)
                {
                    var all = Resources.FindObjectsOfTypeAll<T>();

#if UNITY_EDITOR
                    if (all.Length == 0)
                    {
                        Log.I("ScriptableConfig", "未找到已加载的资源，尝试从项目中加载...");
                        var paths = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
                        if (paths.Length > 0)
                            all = new T[] { AssetDatabase.LoadAssetAtPath<T>(paths[0]) };
                    }
#endif

                    if (all.Length == 0) Log.E("ScriptableConfig", "未找到配置，请至少创建一个!");
                    else instance = all[0];
                }
                return instance;
            }
        }
        private static T instance;
    }
}