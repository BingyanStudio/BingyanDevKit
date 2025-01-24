using System;
using UnityEngine.Audio;

namespace Bingyan
{
    [Serializable]
    public class ClipGroup
    {
        public string Name;
        public AudioMixerGroup Bus;
        public ClipInfo[] Infos;
    }
}