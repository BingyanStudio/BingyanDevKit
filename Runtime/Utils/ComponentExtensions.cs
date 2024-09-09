using UnityEngine;

namespace Bingyan
{
    public static class ComponentExtensions
    {
        /// <summary>
        /// 尝试在子物体中获取组件（本身也会被搜索）
        /// </summary>
        /// <param name="result">组件实例</param>
        /// <param name="includeInactive">是否包括被禁用的组件</param>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>组件是否存在</returns>
        public static bool TryGetComponentInChildren<T>(this Component component, out T result, bool includeInactive = true)
        {
            result = component.GetComponentInChildren<T>(includeInactive);
            return result != null;
        }

        /// <summary>
        /// 尝试在父物体中获取组件（本身不会被搜索）
        /// </summary>
        /// <param name="result">组件实例</param>
        /// <param name="includeInactive">是否包括被禁用的组件</param>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>组件是否存在</returns>
        public static bool TryGetComponentInParent<T>(this Component component, out T result, bool includeInactive = true)
        {
            result = component.GetComponentInParent<T>(includeInactive);
            return result != null;
        }
    }
}