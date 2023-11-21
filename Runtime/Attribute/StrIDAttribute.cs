using System;
using UnityEngine;

namespace Bingyan
{
    /// <summary>
    /// 用于将一个暴露给Unity字符串变成自定义ID选择器的Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class StrIDAttribute : PropertyAttribute
    {
        private string idGroupKey;
        private bool allowNew;
        private string prefix;

        public string IdGroup => idGroupKey;
        public bool AllowNew => allowNew;
        public string Prefix => prefix;

        /// <summary>
        /// 用于将一个暴露给Unity字符串变成自定义ID选择器的Attribute
        /// </summary>
        /// <param name="idGroup">这个id的组别。不同组别的id允许出现相同的项，且不会冲突</param>
        /// <param name="allowNew">是否出现【创建新的id】选项</param>
        /// <param name="prefix">ID的前缀。【不要】在其中混入英文冒号':'</param>
        public StrIDAttribute(string idGroup, bool allowNew = true, string prefix = "")
        {
            this.idGroupKey = idGroup;
            this.allowNew = allowNew;
            this.prefix = prefix.Trim(' ', ':');
        }
    }
}