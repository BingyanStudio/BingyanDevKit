using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Text;

namespace Bingyan
{
    /// <summary>
    /// Mono 有限状态机<br/>
    /// 要指定初始状态，只需要在 Start 或 Awake 中调用 <see cref="ChangeState"/>
    /// </summary>
    public abstract class FSM : ProcessableMono
    {
        [SerializeField, Title("使用 Unity Update")] protected bool automatic = true;
        [SerializeField, Title("时间尺度")] private float timeScale = 1;

        public float TimeScale { get => timeScale; set => timeScale = value; }

        protected Dictionary<Type, FSMState> states = new();
        public FSMState CurrentState { get; private set; }

        protected virtual void Awake()
        {
            if (automatic) Init();
        }

        public virtual void Init()
        {
            DefineStates();
            ChangeState(GetDefaultState());
        }

        /// <summary>
        /// 定义该 FSM 拥有的所有状态<br/>
        /// 使用 <see cref="AddState(FSMState)"/> 定义
        /// </summary>
        protected abstract void DefineStates();

        /// <summary>
        /// 定义该 FSM 的初始状态
        /// </summary>
        /// <returns>初始状态类型</returns>
        protected abstract Type GetDefaultState();

        protected virtual void Update()
        {
            if (automatic) Process(Time.deltaTime);
        }

        protected virtual void FixedUpdate()
        {
            if (automatic) PhysicsProcess(Time.fixedDeltaTime);
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            CurrentState?.OnTriggerEnter2D(other);
        }

        protected virtual void OnTriggerStay2D(Collider2D other)
        {
            CurrentState?.OnTriggerStay2D(other);
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            CurrentState?.OnTriggerExit2D(other);
        }

        protected virtual void OnCollisionEnter2D(Collision2D other)
        {
            CurrentState?.OnColliderEnter2D(other);
        }

        protected virtual void OnCollisionStay2D(Collision2D other)
        {
            CurrentState?.OnColliderStay2D(other);
        }

        protected virtual void OnCollisionExit2D(Collision2D other)
        {
            CurrentState?.OnColliderExit2D(other);
        }

        protected virtual void OnDrawGizmos()
        {
            CurrentState?.DrawGizmos();
        }

        protected virtual void OnDrawGizmosSelected()
        {
            CurrentState?.DrawGizmosSelected();
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
        /// 添加状态<br/>
        /// </summary>
        /// <param name="state">要添加的状态</param>
        protected void AddState(FSMState state)
        {
            var type = state.GetType();
            if (states.ContainsKey(type)) states[type] = state;
            else states.Add(type, state);
        }

        /// <summary>
        /// 切换状态<br/>
        /// S 为要切换的状态类型，在找不到时发出警告，并切换为【无状态】。
        /// </summary>
        /// <typeparam name="S">要切换的状态类型</typeparam>
        public void ChangeState<S>() where S : FSMState => ChangeState(typeof(S));

        public void ChangeState(Type state)
        {
            CurrentState?.OnExit();
            if (states.TryGetValue(state, out var value))
            {
                CurrentState = value;
                CurrentState.OnEnter();
            }
            else
            {
                CurrentState = null;

                var sb = new StringBuilder($"{name} 状态机内并不包含状态 {state}\n当前包含的状态有: ");
                foreach (var item in states.Keys) sb.AppendLine(item.ToString());
                Debug.LogWarning(sb.ToString());
            }
        }
    }
}