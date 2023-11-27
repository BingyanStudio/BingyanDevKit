using System;
using System.Collections.Generic;

namespace Bingyan
{
    /// <summary>
    /// 用于描述方法的Attribute。
    /// <para>主要是为了将Inspector里注册委托事件的过程变得更加"策划友好"</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class MethodDescripterAttribute : Attribute
    {
        private string id;
        private string desc;
        private List<string> paramDescs = new List<string>();

        /// <summary>
        /// 提供这个方法的描述，用于在Unity编辑器里显示
        /// </summary>
        /// <param name="id">这个方法的识别ID，用于确定究竟是哪个方法</param>
        /// <param name="desc">方法的描述，在编辑器里展示</param>
        /// <param name="paramDescs">各个参数的描述，如果没有，则使用参数的名称</param>
        public MethodDescripterAttribute(string id, string desc, params string[] paramDescs)
        {
            this.id = id;
            this.desc = desc;
            this.paramDescs = new List<string>(paramDescs);
        }

        public string Id => id;
        public string Desc => desc;
        public List<string> ParamDescs => paramDescs;
    }
}