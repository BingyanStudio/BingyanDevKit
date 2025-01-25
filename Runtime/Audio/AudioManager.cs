using System.Collections.Generic;
using UnityEngine;

namespace Bingyan
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        static AudioManager()
        {
            Instance = new GameObject().AddComponent<AudioManager>();
            DontDestroyOnLoad(Instance.gameObject);

            Instance.Config = Resources.Load<ClipConfig>("Audio/ClipMap").Init();
            Instance.infos = new();
        }

        public ClipConfig Config;
        private class SourceInfo
        {
            public string Name;
            public GameObject Target;
            public AudioSource Source;
            public float TimeSamples;
            public ClipInfo Clip;
        }
        private List<SourceInfo> infos;
        /// <summary>
        /// 播放<paramref name="player"/>，且保证只有一个AudioSource播放它
        /// </summary>
        public AudioSource PlaySingle(ClipPlayer player) => Play(player, gameObject, true);
        /// <summary>
        /// 跟踪<paramref name="target"/>播放<paramref name="player"/>，且保证只有一个AudioSource播放它
        /// </summary>
        public AudioSource PlaySingle(ClipPlayer player, GameObject target) => Play(player, target, true);
        /// <summary>
        /// 播放<paramref name="player"/>
        /// </summary>
        public AudioSource Play(ClipPlayer player) => Play(player, gameObject, false);
        /// <summary>
        /// 跟踪<paramref name="target"/>播放<paramref name="player"/>
        /// </summary>
        public AudioSource Play(ClipPlayer player, GameObject target) => Play(player, target, false);
        private AudioSource Play(ClipPlayer player, GameObject target, bool single)
        {
            SourceInfo curInfo = null;
            foreach (var info in infos)
            {
                if (single && info.Name == player.Name && info.Source.isPlaying)
                {
                    info.Target = target;
                    return info.Source;
                }
                if (!info.Source.isPlaying)
                {
                    curInfo = info;
                    if (!single) break;
                }
            }
            if (curInfo == null)
            {
                curInfo = new SourceInfo();
                curInfo.Source = new GameObject().AddComponent<AudioSource>();
                curInfo.Source.transform.parent = transform;
                infos.Add(curInfo);
            }
            curInfo.Name = player.Name;
            curInfo.Target = target;
            curInfo.Source.clip = player.Info.Clips.Length > 0 ? player.Info.Clips[UnityEngine.Random.Range(0, player.Info.Clips.Length)] : null;
            curInfo.Source.loop = player.Info.Loop;
            curInfo.Source.pitch = 1 + player.Info.Pitch;
            curInfo.Source.outputAudioMixerGroup = player.Info.Bus;
            curInfo.Source.spatialBlend = player.Info.Stereo;
            curInfo.TimeSamples = 0;
            curInfo.Clip = player.Info;
            curInfo.Source.Play();
            return curInfo.Source;
        }
        /// <summary>
        /// 停止播放<paramref name="player"/>
        /// </summary>
        public void Stop(ClipPlayer player)
        {
            foreach (var info in infos)
                if (info.Target == gameObject && info.Name == player.Name)
                    info.Source.Stop();
        }
        /// <summary>
        /// 停止播放正在跟踪<paramref name="target"/>的<paramref name="player"/>
        /// </summary>
        public void Stop(ClipPlayer player, GameObject target)
        {
            foreach (var info in infos)
                if (info.Target == target && info.Name == player.Name)
                    info.Source.Stop();
        }
        private void FixedUpdate()
        {
            foreach (var info in infos)
                if (info.Target != null && info.Source.isPlaying)
                {
                    if (info.Source.timeSamples < info.TimeSamples)
                        info.Source.pitch = 1 + info.Clip.Pitch;
                    info.TimeSamples = info.Source.timeSamples;
                    info.Source.transform.position = info.Target.transform.position;
                }
        }
    }
}