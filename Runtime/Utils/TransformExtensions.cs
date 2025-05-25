using System.Runtime.CompilerServices;
using UnityEngine;

namespace Bingyan
{
    public static class TransformExtensions
    {
        /// <summary>
        /// 控制横向翻转，通过修改 localScale 实现
        /// </summary>
        /// <param name="inverse">是否翻转</param>
        /// <returns>原 Transform</returns>
        public static Transform FlipX(this Transform transform, bool inverse)
        {
            transform.localScale = new Vector3((inverse ? -1 : 1) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            return transform;
        }

        /// <summary>
        /// 控制竖向翻转，通过修改 localScale 实现
        /// </summary>
        /// <param name="inverse">是否翻转</param>
        /// <returns>原 Transform</returns>
        public static Transform FlipY(this Transform transform, bool inverse)
        {
            transform.localScale = new Vector3(transform.localScale.x, (inverse ? -1 : 1) * Mathf.Abs(transform.localScale.y), transform.localScale.z);
            return transform;
        }

        /// <summary>
        /// 将 <see cref="Transform"/> 快速转为 <see cref="RectTransform"/>
        /// </summary>
        /// <returns>转型后的 <see cref="RectTransform"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectTransform AsRectTransform(this Transform transform) => transform as RectTransform;

        /// <summary>
        /// 判断一个物体是否是另一个物体的祖宗
        /// </summary>
        /// <param name="child">需要判断的子物体</param>
        /// <param name="maxDepth">最大搜索深度。如果不指定，则搜索到场景最上层。</param>
        /// <returns>是否是祖宗</returns>
        public static bool IsAncesterOf(this Transform self, Transform child, int maxDepth = int.MaxValue)
        {
            var tr = child;
            for (int i = 0; i < maxDepth; i++)
            {
                if (tr.parent == self) return true;
                else if (!tr.parent) break;
                tr = tr.parent;
            }
            return false;
        }
    }
}