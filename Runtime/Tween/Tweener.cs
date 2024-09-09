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

        internal static Tweener Instance { get; private set; }
        private List<Tween> tweens = new();

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
                    tweens[i].Stop(true);
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

        internal void Register(Tween t)
        {
            if (tweens.Contains(t)) return;
            tweens.Add(t);
        }

        internal bool Remove(Tween t)
        {
            if (tweens.Contains(t))
            {
                tweens.Remove(t);
                return true;
            }
            return false;
        }
    }
}