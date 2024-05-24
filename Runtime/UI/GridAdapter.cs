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
        [SerializeField, Title("规则")] private Rule rule;

        private void Awake()
        {
            var grid = GetComponent<GridLayoutGroup>();
            switch (mode)
            {
                case Mode.Horizontal:
                    var width = (transform as RectTransform).rect.width;

                    switch (rule)
                    {
                        case Rule.Cell:
                            grid.cellSize = grid.cellSize.SetX((width - grid.padding.left - grid.padding.right - grid.spacing.x * (count - 1)) / count);
                            break;

                        case Rule.Spacing:
                            grid.spacing = grid.spacing.SetX((width - grid.padding.left - grid.padding.right - grid.cellSize.x * count) / (count - 1));
                            break;
                    }

                    break;

                case Mode.Vertical:
                    var height = (transform as RectTransform).rect.height;

                    switch (rule)
                    {
                        case Rule.Cell:
                            grid.cellSize = grid.cellSize.SetY((height - grid.padding.top - grid.padding.bottom - grid.spacing.y * (count - 1)) / count);
                            break;

                        case Rule.Spacing:
                            grid.spacing = grid.spacing.SetY((height - grid.padding.top - grid.padding.bottom - grid.cellSize.y * count) / (count - 1));
                            break;
                    }

                    break;
            }
        }

        public enum Mode
        {
            [InspectorName("限定列数")] Horizontal,
            [InspectorName("限定行数")] Vertical
        }

        public enum Rule
        {
            [InspectorName("拉伸单元格")] Cell,
            [InspectorName("拉伸间距")] Spacing
        }
    }
}