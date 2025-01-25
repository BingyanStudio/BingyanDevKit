using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using System;

namespace Bingyan.Editor
{
    [CustomEditor(typeof(UnityEngine.Object), true)]
    public class CategorizedObjectEditor : UnityEditor.Editor
    {
        private static GUIStyle catStyle, subStyle, subBoxStyle;
        private static float catSpacing, subSpacing;

        private static string cat, subcat;
        private static readonly Dictionary<string, Dictionary<string, List<string>>> cats = new();
        private static readonly List<string> catOrder = new();
        private static readonly Dictionary<string, List<string>> subcatOrder = new();

        private readonly HashSet<string> catOpenedSet = new(), subOpenedSet = new();

        private static void Init()
        {
            catStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 16,
                onNormal = new GUIStyleState()
                {
                    textColor = Color.yellow,
                },

            };

            subStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 14,
            };

            subBoxStyle = new GUIStyle(EditorStyles.selectionRect);

            catSpacing = EditorGUIUtility.singleLineHeight * 0.6f;
            subSpacing = EditorGUIUtility.singleLineHeight * 0.3f;
        }

        public override void OnInspectorGUI()
        {
            if (target.GetType().GetCustomAttribute<AdvancedEditorAttribute>() == null) base.OnInspectorGUI();
            else
            {
                if (catStyle == null) Init();

                if (targets.Length > 1)
                {
                    EditorGUILayout.HelpBox("不支持多物体编辑!", MessageType.Warning);
                    return;
                }

                var prop = serializedObject.GetIterator();
                prop.Next(true);
                prop.NextVisible(false);

                cat = null; subcat = null;
                cats.Clear();
                catOrder.Clear();
                subcatOrder.Clear();

                while (prop.NextVisible(false))
                {
                    string currentCat, currentSub;
                    FieldInfo field = null;
                    Type type = target.GetType();
                    while (type != typeof(UnityEngine.Object))
                    {
                        field = type.GetField(prop.name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                        if (field != null) break;

                        type = type.BaseType;
                    }

                    if (field == null) return;
                    var catAttr = field.GetCustomAttribute<CategoryAttribute>();
                    var subAttr = field.GetCustomAttribute<SubCategoryAttribute>();
                    currentCat = catAttr != null ? catAttr.Name : (cat ?? string.Empty);
                    // cat 先置 null, 再在上面 ?? 判定
                    // 是为了提高在用户全部指定了 cat 时，不添加空字符串作为默认 cat

                    if (currentCat != cat)
                    {
                        subcat = null;
                        if (cats.TryAdd(currentCat, new()))
                            catOrder.Add(currentCat);
                    }

                    currentSub = subAttr != null ? subAttr.Name : (subcat ?? string.Empty);

                    if (currentSub != subcat && cats[currentCat].TryAdd(currentSub, new()))
                    {
                        subcatOrder.TryAdd(currentCat, new());
                        subcatOrder[currentCat].Add(currentSub);
                    }

                    cats[currentCat][currentSub].Add(prop.name);

                    cat = currentCat;
                    subcat = currentSub;
                }

                prop.Reset();
                foreach (var cat in catOrder)
                {
                    bool renderTitle = cat.Length > 0;
                    if (renderTitle)
                    {
                        if (EditorGUILayout.Foldout(catOpenedSet.Contains(cat), cat, true, catStyle))
                        {
                            catOpenedSet.Add(cat);
                            EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing * 2);
                        }
                        else
                        {
                            catOpenedSet.Remove(cat);
                            EditorGUILayout.Space(catSpacing);
                            continue;
                        }
                        EditorGUI.indentLevel++;
                    }

                    foreach (var sub in subcatOrder[cat])
                    {
                        var renderSub = sub.Length > 0;
                        var id = $"{cat}_{sub}";
                        if (renderSub)
                        {
                            EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
                            EditorGUILayout.BeginVertical(subBoxStyle);
                            if (EditorGUILayout.Foldout(subOpenedSet.Contains(id), sub, true, subStyle))
                            {
                                subOpenedSet.Add(id);
                                EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
                            }
                            else
                            {
                                subOpenedSet.Remove(id);
                                EditorGUILayout.EndVertical();
                                EditorGUILayout.Space(subSpacing);
                                continue;
                            }
                        }

                        foreach (var p in cats[cat][sub])
                            EditorGUILayout.PropertyField(serializedObject.FindProperty(p));

                        if (renderSub)
                        {
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.Space(subSpacing);
                        }
                    }

                    if (renderTitle)
                    {
                        EditorGUI.indentLevel--;
                        EditorGUILayout.Space(catSpacing);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}