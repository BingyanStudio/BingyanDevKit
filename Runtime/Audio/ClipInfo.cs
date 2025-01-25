using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Bingyan
{
    [Serializable]
    public class ClipInfo
    {
        public string Name;
        public AudioClip[] Clips;
        public bool Loop;
        public FloatRange Pitch;
        public AudioMixerGroup Bus;
        [Range(0, 1)] public float Stereo;
    }
}