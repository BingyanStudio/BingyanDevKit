using System;
using System.Linq;

[AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class CompositeComponentAttribute : Attribute
{
    public string Menu { get; private set; }
    public string CompName { get; private set; }

    public CompositeComponentAttribute(string menu)
    {
        Menu = menu;
        CompName = menu.Split('/').Last();
    }
}