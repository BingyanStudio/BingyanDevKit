using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static UnityEditor.EditorGUILayout;

namespace Bingyan
{
    public abstract class CustomComponentEditor : UnityEditor.Editor
    {
        protected virtual bool ShowSubObjs => false;

        protected abstract SerializedProperty GetCompField(SerializedObject target);
        protected abstract List<string> GetCompMenu();
        protected abstract string CompMenuToClassName(string menu);
        protected abstract string TypeToCompName(string type);

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Space(30);
            var menuItems = GetCompMenu();
            if (menuItems.Count == 0)
            {
                HelpBox("你还没有编写任何组件!", MessageType.Warning);
                serializedObject.ApplyModifiedProperties();
                return;
            }

            var path = AssetDatabase.GetAssetPath(target);
            if (GUILayout.Button("添加组件", GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.5f)))
            {
                var menu = new GenericMenu();
                menuItems.ForEach(i => menu.AddItem(new GUIContent(i), false,
                 () =>
                 {
                     var name = CompMenuToClassName(i);
                     var item = ScriptableObject.CreateInstance(name);
                     item.name = name;
                     item.hideFlags = HideFlags.HideInHierarchy;
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
                EditorGUILayout.Space(30f);
                if (item == target) continue;

                comps.GetArrayElementAtIndex(idx++).objectReferenceValue = item;
                item.hideFlags = ShowSubObjs ? HideFlags.None : HideFlags.HideInHierarchy;

                BeginHorizontal();
                var itemName = TypeToCompName(item.GetType().ToString());
                LabelField(itemName, EditorStyles.boldLabel);
                if (GUILayout.Button("移除") && DialogUtils.Show("删除组件", $"你确定要删除 {itemName} 吗?", isErr: false))
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