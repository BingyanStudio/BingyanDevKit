using UnityEngine;
using System.Collections.Generic;

namespace Bingyan
{
    /// <summary>
    /// 用于对注册的 <see cref="IProcessable"/> 进行更新的处理器类。<br/>
    /// 核心思想是，并不是所有的组件/物体都需要用 Unity 自己的 Update 与 FixedUpdate。滥用这俩即消耗了更多的资源，又无法很好地控制他们的时间尺度。<br/>
    /// 现在，你可以让组件继承 <see cref="ProcessableMono"/>，通过重写 <see cref="ProcessableMono.Process(float)"/> 与 <see cref="ProcessableMono.PhysicsProcess(float)"/> 以更好地自定义它们在更新时的行为。<br/>
    /// 别忘了要让他们注册到一个 <see cref="Processer"/> 名下!
    /// </summary>
    public class Processer<T> : MonoBehaviour where T : IProcessable
    {
        protected RelayProcesser<T> processer = new();

        protected virtual void Update()
        {
            processer.Process(Time.deltaTime);
        }

        protected virtual void FixedUpdate()
        {
            processer.PhysicsProcess(Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// 泛用的 Processer，如果不想指定特定的类型就用它吧！
    /// </summary>
    public class DefaultProcesser : Processer<IProcessable> { }

    /// <summary>
    /// 中继处理器类<br/>
    /// 中继处理器并不会主动更新其子类，而是接受其他处理器的控制<br/>
    /// 例如，如果要实现【冻结敌人时间】的效果，则应当让所有敌人由一个中继处理器控制，在触发效果时将该处理器的时间尺度调为 0
    /// </summary>
    public class RelayProcesser<T> : Processable where T : IProcessable
    {
        /// <summary>
        /// 该处理器的时间尺度。这会影响所有他处理的物体的时间尺度。<br/>
        /// 妈妈再也不用担心我用 Time.timeScale = 0 把所有东西暂停了！
        /// </summary>
        public float TimeScale { get; set; } = 1;

        /// <summary>
        /// 当前注册的所有需更新的物体
        /// </summary>
        protected List<T> items = new();

        public override void Process(float delta)
        {
            items.ForEach(i => i?.Process(Time.deltaTime * TimeScale));
        }

        public override void PhysicsProcess(float delta)
        {
            items.ForEach(i => i?.PhysicsProcess(Time.fixedDeltaTime * TimeScale));
        }

        /// <summary>
        /// 将一个 <see cref="IProcessable"/> 注册过来，以对其进行更新
        /// </summary>
        /// <param name="item">要注册的物体</param>
        public virtual void Add(T item)
        {
            items.Add(item);
        }

        /// <summary>
        /// 取消注册某个 <see cref="IProcessable"/>
        /// </summary>
        /// <param name="item">要取消的物体</param>
        public virtual void Remove(T item)
        {
            items.Remove(item);
        }
    }

    /// <summary>
    /// 泛用的 RelayProcesser，如果不想指定类型就用它吧！
    /// </summary>
    public class DefaultRelayProcesser : RelayProcesser<IProcessable> { }
}