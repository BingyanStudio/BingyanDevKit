using System;
using UnityEngine;

/// <summary>
/// 为 Inspector 内绘制的属性添加分类<br/>
/// 所有属于相同分类的属性将被绘制在一起，而非按照声明顺序排列<br/>
/// 需要类型具有 <see cref="AdvancdEditorAttribute"/> 才能生效
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class CategoryAttribute : PropertyAttribute
{
    public readonly string Name;

    public CategoryAttribute(string name)
    {
        Name = name;
    }
}

/// <summary>
/// 为 Inspector 内绘制的属性添加小分类<br/>
/// 小分类会在大分类的基础上进行细分<br/>
/// 所有属于相同分类的属性将被绘制在一起，而非按照声明顺序排列<br/>
/// 需要类型具有 <see cref="AdvancdEditorAttribute"/> 才能生效
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class SubCategoryAttribute : PropertyAttribute
{
    public readonly string Name;

    public SubCategoryAttribute(string name)
    {
        Name = name;
    }
}