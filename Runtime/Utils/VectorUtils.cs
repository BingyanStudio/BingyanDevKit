using UnityEngine;

namespace Bingyan
{
    public static class VectorUtils
    {
        /// <summary>
        /// 选取长度更小的向量
        /// </summary>
        /// <param name="left">比较的左侧</param>
        /// <param name="right">比较的右侧</param>
        /// <returns>长度较小的向量</returns>
        public static Vector2 MinLen(Vector2 left, Vector2 right)
            => left.sqrMagnitude > right.sqrMagnitude ? right : left;

        /// <summary>
        /// 选取长度更大的向量
        /// </summary>
        /// <param name="left">比较的左侧</param>
        /// <param name="right">比较的右侧</param>
        /// <returns>长度较大的向量</returns>
        public static Vector2 MaxLen(Vector2 left, Vector2 right)
            => left.sqrMagnitude > right.sqrMagnitude ? left : right;
    }
}