using System.Collections.Generic;
using UnityEngine;

namespace Bingyan
{
    public static class ListExtensions
    {
        /// <summary>
        /// 获取列表中的随机一个元素
        /// </summary>
        /// <returns>列表中随机一个元素，若列表为空，则返回空</returns>
        public static T GetRand<T>(this List<T> list)
            => list.Count == 0 ? default : list[Random.Range(0, list.Count)];

        /// <summary>
        /// 获取列表中的随机一个元素并返回下标
        /// </summary>
        /// <param name="idx">对应元素的下标，若列表为空，则返回 0</param>
        /// <returns>列表中随机一个元素，若列表为空，则返回空</returns>
        public static T GetRand<T>(this List<T> list, out int idx)
        {
            idx = 0;
            if (list.Count == 0) return default;

            idx = Random.Range(0, list.Count);
            return list[idx];
        }
    }
}