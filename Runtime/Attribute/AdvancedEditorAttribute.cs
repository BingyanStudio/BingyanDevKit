using System;
using UnityEngine;

/// <summary>
/// 启用高级 Inspector 绘制<br/>
/// 使用该 Attribute 后，将使用 DevKit 重载的编辑器绘制器进行 Inspector 绘制，以提供高级功能。<br/>
/// 部分其他 Attribute 依赖该 Attribute 发挥效果<br/>
/// 例如 <see cref="CategoryAttribute"/>
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class AdvancedEditorAttribute : PropertyAttribute { }