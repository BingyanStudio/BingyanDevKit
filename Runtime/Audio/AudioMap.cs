using UnityEngine;
using System.Collections.Generic;

namespace Bingyan
{
    #pragma warning disable CS0414
    [CreateAssetMenu(menuName = "DevKit/AudioMap")]
    public class AudioMapConfig : ScriptableConfig<AudioMapConfig>
    {
        [SerializeField] private string scriptName = "AudioMap";
        [SerializeField] private ClipGroup[] groups;
        private Dictionary<string, ClipInfo> infoDict;
        public AudioMapConfig Init()
        {
            infoDict = new();
            foreach (var group in groups)
                foreach (var info in group.Infos)
                {
                    info.Bus = group.Bus;
                    infoDict.Add(group.Name + '/' + info.Name, info);
                }
            return this;
        }
        internal ClipInfo this[string name] => infoDict[name];
    }
}
