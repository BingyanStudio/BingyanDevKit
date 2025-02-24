using UnityEditor;
using UnityEngine;

namespace Bingyan.Editor
{
    public static class EditorList
    {
        private const int ARR_SIZE_FIELD_WIDTH = 48;
        private const int LIST_PADDING = 5;
        private const int HANDLE_WIDTH = 10;
        private const int FIELD_MARGIN = 26;
        private const int FOOTER_WIDTH = 60;
        private const int FOOTER_OFFSET = 10;
        private const int FOOTER_PADDING = 4;

        private static readonly Color colorSelection = ColorUtils.FromHex("#2c5d87");

        private static readonly GUIStyle styleRLHeader = "RL Empty Header";
        private static readonly GUIStyle styleRLBackground = "RL Background";
        private static readonly GUIStyle styleRLFooter = "RL Footer";
        private static readonly GUIStyle styleRLFooterBtn = "RL FooterButton";

        private static readonly GUIContent icoCreate = new(EditorGUIUtility.IconContent("d_Toolbar Plus@2x"));
        private static readonly GUIContent icoRemove = new(EditorGUIUtility.IconContent("d_Toolbar Minus@2x"));

        private static float LineHeight => EditorGUIUtility.singleLineHeight;
        private static float Spacing => EditorGUIUtility.standardVerticalSpacing;

        public static Rect Draw(Rect pos, SerializedProperty property, string label, ref State state)
        {
            bool btnAddClicked, btnRemoveClicked;

            pos.height = LineHeight;

            // 折叠框
            state.Foldout = !EditorGUI.BeginFoldoutHeaderGroup(pos, !state.Foldout, label);
            property.arraySize = EditorGUI.DelayedIntField(new(pos.x + pos.width - ARR_SIZE_FIELD_WIDTH, pos.y, ARR_SIZE_FIELD_WIDTH, pos.height), property.arraySize);
            EditorGUI.EndFoldoutHeaderGroup();

            if (state.Foldout) return Next(pos);
            else pos = Next(pos);

            // 表头
            GUI.Box(new(pos.x, pos.y, pos.width, LIST_PADDING), "", styleRLHeader);
            var insidePos = new Rect(pos.x + LIST_PADDING, pos.y + LIST_PADDING, pos.width - 2 * LIST_PADDING, pos.height);

            // 空列表
            if (property.arraySize == 0)
            {
                GUI.Box(new(pos.x, pos.y + LIST_PADDING, pos.width, LineHeight + LIST_PADDING), "", styleRLBackground);
                GUI.Label(insidePos, "列表里没有任何内容！");
                insidePos = Next(insidePos);
                state.Selected = -1;
            }
            else
            {
                var rects = new Rect[property.arraySize];
                var height = 0f;
                for (int i = 0; i < property.arraySize; i++)
                {
                    var h = EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(i));
                    rects[i] = new Rect(insidePos.x - LIST_PADDING / 2, insidePos.y + height - Spacing / 2, insidePos.width + LIST_PADDING, h + Spacing);
                    height += h + Spacing;

                    if (Event.current == null) continue;
                    var e = Event.current;

                    if (e.type == EventType.MouseDown
                        && rects[i].Contains(e.mousePosition))
                    {
                        state.MouseDown = true;
                        state.Selected = i;
                        state.MouseRelatedPosition = e.mousePosition - rects[i].position;
                    }
                    else if (e.type == EventType.MouseDrag)
                    {
                        if (!state.Dragging)
                        {
                            if (state.MouseDown && (state.MouseOrigin - e.mousePosition).magnitude > 1e-1) state.Dragging = true;
                            else continue;
                        }
                        var rect = rects[i];
                        rect.height = LineHeight;
                        if (rect.Contains(e.mousePosition))
                            state.TargetIndex = i;
                    }
                }

                if (Event.current != null && Event.current.type == EventType.MouseUp)
                {
                    if (state.Dragging)
                    {
                        property.MoveArrayElement(state.Selected, state.TargetIndex);
                        state.Selected = state.TargetIndex;
                    }

                    state.MouseDown = false;
                    state.Dragging = false;
                }

                GUI.Box(new(pos.x, pos.y + LIST_PADDING, pos.width, height - Spacing + LIST_PADDING), "", styleRLBackground);
                if (state.Selected >= property.arraySize) state.Selected = -1;

                void DrawElement(Rect rect, int index, State state)
                {
                    if (index == state.Selected)
                        EditorGUI.DrawRect(rect, colorSelection);

                    rect.x += LIST_PADDING / 2;
                    rect.y += Spacing / 2;
                    rect.width -= LIST_PADDING;
                    rect.height -= Spacing;

                    GUI.Box(new(rect.x + 2, rect.y + LineHeight * 0.4f, HANDLE_WIDTH, rect.height), "", "RL DragHandle");
                    EditorGUI.PropertyField(
                        new(rect.x + FIELD_MARGIN + HANDLE_WIDTH, rect.y, rect.width - FIELD_MARGIN - HANDLE_WIDTH, rect.height),
                        property.GetArrayElementAtIndex(index),
                        EditorGUIUtility.TrTempContent($"元素 {index}"),
                        true);
                }

                for (int i = 0; i < property.arraySize; i++)
                {
                    var rect = rects[i];
                    if (state.Dragging)
                    {
                        if (state.Selected == i) continue;
                        else if (state.Selected > state.TargetIndex
                                    && i >= state.TargetIndex
                                    && i < state.Selected) rect.y += rects[state.Selected].height;
                        else if (state.Selected < state.TargetIndex
                                    && i > state.Selected
                                    && i <= state.TargetIndex) rect.y -= rects[state.Selected].height;
                    }
                    DrawElement(rect, i, state);
                }

                if (state.Dragging && Event.current != null)
                    DrawElement(new(rects[0].x,
                                    Event.current.mousePosition.y - state.MouseRelatedPosition.y,
                                    rects[0].width,
                                    EditorGUI.GetPropertyHeight(
                                        property.GetArrayElementAtIndex(state.Selected)
                                    ) + Spacing),
                                state.Selected,
                                state);

                insidePos.y += height;
            }

            var footerPos = new Rect(pos.x + pos.width - FOOTER_OFFSET - FOOTER_WIDTH,
                                    insidePos.y + LIST_PADDING - Spacing,
                                    FOOTER_WIDTH, pos.height);

            // 底部按钮
            GUI.Box(footerPos, "", styleRLFooter);
            btnAddClicked = GUI.Button(new(footerPos.x + FOOTER_PADDING, footerPos.y, FOOTER_WIDTH / 2 - FOOTER_PADDING * 3 / 2, footerPos.height), icoCreate, styleRLFooterBtn);

            GUI.enabled = property.arraySize > 0;
            btnRemoveClicked = GUI.Button(new(footerPos.x + FOOTER_WIDTH / 2 + FOOTER_PADDING / 2, footerPos.y, FOOTER_WIDTH / 2 - FOOTER_PADDING * 3 / 2, footerPos.height), icoRemove, styleRLFooterBtn);
            GUI.enabled = true;

            if (btnAddClicked) property.arraySize++;
            if (btnRemoveClicked)
            {
                if (state.Selected >= 0 && state.Selected < property.arraySize) property.DeleteArrayElementAtIndex(state.Selected);
                else property.arraySize--;
                state.Selected = -1;
            }

            return new(pos.x, footerPos.y + LineHeight + 2 * Spacing, pos.width, LineHeight);
        }

        public static float GetElementsHeight(SerializedProperty property)
        {
            var height = 0f;
            for (int i = 0; i < property.arraySize; i++)
            {
                height += EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(i));
                height += Spacing;
            }
            return height - Spacing;
        }

        public static float GetHeight(SerializedProperty property, State state)
        {
            if(state.Foldout) return LineHeight;
            if (property.arraySize == 0) return 3 * LineHeight + 3 * Spacing + 2 * LIST_PADDING;
            return 2 * LineHeight
                    + 3 * Spacing
                    + 2 * LIST_PADDING
                    + GetElementsHeight(property);
        }

        private static Rect Next(Rect prev)
        {
            prev.y += LineHeight + Spacing;
            return prev;
        }

        public struct State
        {
            public bool Foldout;
            internal int Selected;

            internal bool MouseDown;
            internal bool Dragging;
            internal Vector2 MouseOrigin;
            internal Vector2 MouseRelatedPosition;
            internal int TargetIndex;
        }
    }
}