using System;
using UnityEngine;
using UnityEditor;
using System.Linq;
using static UnityEditor.EditorGUI;
using System.Reflection;

namespace Bingyan.Editor
{
    /// <summary>
    /// 绘制 <see cref="Callback"/> 对属性框的更改
    /// <para>绘制一个限定了范围的方法选择器</para>
    /// </summary>
    [CustomPropertyDrawer(typeof(Callback<>))]
    public class CallbackDrawer : LinedPropertyDrawer
    {
        private static int fixedLines = 4;

        private static float padding = 0;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // DrawRect(new Rect(position.x, position.y, position.width, position.height - Spacing), new Color(0.2539f, 0.2539f, 0.2539f));
            position.x += padding;
            position.y += padding;
            position.width -= padding * 2;
            position.height -= padding * 2;
            base.OnGUI(position, property, label);

            var target = property.FindPropertyRelative("target");
            var comp = property.FindPropertyRelative("comp");

            BeginProperty(position, label, property);

            LabelField(pos, label, EditorStyles.boldLabel);
            Next();

            target.objectReferenceValue = ObjectField(PrefixLabel(pos, new GUIContent("目标物体", "可以执行交互指令的物体")), target.objectReferenceValue ?? ((Component)property.serializedObject.targetObject).gameObject,
                    typeof(GameObject), true);
            Next();

            var go = target.objectReferenceValue as GameObject;
            var comps = go.GetComponents<MonoBehaviour>();

            var compFullNames = comps.Select(i => i.GetType().AssemblyQualifiedName).ToList();
            var compNames = comps.Select(i => i.GetType().Name).ToList();

            var selectedComp = Popup(PrefixLabel(pos, new GUIContent("执行组件", "含有交互指令的组件 / Component")),
                                        comp.objectReferenceValue ?
                                            compNames.IndexOf(comp.objectReferenceValue.GetType().Name)
                                            : compNames.IndexOf(property.serializedObject.targetObject.GetType().Name),
                                        compNames.ToArray());
            Next();
            comp.objectReferenceValue = comps[selectedComp];

            // 依据泛型类型，找到目标Attr
            var targetAttrType = fieldInfo.FieldType.GetGenericArguments()[0];
            var mtds = comps[selectedComp].GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).ToList()
                    .FindAll(i => i.GetCustomAttribute(targetAttrType, true) != null && !i.IsGenericMethod);
            if (mtds.Count == 0)
            {
                LabelField(pos, $"没有找到带有 [{targetAttrType.Name.Split("Attribute")[0]}] 标签的方法 :(");
                EndProperty();
                property.serializedObject.ApplyModifiedProperties();
                return;
            }

            // 选择方法
            var mtdId = property.FindPropertyRelative("methodId");
            var mtdAttrs = mtds.Select(i => i.GetCustomAttribute(targetAttrType) as MethodDescripterAttribute).ToList();
            var mtdIds = mtdAttrs.Select(i => i.Id).ToList();
            var mtdDescs = mtdAttrs.Select(i => i.Desc).ToList();
            mtdDescs.Insert(0, "无");

            int selected = EditorGUI.Popup(EditorGUI.PrefixLabel(pos, new GUIContent("方法", "指定需要调用的方法")),
                                            mtdIds.Contains(mtdId.stringValue) ?
                                            mtdIds.IndexOf(mtdId.stringValue) + 1
                                            : 0, mtdDescs.ToArray());
            Next();
            // 选择了“无”
            if (selected == 0)
            {
                mtdId.stringValue = "";
                property.FindPropertyRelative("args").arraySize = 0;
                EndProperty();
                property.serializedObject.ApplyModifiedProperties();
                return;
            }
            selected--;      // 回归正常的序号，不再受“无”影响

            var mtd = mtds[selected];
            mtdId.stringValue = mtdIds[selected];
            var mtdArgs = mtd.GetParameters();
            var args = property.FindPropertyRelative("args");
            var argCount = mtdArgs.Length;
            args.arraySize = argCount;
            if (argCount == 0)
            {
                EndProperty();
                property.serializedObject.ApplyModifiedProperties();
                return;
            }

            LabelField(pos, new GUIContent("参数", "这个方法需要的参数"));
            Next();

            var argDatas = mtdAttrs[selected].ParamDescs;
            AddTab();
            for (int i = 0; i < argCount; i++)
            {
                var arg = args.GetArrayElementAtIndex(i);
                var argInfo = mtd.GetParameters()[i];
                var argName = argDatas.Count > i ? argDatas[i] : argInfo.Name;

                arg.FindPropertyRelative("typeFullName").stringValue = argInfo.ParameterType.FullName;
                switch (argInfo.ParameterType.FullName)
                {
                    case "System.Int32":
                        var v = arg.FindPropertyRelative("intVal");
                        v.intValue
                            = EditorGUI.IntField(pos, argName, v.intValue);
                        break;

                    case "System.Boolean":
                        v = arg.FindPropertyRelative("boolVal");
                        v.boolValue
                            = EditorGUI.Toggle(pos, argName, v.boolValue);
                        break;

                    case "System.Single":
                        v = arg.FindPropertyRelative("floatVal");
                        v.floatValue
                        = EditorGUI.FloatField(pos, argName, v.floatValue);
                        break;

                    case "System.String":
                        v = arg.FindPropertyRelative("strVal");
                        v.stringValue
                            = EditorGUI.TextField(EditorGUI.PrefixLabel(pos, new GUIContent(argName)), v.stringValue);
                        break;

                    case "UnityEngine.Vector2":
                        v = arg.FindPropertyRelative("vec2Val");
                        v.vector2Value
                            = EditorGUI.Vector2Field(pos, argName, v.vector2Value);
                        break;

                    default:
                        if (argInfo.ParameterType.IsSubclassOf(typeof(UnityEngine.Object)))
                        {
                            arg.FindPropertyRelative("typeFullName").stringValue = "UnityEngine.Object";
                            v = arg.FindPropertyRelative("objVal");
                            v.objectReferenceValue = EditorGUI.ObjectField(pos, new GUIContent(argName), v.objectReferenceValue, argInfo.ParameterType, true);
                        }
                        else EditorGUI.LabelField(pos, $"无法解析的类型: {argInfo.ParameterType.Name}");
                        break;
                }
                Next();
            }
            ReduceTab();

            EndProperty();
            property.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var lines = GetLines(property);
            return lines * lineHeight + (lines - 1) * padding + 2 * padding;
        }

        public static int GetLines(SerializedProperty property)
        {
            var lines = fixedLines;
            var target = property.FindPropertyRelative("target");
            var comp = property.FindPropertyRelative("comp");
            var mtd = property.FindPropertyRelative("methodId");
            var argCnt = property.FindPropertyRelative("args").arraySize;
            // +1 是为了空出更多的空间，好看！
            return lines + argCnt + (argCnt > 0 ? 3  : 2);
        }
    }
}