using UnityEngine;

namespace Bingyan
{
    public static class TransformExtensions
    {
        public static Transform FlipX(this Transform transform, bool inverse)
        {
            transform.localScale = new Vector3((inverse ? -1 : 1) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            return transform;
        }

        public static Transform FlipY(this Transform transform, bool inverse)
        {
            transform.localScale = new Vector3(transform.localScale.x, (inverse ? -1 : 1) * Mathf.Abs(transform.localScale.y), transform.localScale.z);
            return transform;
        }
    }
}