using UnityEngine;
using UnityEngine.UI;

public class GridAutoScaler : MonoBehaviour
{
    public GridLayoutGroup grid;
    public RectTransform gridRect;
    public int rows = 2;
    public int cols = 2;
    public Vector2 cellSize = new Vector2(115, 181);
    public Vector2 screenSize = new Vector2(1920, 1080);

    void Start()
    {
        ApplyLayout();
    }

    public void ApplyLayout()
    {
        float totalWidth = cols * cellSize.x;
        float totalHeight = rows * cellSize.y;

        float horizontalSpacing = (screenSize.x - totalWidth) / (cols + 1);
        float verticalSpacing = (screenSize.y - totalHeight) / (rows + 1);

        // Set Grid Layout Group
        grid.cellSize = cellSize;
        grid.spacing = new Vector2(horizontalSpacing, verticalSpacing);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = cols;
        grid.childAlignment = TextAnchor.UpperLeft;

        // Resize grid container to fit content
        float layoutWidth = totalWidth + horizontalSpacing * (cols - 1);
        float layoutHeight = totalHeight + verticalSpacing * (rows - 1);

        gridRect.sizeDelta = new Vector2(layoutWidth, layoutHeight);
        gridRect.anchoredPosition = Vector2.zero;
    }

    public void SetGridSize(int newRows, int newCols)
    {
        rows = newRows;
        cols = newCols;
        ApplyLayout();
    }
}