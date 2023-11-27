using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Bingyan.Editor
{
    /// <summary>
    /// 绘制 <see cref="SceneNameAttribute"/> 对属性框的更改
    /// <para>绘制用于选择场景名称的选择框，方便策划选中相关场景</para>
    /// </summary>
    [CustomPropertyDrawer(typeof(SceneNameAttribute))]
    public class SceneNamePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // base.OnGUI(position, property, label);
            List<string> l = EditorBuildSettings.scenes.Select(i =>
            {
                var p = i.path.Split('/');
                return p[p.Length - 1].Split('.')[0];
            }).ToList();

            var display = l.Select(i => new GUIContent(i)).ToList();
            display.Add(new GUIContent("找不到? 去BuildSettings添加对应场景"));

            int sel = EditorGUI.Popup(position, label,
                                l.IndexOf(property.stringValue) == -1 ? 0 : l.IndexOf(property.stringValue),
                                display.ToArray());
            property.stringValue = sel < l.Count ? l[sel] : l[0];
        }
    }
}