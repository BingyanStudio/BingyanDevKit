using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Bingyan
{
#pragma warning disable CS0414
    [CreateAssetMenu(menuName = "DevKit/AudioMap")]
    public class AudioMapConfig : ScriptableConfig<AudioMapConfig>
    {
        [SerializeField] private string scriptPath = "Assets/AudioMap.Generated.cs";
        [SerializeField] private ClipGroup[] groups;

        private Dictionary<string, ClipInfo> infoDict;

        public AudioMapConfig Init()
        {
            infoDict = new();
            foreach (var group in groups)
                foreach (var info in group.Infos)
                {
                    info.Bus = group.Bus;
                    infoDict.Add($"{group.Name}/{info.Name}", info);
                }
            return this;
        }

        internal ClipInfo this[string name] => infoDict[name];
    }

    [Serializable]
    internal class ClipGroup
    {
        public string Name;
        public AudioMixerGroup Bus;
        public ClipInfo[] Infos;
    }

    [Serializable]
    internal class ClipInfo
    {
        public string Name;
        public AudioClip[] Clips;
        public bool Loop;
        public float Range;
        public FloatRange Pitch;
        public AudioMixerGroup Bus;
    }
}
