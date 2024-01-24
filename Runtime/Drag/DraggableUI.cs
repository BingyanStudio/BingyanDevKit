using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Bingyan
{
    /// <summary>
    /// 可拖拽物体的基类。继承这个类以迅速创建一个可以拖拽的物体。
    /// </summary>
    public abstract class DraggableUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        private bool dragging = false;
        private DroppableUI target;

        private GraphicRaycaster raycaster;
        private Vector2 originalPos;
        private Transform originalParent;
        private Transform canvasTr;

        protected virtual void Start()
        {
            raycaster = GetComponentInParent<GraphicRaycaster>();
            originalPos = GetDragObjTransform().position;
            originalParent = GetDragObjTransform().parent;
            canvasTr = GetComponentInParent<Canvas>().transform;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!dragging) return;
            GetDragObjTransform().position = eventData.position;

            var list = new List<RaycastResult>();
            raycaster.Raycast(eventData, list);

            var flag = false;
            var droppables = DroppableUI.GetDroppableObjs();

            if (droppables.Count == 0) return;
            if (list.Count == 0)
            {
                target?.DisplayDroppable();
                target = null;
                return;
            }

            foreach (var item in list)
            {
                if (!droppables.Contains(item.gameObject)) continue;

                var droppable = item.gameObject.GetComponent<DroppableUI>();
                if (!droppable.CanDrop(this)) continue;

                flag = true;    // 找到了一个，flag置true！
                if (target && target == droppable) continue;

                // target为null 或 target与droppable不相同
                target?.DisplayDroppable();
                target = droppable;
                target?.DisplayHighlight();
                break;
            }
            if (!flag)
            {
                target?.DisplayDroppable();
                target = null;
            }
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (!CanDrag()) return;
            dragging = true;

            // 设置到 Canvas 下，防止遮罩影响拖拽
            GetDragObjTransform().SetParent(canvasTr);

            DroppableUI.GetDroppables().ForEach(i =>
            {
                if (i.CanDrop(this))
                    i.DisplayDroppable();
            });
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (!dragging) return;
            dragging = false;

            OnEndDrag();

            if (target && OnDrop())
            {
                target.OnDrop(this);
                target = null;
            }
            else ResetDragObj();

            DroppableUI.GetDroppables().ForEach(i => i.DisplayNormal());
        }

        public void ResetDragObj()
        {
            var dragObj = GetDragObjTransform();
            dragObj.position = originalPos;
            dragObj.SetParent(canvasTr);
            dragObj.SetParent(originalParent);
        }

        /// <summary>
        /// 目前这个物体是否可以拖拽？这决定了用户尝试拖拽这个物体时，会不会有反应
        /// </summary>
        protected abstract bool CanDrag();

        /// <summary>
        /// 在即将放置时触发，你可以在这里返回 false 来阻止这一次放置
        /// </summary>
        /// <returns>是否可以放置</returns>
        protected abstract bool OnDrop();

        /// <summary>
        /// 获取需要被拖拽的物体的 Transform 组件<br/>
        /// 重写这个方法可以改变你实际拖拽的东西<br/>
        /// 例如，如果要实现【从苹果堆里拖拽出一个苹果】，则可以给【苹果堆】写一个组件并重写这个方法，并返回【单个苹果】的transform。详见飞书文档。
        /// </summary>
        protected virtual Transform GetDragObjTransform() => transform;

        /// <summary>
        /// 开始拖拽的回调，可以用来实现一些动画
        /// </summary>
        protected virtual void OnBeginDrag() { }

        /// <summary>
        /// 拖拽结束的回调，可以用来实现一些动画。无论在何种情况下结束拖拽，该回调均会触发。<br/>
        /// 触发时机早于 <see cref="OnDrop()"/>
        /// </summary>
        protected virtual void OnEndDrag() { }
    }
}