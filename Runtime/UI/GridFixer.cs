using UnityEngine;
using UnityEngine.UI;

namespace Bingyan
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class GridFixer : MonoBehaviour
    {
        [SerializeField, Title("固定行数")] private int row;
        [SerializeField, Title("固定列数")] private int column;

        private void Awake()
        {
            var grid = GetComponent<GridLayoutGroup>();

            var tr = transform.AsRectTransform();
            var width = tr.rect.width;
            var height = tr.rect.height;

            var spX = (width - column * grid.cellSize.x) / (column + 1);
            var spY = (height - row * grid.cellSize.y) / (row + 1);

            grid.spacing = new Vector2(spX, spY);

            var spXI = Mathf.FloorToInt(spX);
            var spYI = Mathf.FloorToInt(spY);
            grid.padding = new(spXI, spXI, spYI, spYI);
        }
    }
}
