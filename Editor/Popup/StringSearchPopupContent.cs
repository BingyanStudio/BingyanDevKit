using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using static UnityEditor.EditorGUI;

namespace Bingyan.Editor
{
    /// <summary>
    /// 一个简单的搜索弹窗，可以匹配字符串，并将输入的<see cref="SerializedProperty"/>的字符串值替换为选中的内容
    /// </summary>
    public class StringSearchPopupContent : PopupWindowContent
    {
        private const string SEARCH_BOX_NAME = "SearchBox";
        private float lineHeight => EditorGUIUtility.singleLineHeight;
        private float spacing => 2;
        private float margin => 3;
        private float lineCount = 5;

        private Vector2 scroll = Vector2.zero;

        private SerializedProperty property;
        private List<string> availables, match;
        private string searchText = "";
        private string selectedStr = "";

        /// <summary>
        /// 一个简单的搜索弹窗，可以匹配字符串，并将输入的<see cref="SerializedProperty"/>的字符串值替换为选中的内容
        /// </summary>
        /// <param name="prop">要修改的属性</param>
        /// <param name="availables">可用的搜索条目</param>
        public StringSearchPopupContent(SerializedProperty prop, List<string> availables)
        {
            this.availables = availables;
            match = availables;
            property = prop;

            if (match.Count > 0) selectedStr = match[0];
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(250, (lineHeight + spacing) * lineCount);
        }

        private Rect line;
        public override void OnGUI(Rect rect)
        {
            line = new Rect(rect.x + margin, rect.y + spacing, rect.width - margin * 2, lineHeight);

            if (Event.current.type == EventType.KeyDown)
            {
                if (match.Count > 0)
                {
                    if (Event.current.keyCode == KeyCode.DownArrow)
                    {
                        var target = match.IndexOf(selectedStr) + 1;
                        target = target >= match.Count ? 0 : target;
                        selectedStr = match[target];
                        Event.current.Use();
                    }

                    if (Event.current.keyCode == KeyCode.UpArrow)
                    {
                        var target = match.IndexOf(selectedStr) - 1;
                        selectedStr = match[target < 0 ? match.Count - 1 : target];
                        Event.current.Use();
                    }

                    if (Event.current.keyCode == KeyCode.Return)
                    {
                        if (selectedStr != "")
                        {
                            property.stringValue = selectedStr;
                            property.serializedObject.ApplyModifiedProperties();
                        }
                        EditorWindow.focusedWindow.Close();
                    }
                }

                if (Event.current.keyCode == KeyCode.Escape)
                {
                    EditorWindow.focusedWindow.Close();
                }
            }
            GUI.FocusControl(SEARCH_BOX_NAME);
            GUI.SetNextControlName(SEARCH_BOX_NAME);
            var prevST = searchText;
            searchText = GUI.TextField(line, searchText);
            Next();
            if (prevST != searchText)
            {
                match = availables.Where(i =>
                    {
                        int idx = 0;
                        foreach (var c in searchText)
                        {
                            var newIdx = i.IndexOf(c, idx);
                            if (newIdx < 0) return false;   // 没有查找到则不匹配
                            idx = newIdx + 1;               // 查找到则匹配，下一次从“匹配字符的下一位”开始检查
                        }
                        return true;
                    }).ToList();
                if (match.Count > 0) selectedStr = match[0];
                else selectedStr = "";
            }


            if (match.Count > 0)
            {
                line.height = lineHeight * (lineCount - 1) + spacing * (lineCount - 2);
                scroll = GUI.BeginScrollView(line, scroll, new Rect(0, 0, line.width - 20, match.Count * (lineHeight + spacing)));
                line.height = lineHeight;
                line.position = Vector2.zero;
                match.ForEach(i =>
                {
                    // if (line.Contains(Event.current.mousePosition) &&
                    //         (Event.current.type == EventType.MouseMove ||
                    //         Event.current.type == EventType.ScrollWheel))
                    //     selectedStr = i;

                    Selection(line, i);
                    Next();
                });
                GUI.EndScrollView();
            }
            else
            {
                selectedStr = "";
                LabelField(line, "没有匹配项");
            }
        }

        public override void OnClose()
        {
            GUI.SetNextControlName("");
        }

        private void Next()
        {
            line.y += lineHeight + spacing;
        }

        private void SelectionBox(Rect rect, Color tint)
        {
            Color c = GUI.color;
            GUI.color = tint;
            GUI.Box(rect, "");
            GUI.color = c;
        }

        private void Selection(Rect rect, string item)
        {
            var selected = item == selectedStr;
            SelectionBox(rect, selected ? Color.cyan : Color.white);
            if (selected) GUI.ScrollTo(rect);
            GUI.Label(rect, item);
        }
    }
}