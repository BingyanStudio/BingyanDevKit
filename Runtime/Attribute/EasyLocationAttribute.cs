using System;

using UnityEngine;

namespace Bingyan.EasyEdit
{
    public enum PositionType
    {
        World,
        Local,
        WorldRelative,
        LocalRelative
    }
    /// <summary>
    /// Use this attribute on a Vector3 field to allow the user to drag the position in the scene view
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class EasyLocationAttribute : PropertyAttribute
    {
        public readonly PositionType positionType;

        public EasyLocationAttribute(PositionType positionType = PositionType.World)
        {
            this.positionType = positionType;
        }
    }
}