using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bingyan
{
    public static class RandomUtils
    {
        #region Utils

        /// <summary>
        /// 从输入中随机选择一项
        /// </summary>
        /// <param name="choices">输入的各个选项</param>
        /// <returns>随机选择的那一项</returns>
        public static T Choose<T>(params T[] choices)
            => choices[Random.Range(0, choices.Length)];

        /// <summary>
        /// 在给定区域内随机选择一点
        /// </summary>
        /// <param name="p1">区域的一个角</param>
        /// <param name="p2">区域的对角</param>
        /// <returns>随机选取的点</returns>
        public static Vector2 Inside(Vector2 p1, Vector2 p2)
            => Inside(p1.x, p1.y, p2.x, p2.y);

        /// <summary>
        /// 在给定区域内随机选择一点
        /// </summary>
        /// <param name="p1">区域的一个角</param>
        /// <param name="p2">区域的对角</param>
        /// <returns>随机选取的点</returns>
        public static Vector2 Inside(float x1, float y1, float x2, float y2)
            => new(Random.Range(x1, x2), Random.Range(y1, y2));

        #endregion

        #region Extensions

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

        /// <summary>
        /// 将列表随机打乱
        /// </summary>
        /// <returns>打乱后的列表</returns>
        public static List<T> Shuffle<T>(this List<T> list)
            => list.OrderBy(i => Random.value).ToList();

        /// <summary>
        /// 将数组随机打乱
        /// </summary>
        /// <returns>打乱后的列表</returns>
        public static T[] Shuffle<T>(this T[] arr)
            => arr.OrderBy(i => Random.value).ToArray();

        #endregion
    }
}