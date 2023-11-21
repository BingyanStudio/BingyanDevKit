using UnityEngine;
using System;
using System.Collections.Generic;

namespace Bingyan
{
    /// <summary>
    /// 用于对外提供音频播放接口的类型，方便策划操作
    /// <para>注意: 如果要改变他在Inspector中的标签，请使用<see cref="UnityEngine.TooltipAttribute"/>而不是<see cref="TitleAttribute"/></para>
    /// </summary>
    [Serializable]
    public class AudioCallback
    {
        [SerializeField] private List<AudioCallbackElement> elements;

        /// <summary>
        /// 调用这一接口拥有的所有音频回调
        /// </summary>
        public void Invoke()
        {
            elements.ForEach(i => i.Invoke());
        }

        /// <summary>
        /// 停止播放所有配置到该回调的音源
        /// </summary>
        public void Stop()
        {
            elements.ForEach(i => i.Stop());
        }

        /// <summary>
        /// 添加一个音频回调
        /// </summary>
        /// <param name="source">音源</param>
        /// <param name="type">执行的方式</param>
        public void Add(AudioSource source, PlayType type)
        {
            var match = elements.Find(i => i.Source == source);
            if (match != null) match.Type = type;
            else elements.Add(new AudioCallbackElement(source, type));
        }

        /// <summary>
        /// 移除一个音频回调
        /// </summary>
        /// <param name="source">音源</param>
        public void Remove(AudioSource source)
        {
            for (int i = elements.Count - 1; i >= 0; i--)
                if (elements[i].Source == source)
                {
                    elements.RemoveAt(i);
                    return;
                }
        }

        [Serializable]
        public class AudioCallbackElement
        {
            [SerializeField] private AudioSource source;
            [SerializeField] private PlayType type;

            public AudioSource Source => source;
            public PlayType Type { get => type; set => type = value; }

            public AudioCallbackElement(AudioSource source, PlayType type)
            {
                this.source = source;
                this.type = type;
            }

            public void Invoke()
            {
                switch (type)
                {
                    case PlayType.Play:
                        source.loop = false;
                        source.Play();
                        break;

                    case PlayType.Loop:
                        source.loop = true;
                        source.Play();
                        break;

                    case PlayType.Stop:
                        source.Stop();
                        break;
                }
            }

            public void Stop()
            {
                source.Stop();
            }
        }

        public enum PlayType
        {
            [InspectorName("播放一次")] Play,
            [InspectorName("循环播放")] Loop,
            [InspectorName("停止")] Stop
        }
    }
}