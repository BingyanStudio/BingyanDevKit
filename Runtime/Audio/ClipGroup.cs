using System;
using UnityEngine.Audio;

namespace Bingyan
{
    [Serializable]
    internal class ClipGroup
    {
        public string Name;
        public AudioMixerGroup Bus;
        public ClipInfo[] Infos;
    }
}