using UnityEngine;
using System;

namespace Bingyan
{
    /// <summary>
    /// 用于指定一个string类型的属性只能填入场景名称的标签
    /// <para>主要是为了将Inspector里注册委托事件的过程变得更加"策划友好"</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SceneNameAttribute : PropertyAttribute { }
}