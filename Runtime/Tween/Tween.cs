using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bingyan
{
    /// <summary>
    /// 补间动画类
    /// <para>本质上来说，Tween干了三件事情: </para>
    /// <para>1. 存储了一个“动画进度值”: float timer</para>
    /// <para>2. 每一帧都按照一定的规则，将timer的值由0向1变化(可能某些设置会让他由1向0变化)</para>
    /// <para>3. timer的值变化后，会以之作为参数，触发代码回调，你可以依据这个 timer 的值来决定你的动画里控制的变量的值应该是多少</para>
    /// <para>----------</para>
    /// <para>例如，要做一个动画，使一个方块(对象名称为cube)的x坐标在5秒内增加3，你的代码应该是这样的: </para>
    /// <para>Tween.Linear(5).Process( () => {<br/>var origin = cube.transform.position; <br/>return timer => {<br/>var pos = cube.transform.position;<br/> pos.x = origin.x + 3 * timer;  // timer 是“进度”<br/>cube.transform.position = pos;<br/>}<br/>}<br/>).Play();</para>
    /// </summary>
    public class Tween
    {
        /// <summary>
        /// 是否打印额外的信息，如 Tween 对象的生成、回收等<br/>
        /// 对于莫名其妙不工作的情况非常有用
        /// </summary>
        public static bool Verbose { get; set; } = false;

        /// <summary>
        /// 所有 <see cref="Tween"/> 的时间尺度
        /// </summary>
        public static float TimeScale { get; set; } = 1;

        /// <summary>
        /// 所有 <see cref="Tween"/> 是否同时暂停?<br/>
        /// 对于游戏自己的暂停系统来说，修改这个值会比将 <see cref="TimeScale"/> 改为 0 更好一些
        /// </summary>
        public static bool GlobalPaused { get; set; } = false;

        internal static Stack<Tween> pool = new();

        static Tween()
        {
            var go = new GameObject("Tweener", typeof(Tweener));
            UnityEngine.Object.DontDestroyOnLoad(go);
        }

        /// <summary>
        /// 这个动画是否正在运行
        /// </summary>
        public bool Playing => Tweener.Instance.IsRunning(this);

        /// <summary>
        /// 这个动画是否已经暂停
        /// </summary>
        public bool Paused => paused;

        private ulong ID => Tweener.Instance.GetID(this);

        private Builder builder;

        private float timer = 0;
        private bool paused = false, pingpongFlag = false;
        private Action<float> processCbk;

        private void Init(Builder builder)
        {
            this.builder = builder;
            Reset();
        }

        private void Reset()
        {
            timer = 0;
            paused = false;
            pingpongFlag = false;
            processCbk = null;
        }

        /// <summary>
        /// 开始执行这个动画
        /// </summary>
        /// <param name="recycle">结束后，是否将其释放并回收到对象池</param>
        public TweenHandle Play()
        {
            if (Playing) return TweenHandle.Invalid;
            if (!IsValid())
            {
                if (Verbose) Debug.LogWarning("该对象已经被回收，而你仍然尝试播放它！");
                return TweenHandle.Invalid;
            }

            processCbk = builder.processCbkCreater?.Invoke();

            var id = Tweener.Instance.Register(this);
            return new(id);
        }

        private TweenHandle Play(ulong id)
        {
            if (Playing) return TweenHandle.Invalid;
            if (!IsValid())
            {
                if (Verbose) Debug.LogWarning("该对象已经被回收，而你仍然尝试播放它！");
                return TweenHandle.Invalid;
            }

            processCbk = builder.processCbkCreater?.Invoke();
            Tweener.Instance.Register(id, this);

            return new(id);
        }

        /// <summary>
        /// 暂停动画，只能在动画运行时调用
        /// </summary>
        internal void Pause()
        {
            if (!Playing) return;
            if (!IsValid())
            {
                if (Verbose) Debug.LogWarning("该对象已经被回收，而你仍然尝试暂停它！");
                return;
            }
            paused = true;
        }

        /// <summary>
        /// 继续动画，只能在动画运行时调用
        /// </summary>
        internal void Resume()
        {
            if (!Playing) return;
            if (!IsValid())
            {
                if (Verbose) Debug.LogWarning("该对象已经被回收，而你仍然尝试继续它！");
                return;
            }
            paused = false;
        }

        /// <summary>
        /// 停止执行这个动画
        /// </summary>
        /// <param name="recycle">停止后，是否将其释放并回收到对象池</param>
        public void Stop()
        {
            if (!IsValid())
            {
                if (Verbose) Debug.LogWarning("该对象已经被回收，而你仍然尝试停止它！");
                return;
            }

            if (Tweener.Instance.Remove(this))
            {
                builder.finallyCbk?.Invoke();
                Reset();
                pool.Push(this);
                if (Verbose) Debug.Log($"Tween 被回收: {GetHashCode()}");
            }
        }

        /// <summary>
        /// 当前 Tween 是否可用（没有被回收入对象池中）<br/>
        /// 如果你的代码里长期留存某个可能被回收的 Tween 对象，请务必检查这个
        /// </summary>
        public bool IsValid() => !pool.Contains(this);

        private void Finish()
        {
            builder.finishCbk?.Invoke();

            var id = ID;    // 保留上一次 ID
            Stop();

            builder.next?.Build().Play(id);
        }

        internal void Update(float delta)
        {
            if (!Playing || paused) return;
            if (GlobalPaused && !builder.unscaled) return;
            delta = builder.unscaled ? delta : delta * TimeScale;
            if (builder.limitDeltaTime) delta = Mathf.Min(delta, builder.maxDeltaTime);

            Update(delta, builder.pingpong && pingpongFlag);
            if (timer >= 1 - 1e-3) timer = 1;
            else if (timer <= 1e-3) timer = 0;

            processCbk?.Invoke(timer);

            if (builder.pingpong)
            {
                if (pingpongFlag && timer == 0)
                {
                    timer = 0;
                    pingpongFlag = false;

                    if (!builder.loop) Finish();
                }
                else if ((!pingpongFlag) && timer == 1)
                {
                    timer = 1;
                    pingpongFlag = true;
                }
            }
            else if (timer == 1)
            {
                if (!builder.loop) Finish();
                else timer = 0;
            }
        }

        private void Update(float delta, bool pingpong)
        {
            switch (builder.type)
            {
                case TweenType.Lerp:
                    if (delta > 0 || builder.unscaled)
                        timer = Mathf.Lerp(timer, pingpong ? 0 : 1, builder.arg);
                    break;

                case TweenType.Linear:
                    timer += (pingpong ? -1 : 1) * delta / builder.arg;
                    break;
            }
        }

        /// <summary>
        /// 以 Lerp 方式进行过渡
        /// <para>相当于每一帧调用一次 value = Mathf.Lerp(value, 1, speed)，然后将 value 作为<see cref="Builder.OnProcess(Action{float})"/>回调的输入值</para>
        /// </summary>
        /// <param name="speed">变换速度</param>
        /// <returns>用于配置这个Tween的Builder对象</returns>
        public static Builder Lerp(float speed = 0.1f) => new(TweenType.Lerp) { arg = speed };

        /// <summary>
        /// 以 线性 方式进行过渡
        /// <para>相当于每一帧调用一次 value += Time.deltaTime/length，然后将 value 作为<see cref="Builder.OnProcess(Action{float})"/>回调的输入值</para>
        /// </summary>
        /// <param name="length">总时长</param>
        /// <returns>用于配置这个Tween的Builder对象</returns>
        public static Builder Linear(float length = 0.1f) => new(TweenType.Linear) { arg = length };

        public class Builder
        {
            internal TweenType type;

            internal bool loop = false, pingpong = false, unscaled = false, limitDeltaTime = false;

            internal float arg, maxDeltaTime = -1;

            internal Func<Action<float>> processCbkCreater;
            internal Action finishCbk, finallyCbk;

            internal Builder next, previous;

            internal Builder(TweenType type)
            {
                this.type = type;
            }

            /// <summary>
            /// 依据这个 <see cref="Builder"/> 的配置创建 <see cref="Tween"/> 对象
            /// </summary>
            /// <returns>创建的 <see cref="Tween"/></returns>
            public Tween Build()
            {
                var b = this;
                while (b.previous != null)
                {
                    b = b.previous;
                    b.next.previous = null;
                }
                var t = pool.Count > 0 ? pool.Pop() : new Tween();
                if (Verbose) Debug.Log($"Tween 被获取: {t.GetHashCode()}");
                t.Init(b);
                return t;
            }

            /// <summary>
            /// 让动画循环。循环时，动画将永远不会结束，除非手动调用 tween.Stop(bool)
            /// </summary>
            /// <returns></returns>
            public Builder Loop()
            {
                loop = true;
                return this;
            }

            /// <summary>
            /// 让动画仅执行一次
            /// </summary>
            /// <returns></returns>
            public Builder Once()
            {
                loop = false;
                return this;
            }

            /// <summary>
            /// 让动画中的执行量从 0 变为 1，再变回 0 ，即让动画从头到尾再到头播放
            /// </summary>
            public Builder Pingpong()
            {
                pingpong = true;
                return this;
            }

            /// <summary>
            /// 使用未放缩的时间进行动画更新<br/>
            /// 即: 忽略 <see cref="Time.timeScale"/> 与 <see cref="TimeScale"/><br/>
            /// 在 <see cref="Time.timeScale"/> 设置为 0，但仍需执行动画的情况下很有用
            /// </summary>
            public Builder Unscaled()
            {
                unscaled = true;
                return this;
            }

            /// <summary>
            /// 限制单次更新最大的间隔时间"输入"<br/>
            /// 这主要用于切换场景后的第一帧: 由于加载场景时间较长，<see cref="Time.unscaledDeltaTime"/> 会给出一个很大的数字<br/>
            /// 此时若在 Awake 或 Start 里启动一个 Linear 类型的 <see cref="Tween"/>，则它会几乎立刻结束<br/>
            /// 因此，你可以使用这个方法来避免该情况
            /// </summary>
            /// <returns></returns>
            public Builder LimitDeltaTime(float max = 1f / 60)
            {
                limitDeltaTime = true;
                maxDeltaTime = max;
                return this;
            }

            /// <summary>
            /// 更新时的回调
            /// </summary>
            /// <param name="cbk">回调</param>
            public Builder Process(Action<float> cbk)
                => Process(() => cbk);

            /// <summary>
            /// 用于创建 “在动画运行时，依据给出的值进行更新的回调” 的回调
            /// <para>这个回调会在 Tween 对象调用 Play(bool) 方法时执行，并获取到一个 Action{float} 回调</para>
            /// <para>接着，这个 Action{float} 回调会在每一帧都执行，用于修改需要执行动画的物体的属性</para>
            /// <para>这样做的目的是，你可以读取到“这个动画开始时，要执行动画的物体”的初始属性，例如位置、大小等</para>
            /// <para>从而利用这个初始值来构建你的动画</para>
            /// </summary>
            /// <param name="cbk">回调</param>
            public Builder Process(Func<Action<float>> cbk)
            {
                processCbkCreater = cbk;
                return this;
            }

            /// <summary>
            /// 动画结束时的回调
            /// </summary>
            /// <param name="cbk">回调</param>
            public Builder Finish(Action cbk)
            {
                finishCbk = cbk;
                return this;
            }

            /// <summary>
            /// 动画最后的回调<br/>
            /// 与 <see cref="Finish"/> 不同，此处的回调会在中途停止时也执行一次
            /// </summary>
            /// <param name="cbk">回调</param>
            public Builder Finally(Action cbk)
            {
                finallyCbk = cbk;
                return this;
            }

            public Builder Next(Builder b)
            {
                next = b;
                b.previous = this;
                return b;
            }
        }
    }

    public readonly struct TweenHandle
    {
        public static readonly TweenHandle Invalid = new(ulong.MaxValue);

        private readonly ulong id;

        public TweenHandle(ulong id)
        {
            this.id = id;
        }

        public readonly void Stop()
        {
            if (Tweener.Instance.TryGet(id, out var t))
                t.Stop();
        }

        public readonly void Pause()
        {
            if (Tweener.Instance.TryGet(id, out var t))
                t.Pause();
        }

        public readonly void Resume()
        {
            if (Tweener.Instance.TryGet(id, out var t))
                t.Resume();
        }
    }

    /// <summary>
    /// 指定Tween过渡时，计算 timer 的算法的类型
    /// </summary>
    public enum TweenType
    {
        Lerp, Linear
    }
}