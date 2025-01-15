using UnityEngine;
using System;

namespace Bingyan
{
    /// <summary>
    /// 指定一个 int 类型的变量只能填入某个层级的序号<br/>
    /// 类似于 LayerMask, 但只能选择一个 Layer，且得到序号而非掩码
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class LayerAttribute : PropertyAttribute { }
}