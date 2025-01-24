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
        }
        private List<SourceInfo> infos;
        /// <summary>
        /// 播放<paramref name="clip"/>
        /// </summary>
        public AudioSource Play(AudioClip clip) => Play(new ClipInfo() { Clips = new[] { clip } });
        /// <summary>
        /// 跟踪<paramref name="player"/>播放<paramref name="clip"/>
        /// </summary>
        public AudioSource Play(AudioClip clip, GameObject player) => Play(new ClipInfo() { Clips = new[] { clip } }, player);
        /// <summary>
        /// 播放<paramref name="clip"/>，且保证只有一个AudioSource播放它
        /// </summary>
        public AudioSource PlaySingle(ClipInfo clip) => Play(clip, gameObject, true);
        /// <summary>
        /// 跟踪<paramref name="player"/>播放<paramref name="clip"/>，且保证只有一个AudioSource播放它
        /// </summary>
        public AudioSource PlaySingle(ClipInfo clip, GameObject player) => Play(clip, player, true);
        /// <summary>
        /// 播放<paramref name="clip"/>
        /// </summary>
        public AudioSource Play(ClipInfo clip) => Play(clip, gameObject, false);
        /// <summary>
        /// 跟踪<paramref name="player"/>播放<paramref name="clip"/>
        /// </summary>
        public AudioSource Play(ClipInfo clip, GameObject player) => Play(clip, player, false);
        private AudioSource Play(ClipInfo clip, GameObject player, bool single)
        {
            SourceInfo curInfo = null;
            foreach (var info in infos)
            {
                if (single && info.Name == clip.Name && info.Source.isPlaying)
                {
                    info.Target = player;
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
            curInfo.Name = clip.Name;
            curInfo.Target = player;
            curInfo.Source.clip = clip.Clips.Length > 0 ? clip.Clips[UnityEngine.Random.Range(0, clip.Clips.Length)] : null;
            curInfo.Source.loop = clip.Loop;
            curInfo.Source.pitch = 1 + clip.Pitch;
            curInfo.Source.outputAudioMixerGroup = clip.Bus;
            curInfo.Source.spatialBlend = clip.Stereo;
            curInfo.TimeSamples = 0;
            curInfo.Source.Play();
            return curInfo.Source;
        }
        public void Stop(ClipInfo clip, GameObject player)
        {
            foreach (var info in infos)
                if (info.Target == player && info.Name == clip.Name)
                    info.Source.Stop();
        }
        private void FixedUpdate()
        {
            foreach (var info in infos)
                if (info.Target != null && info.Source.isPlaying)
                {
                    if (info.Source.timeSamples < info.TimeSamples)
                        info.Source.pitch = 1 + Config[info.Name].Pitch;
                    info.TimeSamples = info.Source.timeSamples;
                    info.Source.transform.position = info.Target.transform.position;
                }
        }
    }
}