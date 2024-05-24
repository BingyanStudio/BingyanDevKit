using System;
using System.Collections.Generic;
using Bingyan;

public class Flow
{
    private static Stack<Flow> pool = new();

    private readonly Handle handle;
    private readonly List<Action<Handle>> actions = new();
    private Action<Exception> exceptAction;
    private int idx = 0;

    public static Flow Create() => pool.Count > 0 ? pool.Pop() : new();

    private Flow()
    {
        handle = new(this);
    }

    public Flow Then(Action<Handle> action)
    {
        actions.Add(action);
        return this;
    }

    public Flow Then(Action action) => Then(h => { action?.Invoke(); h.Continue(); });

    public Flow Then(Func<Flow, Flow> pipeline)
        => pipeline.Invoke(this);

    public Flow Then(Tween.Builder tween)
        => Then(h =>
                {
                    tween.finallyCbk += h.Continue;
                    tween.Build()
                        .Play();
                }
            );

    public Flow Except(Action<Exception> exceptAction)
    {
        this.exceptAction = exceptAction;
        return this;
    }

    public Flow Delay(float length)
        => Then(h => Tween.Linear(length).LimitDeltaTime()
                            .Finally(() => h.Continue())
                            .Build()
                            .Play());

    public Flow DelayUnscaled(float length)
                => Then(h => Tween.Linear(length).LimitDeltaTime().Unscaled()
                            .Finally(() => h.Continue())
                            .Build()
                            .Play());

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
        pool.Push(this);
    }

    private void Release()
    {
        actions.Clear();
        exceptAction = null;
        pool.Push(this);
    }

    public class Handle
    {
        private readonly Flow flow;

        public Handle(Flow flow)
        {
            this.flow = flow;
        }

        public void Continue()
        {
            flow.Continue();
        }

        public void Except(Exception e)
        {
            flow._Except(e);
        }
    }
}
