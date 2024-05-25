using System;
using System.Collections.Generic;
using Bingyan;

/// <summary>
/// 线性流程<br/>
/// <see cref="Flow"/> 允许你通过链式调用，配置一连串需要执行的代码<br/>
/// 这些代码可以以任意你喜欢的方式进行延迟执行！
/// </summary>
public class Flow
{
    private static Stack<Flow> pool = new();

    private readonly Handle handle;
    private readonly List<Action<Handle>> actions = new();
    private Action<Exception> exceptAction;
    private int idx = 0;

    /// <summary>
    /// 创建一个新的流程
    /// </summary>
    /// <returns>流程 <see cref="Flow"/> 实例</returns>
    public static Flow Create() => pool.Count > 0 ? pool.Pop() : new();

    private Flow()
    {
        handle = new(this);
    }

    /// <summary>
    /// 指定下一步执行的回调<br/>
    /// 当回调执行完毕后，需要使用传入的 <see cref="Handle"/> 实例<br/>
    /// 调用 <see cref="Handle.Continue"/> 以继续当前流程
    /// </summary>
    /// <param name="action">回调</param>
    /// <returns>同一个 <see cref="Flow"/> 对象，以链式调用</returns>
    public Flow Then(Action<Handle> action)
    {
        actions.Add(action);
        return this;
    }

    /// <summary>
    /// 指定下一步执行的回调<br/>
    /// 该回调执行完毕后，将自动继续当前流程
    /// </summary>
    /// <param name="action">回调</param>
    /// <returns>同一个 <see cref="Flow"/> 对象，以链式调用</returns>
    public Flow Then(Action action) => Then(h => { action?.Invoke(); h.Continue(); });

    /// <summary>
    /// 指定下一步执行的回调<br/>
    /// 需要传入一个输入 <see cref="Flow"/> ，对其进行处理后返回相同 <see cref="Flow"/> 的方法<br/>
    /// 以实现管线处理风格的代码
    /// </summary>
    /// <param name="action">回调</param>
    /// <returns>同一个 <see cref="Flow"/> 对象，以链式调用</returns>
    public Flow Then(Func<Flow, Flow> pipeline)
        => pipeline.Invoke(this);

    /// <summary>
    /// 指定下一步执行的回调<br/>
    /// 需要传入一个 <see cref="Tween"/> , 流程将在 <see cref="Tween"/> 执行完毕后继续
    /// </summary>
    /// <param name="action">回调</param>
    /// <returns>同一个 <see cref="Flow"/> 对象，以链式调用</returns>
    public Flow Then(Tween.Builder tween)
        => Then(h =>
                {
                    tween.finallyCbk += h.Continue;
                    tween.Build()
                        .Play();
                }
            );

    /// <summary>
    /// 指定发生错误时，执行的回调<br/>
    /// 注意：一个 <see cref="Flow"/> 对象只能有一个错误处理回调，多次指定将取最后一个
    /// </summary>
    /// <param name="exceptAction">错误处理回调</param>
    /// <returns>同一个 <see cref="Flow"/> 对象</returns>
    public Flow Except(Action<Exception> exceptAction)
    {
        this.exceptAction = exceptAction;
        return this;
    }

    /// <summary>
    /// 将流程延迟一段时间
    /// </summary>
    /// <param name="length">延迟时长</param>
    /// <returns>同一个 <see cref="Flow"/> 对象</returns>
    public Flow Delay(float length)
        => Then(Tween.Linear(length).LimitDeltaTime());

    /// <summary>
    /// 将流程延迟一段时间，无视 <see cref="UnityEngine.Time.timeScale"/>
    /// </summary>
    /// <param name="length">延迟时长</param>
    /// <returns>同一个 <see cref="Flow"/> 对象</returns>
    public Flow DelayUnscaled(float length)
        => Then(Tween.Linear(length).LimitDeltaTime().Unscaled());

    /// <summary>
    /// 开始执行整个流程
    /// </summary>
    public void Run()
    {
        idx = 0;
        Exec();
    }

    private void Exec()
    {
        if (idx >= actions.Count)
        {
            Release();
            return;
        }

        try
        {
            actions[idx].Invoke(handle);
        }
        catch (Exception e)
        {
            _Except(e);
        }
    }

    private void Continue()
    {
        idx++;
        Exec();
    }

    private void _Except(Exception e)
    {
        exceptAction?.Invoke(e);
        Release();
    }

    private void Release()
    {
        actions.Clear();
        exceptAction = null;
        pool.Push(this);
    }

    /// <summary>
    /// 流程控制类<br/>
    /// 用于在编写流程的过程中，控制流程继续，或抛出异常
    /// </summary>
    public class Handle
    {
        private readonly Flow flow;

        internal Handle(Flow flow)
        {
            this.flow = flow;
        }

        /// <summary>
        /// 让当前流程继续执行
        /// </summary>
        public void Continue()
        {
            flow.Continue();
        }

        /// <summary>
        /// 停止当前流程并抛出异常
        /// </summary>
        /// <param name="e">异常</param>
        public void Except(Exception e)
        {
            flow._Except(e);
        }
    }
}
