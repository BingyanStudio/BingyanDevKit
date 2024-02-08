using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Text;

namespace Bingyan
{
    /// <summary>
    /// 有限状态机<br/>
    /// 内部的状态由框架自动生成，你需要在泛型类 T 中提供你自己的状态基类<br/>
    /// 要指定初始状态，只需要在 Start 或 Awake 中调用 <see cref="ChangeState"/>
    /// </summary>
    /// <typeparam name="T">你的状态基类</typeparam>
    public class FSM<T> : ProcessableMono where T : FSMState
    {
        protected Dictionary<Type, T> states;
        public T CurrentState { get; private set; }

        public FSM()
        {
            var stateTypes = typeof(T).Assembly.GetTypes().Where(i => i.IsSubclassOf(typeof(T)) && !i.IsAbstract);
            states = stateTypes.Select(i => Activator.CreateInstance(i, this) as T)
                        .ToDictionary(i => i.GetType(), j => j);
        }

        public override void Process(float delta)
        {
            if (!CurrentState) return;
            CurrentState.OnUpdate(delta);
        }

        public override void PhysicsProcess(float delta)
        {
            if (!CurrentState) return;
            CurrentState.OnFixedUpdate(delta);
        }

        /// <summary>
        /// 切换状态<br/>
        /// S 为要切换的状态类型，在找不到时发出警告，并切换为【无状态】。
        /// </summary>
        /// <typeparam name="S">要切换的状态类型</typeparam>
        public void ChangeState<S>() where S : T
        {
            CurrentState?.OnExit();
            if (states.TryGetValue(typeof(S), out var value))
            {
                CurrentState = value;
                CurrentState.OnEnter();
            }
            else
            {
                CurrentState = null;

                var sb = new StringBuilder($"{name} 状态机内并不包含状态 {typeof(S)}\n当前包含的状态有: ");
                foreach (var item in states.Keys) sb.AppendLine(item.ToString());
                Debug.LogWarning(sb.ToString());
            }
        }
    }
}