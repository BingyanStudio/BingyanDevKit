using UnityEngine;

namespace Bingyan
{
    /// <summary>
    /// 有限状态机的状态基类<br/>
    /// 继承这个类以生成自己的状态基类，再从自己的状态基类上拓展出具有实际功能的状态<br/>
    /// </summary>
    public abstract class FSMState
    {
        protected ProcessableFSM Host;

        protected FSMState(ProcessableFSM host)
        {
            Host = host;
        }

        public virtual void Init() { }
        public virtual void OnEnter() { }
        public virtual void OnUpdate(float delta) { }
        public virtual void OnFixedUpdate(float delta) { }
        public virtual void OnTriggerEnter2D(Collider2D col) { }
        public virtual void OnTriggerStay2D(Collider2D col) { }
        public virtual void OnTriggerExit2D(Collider2D col) { }
        public virtual void OnColliderEnter2D(Collision2D col) { }
        public virtual void OnColliderStay2D(Collision2D col) { }
        public virtual void OnColliderExit2D(Collision2D col) { }
        public virtual void OnExit() { }

        public virtual void OnDrawGizmos() { }
        public virtual void OnDrawGizmosSelected() { }

        public static implicit operator bool(FSMState self) => self != null;
    }
}
