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
        public static RectTransform AsRectTransform(this Transform transform) => transform as RectTransform;
    }
}