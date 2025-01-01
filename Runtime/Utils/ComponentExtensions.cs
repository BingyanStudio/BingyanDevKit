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

        /// <summary>
        /// 按路径获取组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="comp">开始搜索的组件（根）</param>
        /// <param name="path">路径</param>
        /// <returns>按路径找到的组件</returns>
        public static T GetComp<T>(this Component comp, string path) where T : Component
            => comp.transform.Find(path).GetComponent<T>();

        /// <summary>
        /// 按路径尝试获取组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="comp">开始搜索的组件（根）</param>
        /// <param name="path">路径</param>
        /// <param name="result">按照路径找到的组件</param>
        /// <returns>是否找到</returns>
        public static bool TryGetComp<T>(this Component comp, string path, out T result) where T : Component
        {
            Transform tr;
            result = null;
            if (tr = comp.transform.Find(path)) return tr.TryGetComponent(out result);
            else return false;
        }

        /// <summary>
        /// 直接以 <see cref="Vector2"/> 的形式获取 <see cref="Transform"/> 的位置
        /// </summary>
        /// <param name="tr">物体的 <see cref="Transform"/> 组件</param>
        /// <returns><see cref="Vector2"/> 位置</returns>
        public static Vector2 Pos2D(this Component comp)
            => comp.transform.position.Vec2();
    }
}