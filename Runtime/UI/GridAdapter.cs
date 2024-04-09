using UnityEngine;
using UnityEngine.UI;

namespace Bingyan
{
    /// <summary>
    /// 网格布局的自动适配器<br/>
    /// 挂载后，依据模式不同，将会限制同一行/列上的最大元素数量<br/>
    /// 其中，每一个元素都会尽可能地填满空隙，直到被总空间与间隔限制
    /// </summary>
    [RequireComponent(typeof(GridLayoutGroup))]
    public class GridAdapter : MonoBehaviour
    {
        [SerializeField, Title("模式")] private Mode mode;
        [SerializeField, Title("数量")] private int count;

        private void Awake()
        {
            var grid = GetComponent<GridLayoutGroup>();
            switch (mode)
            {
                case Mode.Horizontal:
                    var width = (transform as RectTransform).rect.width;
                    grid.cellSize = grid.cellSize.SetX((width - grid.padding.left - grid.padding.right - grid.spacing.x * (count - 1)) / count);
                    break;

                case Mode.Vertical:
                    var height = (transform as RectTransform).rect.height;
                    grid.cellSize = grid.cellSize.SetY((height - grid.padding.top - grid.padding.bottom - grid.spacing.y * (count - 1)) / count);
                    break;
            }
        }

        public enum Mode
        {
            [InspectorName("限定列数")] Horizontal,
            [InspectorName("限定行数")] Vertical
        }
    }
}