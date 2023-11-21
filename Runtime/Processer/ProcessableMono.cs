using UnityEngine;

namespace Bingyan
{
    /// <summary>
    /// 使用了新更新方法的组件基类<br/>
    /// 都继承这个了，就别再用 Update 和 FixedUpdate 了罢（
    /// </summary>
    public class ProcessableMono : MonoBehaviour, IProcessable
    {
        public virtual void Process(float delta) { }
        public virtual void PhysicsProcess(float delta) { }
    }
}