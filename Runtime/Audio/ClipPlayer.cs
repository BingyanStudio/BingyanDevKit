using UnityEngine;

namespace Bingyan
{
    public class ClipPlayer
    {
        public readonly ClipInfo Info;
        public readonly string Name;
        public ClipPlayer(string name)
        {
            Info = AudioManager.Instance.Config[name];
            Name = name;
        }
        public static implicit operator string(ClipPlayer player) => player.Name;
        /// <summary>
        /// 使用<see cref="AudioManager"/>播放<see cref="Info"/>
        /// </summary>
        public AudioSource Play() => AudioManager.Instance.Play(Info);
        /// <summary>
        /// 使用<see cref="AudioManager"/>播放<see cref="Info"/>，且保证只有一个AudioSource播放它
        /// </summary>
        public AudioSource PlaySingle() => AudioManager.Instance.PlaySingle(Info);
        /// <summary>
        /// 使用<see cref="AudioManager"/>播放<see cref="Info"/>，跟踪<paramref name="target"/>
        /// </summary>
        public AudioSource Play(GameObject target) => AudioManager.Instance.Play(Info, target);
        /// <summary>
        /// 使用<see cref="AudioManager"/>播放<see cref="Info"/>，跟踪<paramref name="target"/>，且保证只有一个AudioSource播放它
        /// </summary>
        public AudioSource PlaySingle(GameObject target) => AudioManager.Instance.PlaySingle(Info, target);
    }

}