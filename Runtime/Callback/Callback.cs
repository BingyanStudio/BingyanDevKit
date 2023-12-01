using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Bingyan
{
    /// <summary>
    /// 简单的回调封装类，用于与Unity引擎交互。
    /// <para>注意：此处的泛型和正常理解的不太一样，指的是 【若想要将某个方法注册给这个回调，则该方法需要具有的Attribute】，且Attribute必须为 <see cref="MethodDescripterAttribute"/> 的子类</para>
    /// <para>这样做的目的是，限制可选择注册的方法，降低策划的工作负担</para>
    /// <para>需要调用 <see cref="Callback.Init()" /> 进行初始化。如果不调用，在第一次执行回调的时候也会自动初始化，但是这样可能对性能有更多的影响（毕竟需要反射）。</para>
    /// <para>相关的编辑器拓展实现请参考 <see cref="CallbackDrawer"/></para>
    /// </summary>
    /// <typeparam name="T">若想要将某个方法注册给这个回调，则该方法需要具有的Attribute</typeparam>
    [Serializable]
    public class Callback<T> where T : MethodDescripterAttribute
    {
        [SerializeField, HideInInspector] private GameObject target;
        [SerializeField, HideInInspector] private Component comp;
        [SerializeField, HideInInspector] private string methodId;
        [SerializeField, HideInInspector] private List<Parameter> args;

        private MethodInfo mtd;
        private List<object> arguments = new List<object>();

        private bool inited = false;

        /// <summary>
        /// 初始化函数，至少需要执行一次才能正常运行回调。如果没有主动运行，则第一次执行 <see cref="Callback.Invoke"/> 时会自动执行一次
        /// <para>建议在最开始的时候执行一次，提高效率</para>
        /// </summary>
        public void Init()
        {
            if (inited) return;
            inited = true;

            if (methodId == "") return;
            mtd = comp.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        .First(i =>
                        {
                            var attr = i.GetCustomAttribute<T>();
                            return attr != null && attr.Id == methodId;
                        });
            if (mtd == null) Debug.LogError($"错误: 没有找到id为 {methodId} 的方法!");
            arguments = args.Select(i => i.Get()).ToList();
        }

        /// <summary>
        /// 调用方法!
        /// </summary>
        public void Invoke()
        {
            if (methodId == "") return;
            if (!inited) Init();
            mtd.Invoke(comp, arguments.ToArray());
        }
    }

    /// <summary>
    /// 用于存储绑定的方法的参数的通配数据结构
    /// <para>显然这是仔细阅读了Fungus代码的产物（</para>
    /// </summary>
    [Serializable]
    internal class Parameter
    {
        public string typeFullName;

        public int intVal;
        public float floatVal;
        public bool boolVal;
        public string strVal;
        public Vector2 vec2Val;
        public UnityEngine.Object objVal;

        public object Get()
        {
            switch (typeFullName)
            {
                case "System.Int32": return intVal;
                case "System.Boolean": return boolVal;
                case "System.Single": return floatVal;
                case "System.String": return strVal;
                case "UnityEngine.Vector2": return vec2Val;
                case "UnityEngine.Object": return objVal;
                default: return null;
            }
        }

        public void Set(Type type, object val)
        {
            typeFullName = type.FullName;
            switch (typeFullName)
            {
                case "System.Int32": intVal = (int)val; break;
                case "System.Boolean": boolVal = (bool)val; break;
                case "System.Single": floatVal = (float)val; break;
                case "System.String": strVal = (string)val; break;
                case "UnityEngine.Vector2": vec2Val = (Vector2)val; break;
                case "UnityEngine.Object": objVal = (UnityEngine.Object)val; break;
            }
        }
    }
}