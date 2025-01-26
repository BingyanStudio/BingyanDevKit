using UnityEngine;

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
                    if (all.Length == 0) Log.E("ScriptableConfig", "未找到配置，请至少创建一个!");
                    else instance = all[0];
                }
                return instance;
            }
        }
        private static T instance;
    }
}