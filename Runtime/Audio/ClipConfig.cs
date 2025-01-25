using UnityEngine;
using System.Collections.Generic;

namespace Bingyan
{
    [CreateAssetMenu(menuName = "DevKit/ClipConfig")]
    public class ClipConfig : ScriptableObject
    {
        [SerializeField] private string scriptName = "ClipMap";
        [SerializeField] private ClipGroup[] groups;
        private Dictionary<string, ClipInfo> infoDict;
        public ClipConfig Init()
        {
            infoDict = new();
            foreach (var group in groups)
                foreach (var info in group.Infos)
                {
                    info.Bus = group.Bus;
                    infoDict.Add(group.Name + ' ' + info.Name, info);
                }
            return this;
        }
        public ClipInfo this[string name] => infoDict[name];
    }
}
