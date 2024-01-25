using System;
using UnityEngine;
using UnityEngine.Pool;

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
        internal static ObjectPool<Tween> pool = new(() => new Tween(), t => t.Reset());

        static Tween()
        {
            var go = new GameObject("Tweener", typeof(Tweener));
            UnityEngine.Object.DontDestroyOnLoad(go);
        }

        /// <summary>
        /// 这个动画是否正在运行
        /// </summary>
        public bool Running => running;

        /// <summary>
        /// 这个动画是否已经暂停
        /// </summary>
        public bool Paused => paused;

        private Builder builder;

        private float timer = 0;
        private bool running = false, paused = false, pingpongFlag = false, recycleOnFinish = false;
        private Action<float> processCbk;

        private void Init(Builder builder)
        {
            this.builder = builder;
            Reset();
        }

        private void Reset()
        {
            timer = 0;
            running = false;
            paused = false;
            pingpongFlag = false;
            recycleOnFinish = false;
            processCbk = null;
        }

        /// <summary>
        /// 开始执行这个动画
        /// </summary>
        /// <param name="recycle">结束后，是否将其释放并回收到对象池</param>
        public void Play(bool recycle = true)
        {
            if (running) return;
            running = true;
            recycleOnFinish = recycle;

            processCbk = builder.processCbkCreater?.Invoke();

            Tweener.Instance.Register(this);
        }

        /// <summary>
        /// 暂停动画，只能在动画运行时调用
        /// </summary>
        public void Pause()
        {
            if (!running) return;
            paused = true;
        }

        /// <summary>
        /// 继续动画，只能在动画运行时调用
        /// </summary>
        public void Resume()
        {
            if (!running) return;
            paused = false;
        }

        /// <summary>
        /// 停止执行这个动画
        /// </summary>
        /// <param name="recycle">停止后，是否将其释放并回收到对象池</param>
        public void Stop(bool recycle = false)
        {
            if (Tweener.Instance.Remove(this))
            {
                Reset();
                if (recycle) pool.Release(this);
            }
        }

        private void Finish()
        {
            builder.finishCbk?.Invoke();
            Stop(recycleOnFinish);

            builder.next?.Build().Play(recycleOnFinish);
        }

        internal void Update(float delta)
        {
            if (!running || paused) return;

            if (!builder.pingpong) NormalUpdate(delta);
            else PingpongUpdate(delta);
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

        private void NormalUpdate(float delta)
        {
            switch (builder.type)
            {
                case TweenType.Lerp:
                    timer = Mathf.Lerp(timer, 1, builder.lerpSpeed);
                    break;

                case TweenType.Linear:
                    timer += delta / builder.linearTime;
                    break;
            }
        }

        private void PingpongUpdate(float delta)
        {
            switch (builder.type)
            {
                case TweenType.Lerp:
                    timer = Mathf.Lerp(timer, pingpongFlag ? 0 : 1, builder.lerpSpeed);
                    break;

                case TweenType.Linear:
                    timer += (pingpongFlag ? -1 : 1) * delta / builder.linearTime;
                    break;
            }
        }

        /// <summary>
        /// 以 Lerp 方式进行过渡
        /// <para>相当于每一帧调用一次 value = Mathf.Lerp(value, 1, speed)，然后将 value 作为<see cref="Builder.OnProcess(Action{float})"/>回调的输入值</para>
        /// </summary>
        /// <param name="speed">变换速度</param>
        /// <returns>用于配置这个Tween的Builder对象</returns>
        public static Builder Lerp(float speed = 0.1f) => new(TweenType.Lerp) { lerpSpeed = speed };

        /// <summary>
        /// 以 线性 方式进行过渡
        /// <para>相当于每一帧调用一次 value += Time.deltaTime/length，然后将 value 作为<see cref="Builder.OnProcess(Action{float})"/>回调的输入值</para>
        /// </summary>
        /// <param name="length">总时长</param>
        /// <returns>用于配置这个Tween的Builder对象</returns>
        public static Builder Linear(float length = 0.1f) => new(TweenType.Linear) { linearTime = length };

        public class Builder
        {
            internal TweenType type;

            internal bool loop = false, pingpong = false;

            internal float lerpSpeed;
            internal float linearTime;

            internal Func<Action<float>> processCbkCreater;
            internal Action finishCbk;

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
                var t = pool.Get();
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
            /// <returns></returns>
            public Builder Pingpong()
            {
                pingpong = true;
                return this;
            }

            /// <summary>
            /// 用于创建 “在动画运行时，依据给出的值进行更新的回调” 的回调
            /// <para>这个回调会在 Tween 对象调用 Play(bool) 方法时执行，并获取到一个 Action{float} 回调</para>
            /// <para>接着，这个 Action{float} 回调会在每一帧都执行，用于修改需要执行动画的物体的属性</para>
            /// <para>这样做的目的是，你可以读取到“这个动画开始时，要执行动画的物体”的初始属性，例如位置、大小等</para>
            /// <para>从而利用这个初始值来构建你的动画</para>
            /// </summary>
            /// <param name="cbk">回调</param>
            /// <returns></returns>
            public Builder Process(Func<Action<float>> cbk)
            {
                processCbkCreater = cbk;
                return this;
            }

            /// <summary>
            /// 动画结束时的回调
            /// </summary>
            /// <param name="cbk">回调</param>
            /// <returns></returns>
            public Builder Finish(Action cbk)
            {
                finishCbk = cbk;
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

    /// <summary>
    /// 指定Tween过渡时，计算 timer 的算法的类型
    /// </summary>
    public enum TweenType
    {
        Lerp, Linear
    }
}