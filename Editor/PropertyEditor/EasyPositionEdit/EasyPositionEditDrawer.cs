using UnityEditor;
using UnityEngine;

namespace Bingyan.Editor
{
    /// <summary>
    /// Adds a button to the inspector that allows the user to drag the Vector3 position in the scene view
    /// </summary>
    [CustomPropertyDrawer(typeof(EasyPositionEditAttribute))]
    public class EasyPositionEditDrawer : PropertyDrawer
    {
        public override bool CanCacheInspectorGUI(SerializedProperty property) => true;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Vector3)
            {
                Debug.LogWarning("[Vector3 Drawer] You are not editing a Vector3 value!");
                return;
            }

            if (property.serializedObject.isEditingMultipleObjects)
            {
                GUI.Label(position, "Multiediting not supported");
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            var id = property.serializedObject.targetObject.GetInstanceID();
            var path = property.propertyPath;
            var positionType = ((EasyPositionEditAttribute)attribute).positionType;
            
            property.serializedObject.Update();

            // Rect for the next line
            var nextLineRect = position;
            nextLineRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            nextLineRect.height /= 2;

            if (!EasyPositionEditSceneMonitor.IsSelected(id, path))
            {
                // Original Vector3 value, written like this for consistency
                property.vector3Value = EditorGUI.Vector3Field(position, label, property.vector3Value);
                property.serializedObject.ApplyModifiedProperties();

                if (GUI.Button(nextLineRect, "Edit in Scene"))
                {
                    EasyPositionEditSceneMonitor.Select(id, path, positionType, property.vector3Value);
                    SceneView.RepaintAll();
                }
            }
            else
            {
                var originalEditedPos = EasyPositionEditSceneMonitor.GetPosition(id, path);
                var editedPos = EditorGUI.Vector3Field(position, label, originalEditedPos);
                EasyPositionEditSceneMonitor.SetPosition(id, path, editedPos);
                HandleUtility.Repaint();

                float elementWidth = nextLineRect.width / 4;
                var logRect = new Rect(nextLineRect.x, nextLineRect.y, elementWidth * 2, nextLineRect.height);
                var applyButtonRect = new Rect(nextLineRect.x + 2 * elementWidth, nextLineRect.y, elementWidth, nextLineRect.height);
                var cancelButtonRect = new Rect(nextLineRect.x + 3 * elementWidth, nextLineRect.y, elementWidth, nextLineRect.height);

                GUI.Label(logRect, $"Position type is: {positionType}");

                if (GUI.Button(applyButtonRect, "Apply"))
                {
                    property.vector3Value = EasyPositionEditSceneMonitor.GetPosition(id, path);
                    property.serializedObject.ApplyModifiedProperties();
                    EasyPositionEditSceneMonitor.Deselect(id, path);
                }

                if (GUI.Button(cancelButtonRect, "Cancel"))
                {
                    EasyPositionEditSceneMonitor.Deselect(id, path);
                }
            }
            EditorGUI.EndProperty();
        }
    }
}