using System.Collections.Generic;
using UnityEngine;

namespace Bingyan
{
    internal class AudioManager : MonoBehaviour
    {
        internal static AudioManager Instance { get; private set; }

        static AudioManager()
        {
            Instance = new GameObject().AddComponent<AudioManager>();
            DontDestroyOnLoad(Instance.gameObject);

            Instance.Init();
        }

        private AudioMapConfig config;
        private List<SourceState> states;

        private void Init()
        {
            config = AudioMapConfig.Instance;
            config.Init();
            states = new();
        }

        /// <summary>
        /// 播放<paramref name="player"/>，且保证只有一个AudioSource播放它
        /// </summary>
        internal AudioSource PlaySingleton(ClipPlayer player) => Play(player, gameObject, true);

        /// <summary>
        /// 跟踪<paramref name="target"/>播放<paramref name="player"/>，且保证只有一个AudioSource播放它
        /// </summary>
        internal AudioSource PlaySingleton(ClipPlayer player, GameObject target) => Play(player, target, true);

        /// <summary>
        /// 播放<paramref name="player"/>
        /// </summary>
        internal AudioSource Play(ClipPlayer player) => Play(player, gameObject, false);

        /// <summary>
        /// 跟踪<paramref name="target"/>播放<paramref name="player"/>
        /// </summary>
        internal AudioSource Play(ClipPlayer player, GameObject target) => Play(player, target, false);

        private AudioSource Play(ClipPlayer player, GameObject target, bool singleton)
        {
            SourceState state = null;

            foreach (var item in states)
            {
                if (singleton && item.Name == player.Name && item.Source.isPlaying)
                {
                    item.Target = target;
                    return item.Source;
                }
                if (!item.Source.isPlaying)
                {
                    state = item;
                    if (!singleton) break;
                }
            }

            if (state == null)
            {
                state = new SourceState
                {
                    Source = new GameObject().AddComponent<AudioSource>()
                };
                state.Source.transform.parent = transform;
                states.Add(state);
            }
            state.Name = player.Name;
            state.Target = target;

            var info = config[player.Name];

            state.Source.clip = info.Clips.Length > 0 ? info.Clips[Random.Range(0, info.Clips.Length)] : null;
            state.Source.loop = info.Loop;
            state.Source.pitch = 1 + info.Pitch;
            state.Source.outputAudioMixerGroup = info.Bus;
            state.Source.spatialBlend = info.Stereo;
            state.TimeSamples = 0;
            state.Clip = info;
            state.Source.Play();
            return state.Source;
        }

        /// <summary>
        /// 停止播放<paramref name="player"/>
        /// </summary>
        internal void Stop(ClipPlayer player)
        {
            foreach (var info in states)
                if (info.Target == gameObject && info.Name == player.Name)
                    info.Source.Stop();
        }
        /// <summary>
        /// 停止播放正在跟踪<paramref name="target"/>的<paramref name="player"/>
        /// </summary>
        internal void Stop(ClipPlayer player, GameObject target)
        {
            foreach (var info in states)
                if (info.Target == target && info.Name == player.Name)
                    info.Source.Stop();
        }
        private void FixedUpdate()
        {
            foreach (var info in states)
                if (info.Target != null && info.Source.isPlaying)
                {
                    if (info.Source.timeSamples < info.TimeSamples)
                        info.Source.pitch = 1 + info.Clip.Pitch;
                    info.TimeSamples = info.Source.timeSamples;
                    info.Source.transform.position = info.Target.transform.position;
                }
        }

        private class SourceState
        {
            internal string Name;
            internal GameObject Target;
            internal AudioSource Source;
            internal float TimeSamples;
            internal ClipInfo Clip;
        }
    }
}