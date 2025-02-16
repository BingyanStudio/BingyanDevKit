using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Bingyan
{
    /// <summary>
    /// 物体槽基类。继承这个类以创建一个可以盛放 <see cref="DraggableUI"/> 的物体槽
    /// </summary>
    public abstract class DroppableUI : MonoBehaviour
    {
        private static readonly List<DroppableUI> droppables = new();
        public static List<DroppableUI> GetDroppables() => droppables;
        public static List<GameObject> GetDroppableObjs() => droppables.Select(i => i.gameObject).ToList();

        protected virtual void Awake()
        {
            droppables.Add(this);
        }

        protected virtual void OnDestroy()
        {
            droppables.Remove(this);
        }

        /// <summary>
        /// 当有其他组件放置到此处时触发<br/>
        /// 这个回调被触发，说明目前即将放置的物体是经过了<see cref="CanDrop(DraggableUI)"/> 检验的，不需要重复判断
        /// </summary>
        /// <param name="source">其他组件</param>
        public abstract void OnDrop(DraggableUI source);

        /// <summary>
        /// 判定一个物体能否被放置到这里
        /// </summary>
        /// <param name="source">那个UI</param>
        /// <returns>能否放置</returns>
        public abstract bool CanDrop(DraggableUI source);

        /// <summary>
        /// 展示正常状态，重写以添加动画、变颜色等实际效果
        /// </summary>
        public virtual void DisplayNormal() { }

        /// <summary>
        /// 展示【可放置】状态，重写以添加动画、变颜色等实际效果
        /// </summary>
        public virtual void DisplayDroppable() { }

        /// <summary>
        /// 展示【松手即可放置】状态，重写以添加动画、变颜色等实际效果
        /// </summary>
        public virtual void DisplayHighlight() { }
    }
}