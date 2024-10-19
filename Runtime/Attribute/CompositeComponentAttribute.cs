using System;
using System.Linq;

namespace Bingyan
{
    /// <summary>
    /// 将一个 <see cref="ScriptableObject"/> 标记为组件
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public abstract class CompositeComponentAttribute : Attribute
    {
        /// <summary>
        /// 组件的菜单名称
        /// </summary>
        public string Menu { get; private set; }

        /// <summary>
        /// 组件自身的展示名称，取菜单名称的末尾
        /// </summary>
        public string CompName { get; private set; }

        /// <summary>
        /// 将一个 <see cref="ScriptableObject"/> 标记为组件
        /// </summary>
        /// <param name="menu">菜单名称</param>
        public CompositeComponentAttribute(string menu)
        {
            Menu = menu;
            CompName = menu.Split('/').Last();
        }
    }
}