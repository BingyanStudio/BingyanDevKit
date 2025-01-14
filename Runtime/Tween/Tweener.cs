using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bingyan
{
    /// <summary>
    /// 执行Tween动画的组件<br/>
    /// 全自动运行，应该不需要获取到这个对象
    /// </summary>
    internal class Tweener : MonoBehaviour
    {
        internal const int UPDATE_RATE = 90;

        internal static ulong handleCounter = 0;

        internal static Tweener Instance { get; private set; }
        private readonly List<Tween> tweens = new();
        private readonly BiDictionary<Tween, ulong> tweenHandles = new();

        private float frameTime, frameTimer = 0;

        private void Awake()
        {
            Instance = this;
            frameTime = 1f / UPDATE_RATE;
        }

        private void Start()
        {
            SceneManager.sceneUnloaded += s =>
            {
                for (int i = tweens.Count - 1; i >= 0; i--)
                    tweens[i].Stop();
            };
        }

        private void Update()
        {
            frameTimer += Time.unscaledDeltaTime;
            if (frameTimer >= frameTime)
            {
                for (int i = tweens.Count - 1; i >= 0; i--)
                {
                    i = Mathf.Min(i, tweens.Count - 1);
                    tweens[i].Update(frameTimer);
                }
                frameTimer = 0;
            }
        }

        internal ulong Register(Tween t)
        {
            if (tweens.Contains(t)) return ulong.MaxValue;

            var id = handleCounter++;
            Register(id, t);
            return id;
        }

        internal void Register(ulong id, Tween t)
        {
            tweens.Add(t);
            tweenHandles[id] = t;
        }

        internal bool Remove(Tween t)
        {
            if (tweens.Contains(t))
            {
                tweens.Remove(t);
                tweenHandles.Remove(t);
                return true;
            }
            return false;
        }

        internal bool TryGet(ulong id, out Tween tween)
        {
            tween = null;
            return ulong.MaxValue != id && tweenHandles.TryGetValue(id, out tween);
        }

        internal bool IsRunning(Tween tween) => tweenHandles.ContainsKey(tween);
        internal ulong GetID(Tween tween) => tweenHandles.TryGetValue(tween, out var id) ? id : ulong.MaxValue;
    }
}