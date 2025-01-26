using UnityEngine;

namespace Bingyan
{
    public class ClipPlayer
    {
        internal readonly string Name;

        public ClipPlayer(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 停止播放<see cref="Info"/>
        /// </summary>
        public void Stop() => AudioManager.Instance.Stop(this);

        /// <summary>
        /// 使用<see cref="AudioManager"/>播放<see cref="Info"/>
        /// </summary>
        public AudioSource Play() => AudioManager.Instance.Play(this);

        /// <summary>
        /// 使用<see cref="AudioManager"/>播放<see cref="Info"/>，且保证只有一个AudioSource播放它
        /// </summary>
        public AudioSource PlaySingleton() => AudioManager.Instance.PlaySingleton(this);

        /// <summary>
        /// 停止播放正在追踪<paramref name="target"/>的<see cref="Info"/>
        /// </summary>
        public void Stop(GameObject target) => AudioManager.Instance.Stop(this, target);

        /// <summary>
        /// 使用<see cref="AudioManager"/>播放<see cref="Info"/>，跟踪<paramref name="target"/>
        /// </summary>
        public AudioSource Play(GameObject target) => AudioManager.Instance.Play(this, target);

        /// <summary>
        /// 使用<see cref="AudioManager"/>播放<see cref="Info"/>，跟踪<paramref name="target"/>，且保证只有一个AudioSource播放它
        /// </summary>
        public AudioSource PlaySingleton(GameObject target) => AudioManager.Instance.PlaySingleton(this, target);
    }

}