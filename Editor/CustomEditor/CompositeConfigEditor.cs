using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static UnityEditor.EditorGUILayout;

namespace Bingyan.Editor
{
    /// <summary>
    /// 组合式 <see cref="ScriptableObject"/> 编辑器<br/>
    /// 可以让你的 <see cref="ScriptableObject"/> 按照组合 / 组件的方式进行构建<br/>
    /// 他的编辑方式很像 <see cref="GameObject" /> 在添加组件时的样子
    /// </summary>
    /// <typeparam name="T">检索所有可用组件时依据的 Attribute, 必须是 <see cref="CompositeComponentAttribute"/> 的子类</typeparam>
    public abstract class CompositeConfigEditor<T>
                            : UnityEditor.Editor
                            where T : CompositeComponentAttribute
    {
        private static Dictionary<Type, List<string>> menus = new();
        private static Dictionary<Type, Dictionary<string, string>> menuToClassName = new();
        private static Dictionary<Type, Dictionary<Type, string>> typeToName = new();

        /// <summary>
        /// 是否在文件目录中显示挂载在 <see cref="ScriptableObject"/> 上的所有组件
        /// </summary>
        protected virtual bool ShowSubObjs => false;

        /// <summary>
        /// 在没有检测到任何可用作组件的 <see cref="ScriptableObject" /> 时出现的提示 
        /// </summary>
        protected virtual string NoCompScriptHint => "你还没有编写任何组件！";

        /// <summary>
        /// “添加组件”按钮的文字
        /// </summary>
        protected virtual string AddCompHint => "添加组件";

        /// <summary>
        /// “删除组件”提示框的标题
        /// </summary>
        protected virtual string RemoveCompHint => "删除组件";

        /// <summary>
        /// 获取正在编辑的物体中，用于存储所有组件的属性
        /// </summary>
        /// <param name="target">物体</param>
        /// <returns>对应属性</returns>
        protected abstract SerializedProperty GetCompField(SerializedObject target);

        /// <summary>
        /// 获取可用的所有组件的菜单列表
        /// </summary>
        /// <returns>菜单列表</returns>
        protected virtual List<string> GetCompMenu() => menus[typeof(T)];

        /// <summary>
        /// 使用组件菜单名称访问组件的类名
        /// </summary>
        /// <param name="menu">菜单名称</param>
        /// <returns>类名</returns>
        protected virtual string CompMenuToClassName(string menu) => menuToClassName[typeof(T)][menu];

        /// <summary>
        /// 使用组件的类型访问其展示名称
        /// </summary>
        /// <param name="type">组件类型</param>
        /// <returns>展示名称</returns>
        protected virtual string TypeToCompName(Type type) => typeToName[typeof(T)][type];

        private void OnEnable()
        {
            var type = typeof(T);
            if (menus.ContainsKey(type)) return;

            var types = type.Assembly.GetTypes()
                .Where(i => i.GetCustomAttributes(type, false).Count() > 0)
                .Select(i => (i, i.FullName, (T)i.GetCustomAttributes(type, false).First()));
            menus.Add(type, types.Select(i => i.Item3.Menu).ToList());
            menuToClassName.Add(type, types.ToDictionary(i => i.Item3.Menu, j => j.FullName));
            typeToName.Add(type, types.ToDictionary(i => i.Item1, j => j.Item3.CompName));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Space(30);
            var menuItems = GetCompMenu();
            if (menuItems.Count == 0)
            {
                HelpBox(NoCompScriptHint, MessageType.Warning);
                serializedObject.ApplyModifiedProperties();
                return;
            }

            var path = AssetDatabase.GetAssetPath(target);
            if (GUILayout.Button(AddCompHint, GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.5f)))
            {
                var menu = new GenericMenu();
                menuItems.ForEach(i => menu.AddItem(new GUIContent(i), false,
                 () =>
                 {
                     var name = CompMenuToClassName(i);
                     var item = ScriptableObject.CreateInstance(name);
                     item.name = name;
                     item.hideFlags = (ShowSubObjs ? HideFlags.None : HideFlags.HideInHierarchy) | HideFlags.NotEditable;
                     AssetDatabase.AddObjectToAsset(item, target);
                     AssetDatabase.SaveAssets();
                 }));
                menu.ShowAsContext();
            }

            var comps = GetCompField(serializedObject);
            var items = AssetDatabase.LoadAllAssetsAtPath(path);
            comps.arraySize = Mathf.Max(0, items.Length - 1);
            int idx = 0;

            foreach (var item in items)
            {
                if (!item)
                {
                    AssetDatabase.RemoveObjectFromAsset(item);
                    comps.arraySize--;
                    continue;
                }

                EditorGUILayout.Space(30f);
                if (item == target) continue;

                comps.GetArrayElementAtIndex(idx++).objectReferenceValue = item;
                item.hideFlags = (ShowSubObjs ? HideFlags.None : HideFlags.HideInHierarchy) | HideFlags.NotEditable;

                BeginHorizontal();
                var itemName = TypeToCompName(item.GetType());
                LabelField(itemName, EditorStyles.boldLabel);
                if (GUILayout.Button("移除") && DialogUtils.Show(RemoveCompHint, $"你确定要删除 {itemName} 吗?", isErr: false))
                {
                    AssetDatabase.RemoveObjectFromAsset(item);
                    AssetDatabase.SaveAssets();
                    break;
                }
                EndHorizontal();
                EditorGUI.indentLevel++;
                var editor = UnityEditor.Editor.CreateEditor(item);
                editor.OnInspectorGUI();
                EditorGUI.indentLevel--;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}