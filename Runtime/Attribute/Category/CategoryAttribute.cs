using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class CategoryAttribute : PropertyAttribute
{
    public readonly string Name;

    public CategoryAttribute(string name)
    {
        Name = name;
    }
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class SubCategoryAttribute : PropertyAttribute
{
    public readonly string Name;

    public SubCategoryAttribute(string name)
    {
        Name = name;
    }
}