using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using static UnityEditor.EditorGUI;
using System.Reflection;

namespace Bingyan.Editor
{
    /// <summary>
    /// 绘制 <see cref="StrIDAttribute"/> 对属性框的更改
    /// <para>绘制可以选择的ID框，方便策划进行配置</para>
    /// </summary>
    [CustomPropertyDrawer(typeof(StrIDAttribute))]
    public class StringIDDrawer : LinedPropertyDrawer
    {
        public const char MANUAL_INPUT_SIGNATURE = '#';
        public const char PREFIX_COMMAND_SIGNATURE = '$';
        public const string PREFIX_SEPARATOR = ": ";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = attribute as StrIDAttribute;
            IDField(position, property, label, attr, fieldInfo);
        }

        /// <summary>
        /// 绘制一个ID选择框
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="property">字符串属性</param>
        /// <param name="label">标签</param>
        /// <param name="attr">StrID标签对象</param>
        /// <param name="field">[可选]这个属性对应的反射，用于一些特殊的前缀</param>
        public static void IDField(Rect position, SerializedProperty property, GUIContent label, StrIDAttribute attr, FieldInfo field = null)
            => IDField(position, property, label, attr.IdGroup, attr.AllowNew, attr.Prefix, field);

        /// <summary>
        /// 绘制一个ID选择框
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="property">字符串属性</param>
        /// <param name="label">标签</param>
        /// <param name="idGroup">ID所属的ID组</param>
        /// <param name="allowNew">是否允许新建ID</param>
        /// <param name="prefix">[可选]筛选ID的前缀</param>
        /// <param name="field">[可选]这个属性对应的反射，用于一些特殊的前缀</param>
        public static void IDField(Rect position, SerializedProperty property, GUIContent label, string idGroup, bool allowNew, string prefix = "", FieldInfo field = null)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                LabelField(position, "仅string类型属性可被StrID修饰");
                return;
            }

            prefix = GetPrefix(prefix, field);

            // 读取所有属于指定组，指定前缀的ID列表。
            var idList = GetIdList(idGroup, prefix);
            // 去除前缀
            for (int i = 0; i < idList.Count; i++) idList[i] = TrimPrefix(idList[i]);
            idList.Sort((l, r) => l[0] - r[0]);
            var list = new List<string>(idList);
            var extraIndex = 3;

            list.Insert(0, "无");
            list.Insert(1, "搜索");
            list.Insert(2, allowNew ? "新建..." : "手动输入...");


            if (!property.stringValue.StartsWith(MANUAL_INPUT_SIGNATURE))
            {
                int selected;
                if (!list.Contains(property.stringValue))
                {
                    if (property.stringValue != string.Empty) Debug.LogWarning($"ID {property.stringValue} 没有出现在可用ID列表里，已经设置为【无】");
                    property.stringValue = string.Empty;
                    selected = 0;
                }
                else selected = list.IndexOf(property.stringValue);

                var prefixedPos = PrefixLabel(position, label);
                selected = Popup(prefixedPos, selected, list.ToArray());
                if (selected == 0) property.stringValue = string.Empty;
                else if (selected == 1) PopupWindow.Show(prefixedPos, new StringSearchPopupContent(property, idList));
                else if (selected == 2) property.stringValue = MANUAL_INPUT_SIGNATURE + "id";
                else property.stringValue = TrimPrefix(idList[selected - extraIndex]);
            }
            else
            {
                var inputRect = new Rect(position);
                inputRect.width -= 90;
                var newId = TrimPrefix(DelayedTextField(inputRect, label, property.stringValue.TrimStart(MANUAL_INPUT_SIGNATURE))
                                .TrimStart(MANUAL_INPUT_SIGNATURE)
                                .TrimStart(PREFIX_COMMAND_SIGNATURE));
                property.stringValue = MANUAL_INPUT_SIGNATURE + newId;

                var btn = new Rect(inputRect);
                btn.x += btn.width + 5;
                btn.width = 40;

                // 保存临时变量，请参考这里的代码
                var e = Event.current;
                if (GUI.Button(btn, "确定") || (e.isKey && e.keyCode == KeyCode.Return))
                {
                    // 保存在属性中的ID是没有前缀的，前缀只是用来筛选ID的
                    property.stringValue = newId;

                    // 保存到设置里的ID需要加前缀
                    newId = prefix + newId;

                    var modifiedList = DevKitSetting.GetIds(idGroup);
                    if (!modifiedList.Contains(newId))
                        if (allowNew) modifiedList.Add(newId);
                        else
                        {
                            property.stringValue = string.Empty;
                            DialogUtils.Show("错误", "没有找到这个id，请检查输入是否有误", isErr: false);
                            return;
                        }

                    var serConfig = DevKitSetting.instance.GetSerializedObject();
                    serConfig.FindProperty("ids_json").stringValue = DevKitSetting.instance.AddIdListToJson(idGroup, modifiedList);
                    serConfig.ApplyModifiedPropertiesWithoutUndo();
                }
                else
                {
                    btn.x += btn.width + 5;
                    if (GUI.Button(btn, "取消"))
                        property.stringValue = string.Empty;
                }
            }
            property.serializedObject.ApplyModifiedProperties();
        }

        protected static string GetPrefix(string prefix, FieldInfo field)
        {
            if (prefix == string.Empty || prefix == null) return string.Empty;
            if (!prefix.StartsWith(PREFIX_COMMAND_SIGNATURE)) return prefix + PREFIX_SEPARATOR;
            prefix = prefix.TrimStart(PREFIX_COMMAND_SIGNATURE);
            switch (prefix)
            {
                case "class":
                    if (field == null)
                    {
                        DialogUtils.Show("错误:", $"需要FieldInfo不为空，才可以解析{prefix}");
                        return string.Empty;
                    }
                    else return field.DeclaringType.Name + PREFIX_SEPARATOR;
                default:
                    Debug.LogError($"错误: 未知的指令符: {prefix}");
                    return string.Empty;
            }
        }
        protected static List<string> GetIdList(string idGroup, string prefix) => DevKitSetting.GetIds(idGroup).FindAll(i => i.StartsWith(prefix));
        protected static string TrimPrefix(string id)
        {
            if (!id.Contains(PREFIX_SEPARATOR)) return id;
            var arr = id.Split(PREFIX_SEPARATOR);
            return arr[arr.Length - 1];
        }
    }
}