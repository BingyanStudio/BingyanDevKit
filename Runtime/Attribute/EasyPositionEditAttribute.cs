using System;

using UnityEngine;

namespace Bingyan
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
    public class EasyPositionEditAttribute : PropertyAttribute
    {
        public readonly PositionType positionType;

        public EasyPositionEditAttribute(PositionType positionType = PositionType.World)
        {
            this.positionType = positionType;
        }
    }
}