using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Bingyan.Editor
{
    [InitializeOnLoad]
    public static class EasyPositionEditSceneMonitor
    {
        private class PositionInfo
        {
            public PositionType positionType;
            public Vector3 position;

            public PositionInfo(PositionType positionType, Vector3 position)
            {
                this.positionType = positionType;
                this.position = position;
            }
        }

        // Maintains a collection of positions that are currently being edited
        // Key consists of the instance ID, and the property path of the designated field
        private static Dictionary<(int, string), PositionInfo> editingInstances;

        static EasyPositionEditSceneMonitor()
        {
            editingInstances = new();
            SceneView.duringSceneGui += OnDuringSceneGUI;
        }

        /// <summary>
        /// Registers / Overwrites the field as currently being edited
        /// </summary>
        /// <param name="id">Instance ID of the component the field belongs to</param>
        /// <param name="path">Serialized property path of the field</param>
        /// <param name="positionType">The specified position type</param>
        /// <param name="position">Initial value of the field</param>
        internal static void Select(int id, string path, PositionType positionType, Vector3 position) => editingInstances[(id, path)] = new PositionInfo(positionType, position);

        /// <summary>
        /// Checks if a Vector3 field with the given ID and path is currently being edited
        /// </summary>
        /// <returns><see cref="bool"/>TRUE if the field is currently being edited</returns>
        internal static bool IsSelected(int id, string path) => editingInstances.ContainsKey((id, path));

        /// <summary>
        /// Retrieves the current value of the position with the given ID and path
        /// </summary>
        /// <returns><see cref="Vector3"/>The current position value</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the field is not currently being edited</exception>
        internal static Vector3 GetPosition(int id, string path) => editingInstances[(id, path)].position;
        
        /// <summary>
        /// Sets the position value of the field with the given ID and path
        /// </summary>
        /// <param name="position">The new position value, preferably entered by the user</param>
        /// <exception cref="KeyNotFoundException">Thrown when the field is not currently being edited</exception>
        internal static void SetPosition(int id, string path, Vector3 position) => editingInstances[(id, path)].position = position;

        /// <summary>
        /// Deselects the field with the given ID and path, removing it from the collection of fields
        /// </summary>
        internal static void Deselect(int id, string path)
        {
            editingInstances.Remove((id, path));
            SceneView.RepaintAll();
        }

        private static void OnDuringSceneGUI(SceneView sceneView)
        {
            foreach (var ((id, _), positionData) in editingInstances)
            {
                var target = ((Component)EditorUtility.InstanceIDToObject(id)).transform;
                if (target == null)
                {
                    Debug.LogWarning($"[Vector3 Scene Helper] Target with ID {id} is null!");
                    continue;
                }

                var rotation = positionData.positionType is PositionType.World or PositionType.WorldRelative ? Quaternion.identity : target.rotation;
                (var worldPosition, var label) = positionData.positionType switch
                {
                    PositionType.World => (positionData.position, "World Position"),
                    PositionType.Local => (target.TransformPoint(positionData.position), "Local Position"),
                    PositionType.WorldRelative => (target.position + positionData.position, "World Relative Position"),
                    PositionType.LocalRelative => (target.TransformPoint(target.localPosition + positionData.position), "Local Relative Position"),
                    _ => (Vector3.zero, "Unknown Position Type")
                };

                Handles.Label(worldPosition, label);
                var newPosition = Handles.PositionHandle(worldPosition, rotation);

                positionData.position = positionData.positionType switch
                {
                    PositionType.World => newPosition,
                    PositionType.Local => target.InverseTransformPoint(newPosition),
                    PositionType.WorldRelative => newPosition - target.position,
                    PositionType.LocalRelative => target.InverseTransformPoint(newPosition) - target.localPosition,
                    _ => Vector3.zero
                };
            }
            if (editingInstances.Count != 0) { SceneView.RepaintAll(); }
        }
    }
}