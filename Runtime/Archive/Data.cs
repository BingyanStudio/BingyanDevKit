using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using LitJson;

namespace Bingyan
{
    /// <summary>
    /// <para>存档操纵类，用于缓存数据、读取/写入存档</para>
    /// <para>缓存是一个 Dictionary&lt;string,object&gt;</para>
    /// <para>数据的流动大致如下：</para>
    /// <para>储存时: 外界 → 缓存 → 存档</para>
    /// <para>读取时：存档 → 缓存 → 外界</para>
    /// </summary>
    public static class Data
    {
        public delegate void DataEventHandler();

        /// <summary>
        /// 在缓存数据即将被写入存档时触发
        /// <para>可以参考一下 DataStore 家族的写法</para>
        /// 另请参阅: <see cref = "Bingyan.DataStore"/>
        /// </summary>
        public static event DataEventHandler Saving;

        /// <summary>
        /// 在缓存数据被写入存档后触发
        /// </summary>
        public static event DataEventHandler Saved;

        /// <summary>
        /// 在存档被加载进缓存后触发
        /// </summary>
        public static event DataEventHandler Loaded;

        /// <summary>
        /// 缓存字典，是外界访问与存档之间的桥梁。
        /// </summary>
        /// <typeparam name="string">键</typeparam>
        /// <typeparam name="object">值</typeparam>
        private static Dictionary<string, object> datas = new Dictionary<string, object>();

        // 延迟更新的数据，直到下一次读档才会被应用
        private static Dictionary<string, object> delayedData = new Dictionary<string, object>();

        /// <summary>
        /// 加载存档，并将字典载入到Data缓存中，由 <c>Get&lt;T&gt;</c> 读取缓存数据, <c>Set&lt;T&gt;</c>写入
        /// </summary>
        /// <param name="saveIndex">存档序号</param>
        public static void LoadToGame(int saveIndex)
        {
            datas = Load(saveIndex);
            if (delayedData.Count > 0)
            {
                foreach (var item in delayedData)
                {
                    if (!datas.ContainsKey(item.Key)) datas.Add(item.Key, item.Value);
                    else datas[item.Key] = item.Value;
                }
                delayedData.Clear();
            }
            Loaded?.Invoke();

            PrintContent();
        }

        /// <summary>
        /// 加载存档并获取存档字典。这个方法允许你不将存档载入进缓存，仅读取存档内容
        /// </summary>
        /// <param name="saveIndex">存档序号</param>
        /// <returns>存档保存的所有键值对组成的字典</returns>
        public static Dictionary<string, object> Load(int saveIndex)
        {
            var path = GetSaveFilePath(saveIndex);
            Print($"正在加载存档 {saveIndex}\n路径: {path}\n\n");

            StreamReader reader;
            if (!Directory.Exists(GetDirectory())) Directory.CreateDirectory(GetDirectory());
            if (!File.Exists(path)) reader = new StreamReader(File.Create(path));
            else reader = new StreamReader(path, Encoding.UTF8);

            Dictionary<string, object> dict = new Dictionary<string, object>();
            var content = reader.ReadToEnd();

            dict = JsonMapper.ToObject<Dictionary<string, object>>(content);
            reader.Close();

            dict ??= new Dictionary<string, object>();
            Print($"存档已加载!\n点击查看内容 \n\n{content}\n\n");
            return dict;
        }

        /// <summary>
        /// 将缓存数据保存至指定存档
        /// </summary>
        /// <param name="saveIndex">存档序号</param>
        public static void Save(int saveIndex) => Save(saveIndex, new Dictionary<string, object>());

        /// <summary>
        /// 将缓存数据保存至指定存档
        /// </summary>
        /// <param name="saveIndex">存档序号</param>
        /// <param name="extras">额外的数据。会在 <see cref="Saving"/> 执行之后才会写入</param>
        public static void Save(int saveIndex, Dictionary<string, object> extras)
        {
            var path = GetSaveFilePath(saveIndex);
            Print($"正在保存存档 {saveIndex}\n路径: {path}\n\n");
            Saving?.Invoke();

            foreach (var item in extras)
                Set(item.Key, item.Value);

            PrintContent();
            StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8);
            string content = JsonMapper.ToJson(datas);
            writer.Write(content);
            writer.Flush();
            writer.Close();

            Saved?.Invoke();
            Print($"存档已保存! \n点击查看内容 \n{content}\n\n");
        }

        /// <summary>
        /// 清除指定存档的数据
        /// </summary>
        /// <param name="saveIndex">存档序号</param>
        public static void Clear(int saveIndex)
        {
            var path = GetSaveFilePath(saveIndex);
            StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8);
            string content = "";
            writer.Write(content);
            writer.Flush();
            writer.Close();

            Print($"存档 {saveIndex} 已清空!");
        }

        /// <summary>
        /// 检测指定序号的存档是否曾经被存储过
        /// </summary>
        /// <param name="saveIndex">存档序号</param>
        /// <returns>存档是否曾经被存储过</returns>
        public static bool HasSave(int saveIndex)
            => Directory.Exists(GetDirectory()) && File.Exists(GetSaveFilePath(saveIndex));


        /// <summary>
        /// 检测缓存区字典中是否包含键为 key 的键值对
        /// </summary>
        /// <param name="key">指定的键</param>
        /// <returns>缓存是否含有这个键</returns>
        public static bool Has(string key) => datas.ContainsKey(key);

        /// <summary>
        /// 读取缓存区的指定数据
        /// </summary>
        /// <param name="key">指定的键</param>
        /// <param name="defaultVal">默认的返回值, 当键不存在时返回</param>
        /// <typeparam name="T">任意类型</typeparam>
        /// <returns>读取的返回值</returns>
        public static T Get<T>(string key, T defaultVal)
        {
            if (!datas.ContainsKey(key)) datas.Add(key, defaultVal);
            return (T)datas[key];
        }

        /// <summary>
        /// 向缓存延迟设置键值。数据将会延迟到下一次调用 <c>LoadToGame</c> 时加入到缓存区。换句话说，在此设置的数据会加入到下一次读档后的缓存中。
        /// <para>（呃，老实说，如果游戏的其他部分设计得科学合理的话，应该不会用到这个方法）</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        public static void SetDelayed<T>(string key, T value)
        {
            if (delayedData.ContainsKey(key)) delayedData[key] = value;
            else delayedData.Add(key, value);
        }

        /// <summary>
        /// 向缓存设置键值。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <typeparam name="T">任意类型</typeparam>
        public static void Set<T>(string key, T value)
        {
            if (datas.ContainsKey(key)) datas[key] = value;
            else datas.Add(key, value);
        }

        /// <summary>
        /// 删除缓存中的指定键值对。
        /// </summary>
        /// <param name="key">指定的键</param>
        public static void Remove(string key)
        {
            if (datas.ContainsKey(key)) datas.Remove(key);
        }

        /// <summary>
        /// 打印目前缓存中的内容。通常用于 Debug。
        /// </summary>
        public static void PrintContent()
        {
            var sb = new StringBuilder("Data数据列表\n");
            sb.AppendLine("点击查看内容");
            sb.AppendLine();

            foreach (var item in datas)
            {
                sb.AppendLine($"{item.Key}: {item.Value}");
            }
            sb.AppendLine("========");
            sb.AppendLine();
            Debug.Log(sb);
        }

        /// <summary>
        /// 获取存档的文件夹目录。
        /// </summary>
        /// <returns>存档的文件夹目录</returns>
        private static string GetDirectory() => $"{Application.persistentDataPath}/Save";

        /// <summary>
        /// 读取指定序号的存档的存储路径。
        /// </summary>
        /// <param name="index">指定序号</param>
        /// <returns>指定序号的存档的存储路径</returns>
        private static string GetSaveFilePath(int index) => $"{GetDirectory()}/save_{index}.json";

        /// <summary>
        /// 简化 Debug.Log 用的
        /// </summary>
        /// <param name="msg">信息</param>
        private static void Print(string msg) => Debug.Log(msg);
    }
}