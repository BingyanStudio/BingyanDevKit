namespace Bingyan
{
    /// <summary>
    /// 使用新的更新函数的接口
    /// </summary>
    public interface IProcessable
    {
        /// <summary>
        /// 当这个物体被注册到某个 <see cref="Processer"/> 时，这个方法可以当做 Update 用。
        /// </summary>
        /// <param name="delta">经过的时间，等效于 Time.deltaTime，但受制于 <see cref="Processer.TimeScale"/></param>
        void Process(float delta);

        /// <summary>
        /// 当这个物体被注册到某个 <see cref="Processer"/> 时，这个方法可以当做 FixedUpdate 用。
        /// </summary>
        /// <param name="delta">经过的时间，等效于 Time.fixedDeltaTime，但受制于 <see cref="Processer.TimeScale"/></param>
        void PhysicsProcess(float delta);
    }
}