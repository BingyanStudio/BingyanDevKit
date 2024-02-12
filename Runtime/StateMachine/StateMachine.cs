using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Text;

namespace Bingyan
{
    /// <summary>
    /// Mono 有限状态机<br/>
    /// 内部的状态由框架自动生成，你需要在泛型类 T 中提供你自己的状态基类<br/>
    /// 要指定初始状态，只需要在 Start 或 Awake 中调用 <see cref="ChangeState"/>
    /// </summary>
    /// <typeparam name="T">你的状态基类</typeparam>
    public abstract class FSM<T> : ProcessableMono where T : FSMState
    {
        [SerializeField, Title("自主运行")] private bool automatic = true;
        [SerializeField, Title("时间尺度")] private float timeScale = 1;

        public float TimeScale { get => timeScale; set => timeScale = value; }

        protected Dictionary<Type, T> states;
        public T CurrentState { get; private set; }

        protected virtual void Awake()
        {
            var stateTypes = typeof(T).Assembly.GetTypes().Where(i => i.IsSubclassOf(typeof(T)) && !i.IsAbstract);
            states = stateTypes.Select(i =>
            {
                var state = Activator.CreateInstance(i, this) as T;
                state.Init();
                return state;
            }).ToDictionary(i => i.GetType(), j => j);
        }

        private void Update()
        {
            if (automatic) Process(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            if (automatic) PhysicsProcess(Time.fixedDeltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            CurrentState?.OnTriggerEnter2D(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            CurrentState?.OnTriggerStay2D(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            CurrentState?.OnTriggerExit2D(other);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            CurrentState?.OnColliderEnter2D(other);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            CurrentState?.OnColliderStay2D(other);
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            CurrentState?.OnColliderExit2D(other);
        }

        public override void Process(float delta)
        {
            if (!CurrentState) return;

            delta *= timeScale;
            CurrentState.OnUpdate(delta);
        }

        public override void PhysicsProcess(float delta)
        {
            if (!CurrentState) return;

            delta *= timeScale;
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