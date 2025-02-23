using System;
using UnityEngine;

namespace Bingyan
{
    [Serializable]
    public struct AudioRef
    {
        [SerializeField] internal string Name;

        public AudioRef(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 停止播放<see cref="Info"/>
        /// </summary>
        public readonly void Stop() => AudioManager.Instance.Stop(this);

        /// <summary>
        /// 停止播放正在追踪<paramref name="target"/>的<see cref="Info"/>
        /// </summary>
        public readonly void Stop(GameObject target) => AudioManager.Instance.Stop(this, target);

        /// <summary>
        /// 使用<see cref="AudioManager"/>播放<see cref="Info"/>
        /// </summary>
        public readonly AudioSource Play(Vector3 pos = default) => AudioManager.Instance.Play(this, pos);

        /// <summary>
        /// 使用<see cref="AudioManager"/>播放<see cref="Info"/>，跟踪<paramref name="target"/>
        /// </summary>
        public readonly AudioSource Play(GameObject target) => AudioManager.Instance.Play(this, target);

        /// <summary>
        /// 使用<see cref="AudioManager"/>播放<see cref="Info"/>，且保证只有一个AudioSource播放它
        /// </summary>
        public readonly AudioSource PlaySingleton(Vector3 pos = default) => AudioManager.Instance.PlaySingleton(this, pos);

        /// <summary>
        /// 使用<see cref="AudioManager"/>播放<see cref="Info"/>，跟踪<paramref name="target"/>，且保证只有一个AudioSource播放它
        /// </summary>
        public readonly AudioSource PlaySingleton(GameObject target) => AudioManager.Instance.PlaySingleton(this, target);

        public static implicit operator bool(AudioRef audioRef) => audioRef.Name.Length > 0;
    }
}