using System;
using UnityEngine;

namespace Bingyan
{
    /// <summary>
    /// 修改 Inspector 内属性的标签的 Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class TitleAttribute : PropertyAttribute
    {
        public string Label => label;
        private string label;

        /// <summary>
        /// 修改 Inspector 内属性的标签的 Attribute, 方便策划读懂
        /// <para>注意：如果有多个Attr，则应当把这个放在最前面，否则会画不出来</para>
        /// <para>以及, 这个对复杂的属性不管用(如列表, <see cref="UnityEngine.Events.UnityEvent"/>等), 也可能会画不出来</para>
        /// <para>复杂属性建议使用 <see cref="HeaderAttribute"/> 来加中文</para>
        /// </summary>
        /// <param name="label">描述标签</param>
        public TitleAttribute(string label) { this.label = label; }
    }
}