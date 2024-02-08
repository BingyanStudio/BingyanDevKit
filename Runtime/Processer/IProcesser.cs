using System.Collections.Generic;

namespace Bingyan
{
    public interface IProcesser<T> where T : IProcessable
    {
        /// <summary>
        /// 该处理器的时间尺度。这会影响所有他处理的物体的时间尺度。<br/>
        /// 妈妈再也不用担心我用 Time.timeScale = 0 把所有东西暂停了！
        /// </summary>
        float TimeScale { get; set; }

        /// <summary>
        /// 将一个 <see cref="IProcessable"/> 注册过来，以对其进行更新
        /// </summary>
        /// <param name="item">要注册的物体</param>
        void Add(T item);

        /// <summary>
        /// 取消注册某个 <see cref="IProcessable"/>
        /// </summary>
        /// <param name="item">要取消的物体</param>
        void Remove(T item);
    }
}