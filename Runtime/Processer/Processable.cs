namespace Bingyan
{
    /// <summary>
    /// 使用了新更新方法的物体
    /// </summary>
    public class Processable : IProcessable
    {
        public virtual void PhysicsProcess(float delta) { }
        public virtual void Process(float delta) { }
    }
}