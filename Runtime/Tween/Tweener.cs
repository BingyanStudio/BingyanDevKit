using System.Collections.Generic;
using System.Security.Cryptography;
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
        internal static Tweener Instance { get; private set; }

        private List<Tween> tweens = new List<Tween>();

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            SceneManager.activeSceneChanged += (s1, s2) => StopAll();
        }

        private void Update()
        {
            for (int i = 0; i < tweens.Count; i++)
                tweens[i].Update(Time.deltaTime);
        }

        internal void Register(Tween t)
        {
            if (tweens.Contains(t)) return;
            tweens.Add(t);
        }

        internal void Remove(Tween t)
        {
            if (tweens.Contains(t)) tweens.Remove(t);
        }

        private void StopAll()
        {
            tweens.ForEach(i => i.Stop(true));
        }
    }
}