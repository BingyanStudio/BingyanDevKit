using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using LitJson;

namespace Bingyan
{
    /// <summary>
    /// DevKit的设置文件
    /// </summary>
    [FilePath("ProjectSettings/BingyanDevKit.asset", FilePathAttribute.Location.ProjectFolder)]
    public class DevKitSetting : ScriptableSingleton<DevKitSetting>
    {
        [HideInInspector, SerializeField] private string ids_json = "";

        /// <summary>
        /// 保存的自定义id列表。
        /// <para>注意，每一次调用他都会生成一个新的字典。你需要先整一个临时变量存着他，然后对临时变量操作。</para>
        /// <para>要想保存，请参考 <see cref="StringIDDrawer"/> 在 GUI.Button("确定") 下方的代码</para>
        /// </summary>
        public Dictionary<string, List<string>> Ids
            => JsonMapper.ToObject<Dictionary<string, List<string>>>(ids_json) ?? new Dictionary<string, List<string>>();

        /// <summary>
        /// 依据id组的key获取id列表
        /// <para>注意，改变他并不能直接保存到设置中。</para>
        /// </summary>
        /// <param name="idGroupKey">id组的识别字符串</param>
        /// <returns>id列表</returns>
        public static List<string> GetIds(string idGroupKey)
        {
            if (idGroupKey == string.Empty)
            {
                Debug.LogError("错误: 传递了空的IdGroupKey！");
                return new List<string>();
            }
            var dict = instance.Ids;
            if (!dict.ContainsKey(idGroupKey)) dict.Add(idGroupKey, new List<string>());
            return dict[idGroupKey];
        }

        /// <summary>
        /// 将保存临时变量的对应的字典序列化成Json字符串
        /// </summary>
        /// <param name="dict">字典，可以从 <see cref="Ids"/> 获取，更改后使用这个序列化。</param>
        /// <returns>序列化后的字符串。应当给予StoryTellerSetting.SerConfig的ids_json属性</returns>
        public static string SetIdsDictToJson(Dictionary<string, List<string>> dict) => JsonMapper.ToJson(dict);

        /// <summary>
        /// 对保存临时变量的对应的字典的某个变量组进行修改，并序列化成Json字符串
        /// </summary>
        /// <param name="groupKey">变量组名称</param>
        /// <param name="list">修改后的列表</param>
        /// <returns>序列化后的字符串。应当给予StoryTellerSetting.SerConfig的ids_json属性</returns>
        public string AddIdListToJson(string groupKey, List<string> list)
        {
            var dict = Ids;
            if (!Ids.ContainsKey(groupKey)) dict.Add(groupKey, list);
            else dict[groupKey] = list;
            return SetIdsDictToJson(dict);
        }

        /// <summary>
        /// 获取对应的 SerializedObject
        /// </summary>
        public SerializedObject GetSerializedObject() => new SerializedObject(this);

        private void OnDisable()
        {
            Save(true);
        }

        // [Serializable]
        // private class IdDict
        // {
        //     private
        // }
    }
}