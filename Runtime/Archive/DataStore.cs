using System.Collections.Generic;
using System.Text;
using LitJson;
using UnityEngine;

namespace Bingyan
{
    /// <summary>
    /// 在存档读写时，自动读取/保存数据的存储结构的基类。
    /// <para>超级方便！！！</para>
    /// </summary>
    public abstract class DataStore
    {
        public delegate void DataEventTrigger();

        /// <summary>
        /// 数据从Data的缓存读取后触发
        /// </summary>
        public event DataEventTrigger Loaded;

        /// <summary>
        /// 数据即将写入Data的缓存时触发
        /// </summary>
        public event DataEventTrigger Saving;

        /// <value>
        /// 在 Archive 对应的键，在构造时指定
        /// </value>
        protected string key;

        public DataStore(string key)
        {
            this.key = key;

            Load();

            Archive.Loaded += Load;
            Archive.Saving += Save;
        }

        /// <summary>
        /// 关闭这个存储器，取消其自动读取/写入的功能
        /// </summary>
        public void Close()
        {
            Archive.Loaded -= Load;
            Archive.Saving -= Save;
        }

        protected virtual void Load()
        {
            Loaded?.Invoke();
        }
        protected virtual void Save()
        {
            Saving?.Invoke();
        }

        /// <summary>
        /// 打印存储的数据。通常用于Debug。
        /// </summary>
        public abstract void PrintContent();
    }

    /// <summary>
    /// 在存档读写时，自动读取/保存数据的列表。
    /// <para>数据以 <c>List{ T }</c> 存储</para>
    /// <para>方便得一批！！！</para>
    /// </summary>
    public class DataStoreList<T> : DataStore
    {
        protected List<T> list;
        public List<T> List => list;

        public DataStoreList(string key) : base(key) { }

        protected override void Save() { base.Save(); Archive.Set(key, JsonMapper.ToJson(list)); }
        protected override void Load()
        {
            list = JsonMapper.ToObject<List<T>>(Archive.Get(key, "")) ?? new List<T>();
            base.Load();
        }

        public override void PrintContent()
        {
            StringBuilder sb = new StringBuilder($"列表 {key}: 长度 {list.Count}\n");
            list.ForEach(i => sb.Append($"{i}\n"));
            Debug.Log(sb);
        }
    }

    /// <summary>
    /// 在存档读写时，自动读取/保存数据的字典。
    /// <para>注意：LitJson仅支持键类型为 <see cref="string"/> 的字典，故此处字典的键固定</para>
    /// <para>数据以 <c>Dictionary{ string, V }</c> 存储</para>
    /// <para>贼方便！！！</para>
    /// </summary>
    public class DataStoreDict<V> : DataStore
    {
        protected Dictionary<string, V> dict;
        public Dictionary<string, V> Dict => dict;
        public DataStoreDict(string key) : base(key) { }

        protected override void Save() { base.Save(); Archive.Set(key, JsonMapper.ToJson(dict)); }
        protected override void Load()
        {
            dict = JsonMapper.ToObject<Dictionary<string, V>>(Archive.Get(key, "")) ?? new Dictionary<string, V>();
            base.Load();
        }

        public override void PrintContent()
        {
            StringBuilder sb = new StringBuilder($"字典 {key}: 长度 {dict.Count}\n");
            foreach (var item in dict)
                sb.Append($"{item.Key}: {item.Value}\n");
            Debug.Log(sb.ToString());
        }
    }

    /// <summary>
    /// 在存档读写时，自动读取/保存数据的“字典列表”。
    /// <para>数据以 <c>Dictionary{ string, List{ V } }</c> 存储</para>
    /// <para>方便极了！！！</para>
    /// </summary>
    public class DataStoreDictList<V> : DataStoreDict<List<V>>
    {
        public DataStoreDictList(string key) : base(key) { }

        public override void PrintContent()
        {
            StringBuilder sb = new StringBuilder($"字典 {key}: 长度 {dict.Count}\n");
            foreach (var item in dict)
            {
                sb.Append($"{item.Key}: \n[\n");
                item.Value.ForEach(i => sb.Append($"{i},\n"));
                sb.Remove(sb.Length - 2, 2);
                sb.Append("\n]\n");
            }
            Debug.Log(sb.ToString());
        }
    }

    /// <summary>
    /// 在存档读写时，自动读取/保存数据的“值为字典的字典”。
    /// <para>数据以 <c>Dictionary{ string, Dictionary{ string, V } }</c> 存储</para>
    /// <para>有点奇怪，但是某些情况下出奇的好用</para>
    /// </summary>
    public class DataStoreDictDict<V> : DataStoreDict<Dictionary<string, V>>
    {
        public DataStoreDictDict(string key) : base(key) { }

        public override void PrintContent()
        {
            StringBuilder sb = new StringBuilder($"字典 {key}: 长度 {dict.Count}\n");
            foreach (var item in dict)
            {
                sb.Append($"{item.Key}: \n[\n");
                foreach (var kv in item.Value) sb.Append($"{kv.Key}: {kv.Value},\n");
                sb.Remove(sb.Length - 2, 2);
                sb.Append("\n]\n");
            }
            Debug.Log(sb.ToString());
        }
    }
}