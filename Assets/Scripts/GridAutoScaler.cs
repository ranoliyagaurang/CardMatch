using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the automatic scaling and layout of a grid-based UI system.
/// Handles dynamic resizing of grid cells and spacing based on screen dimensions.
/// </summary>
public class GridAutoScaler : MonoBehaviour
{
    [Header("Grid Components")]
    [SerializeField] private GridLayoutGroup grid;
    [SerializeField] private RectTransform gridRect;

    [Header("Grid Configuration")]
    [SerializeField] private int rows = 2;
    [SerializeField] private int cols = 2;
    [SerializeField] private Vector2 cellSize = new Vector2(115, 181);
    [SerializeField] private Vector2 screenSize = new Vector2(1920, 1080);

    /// <summary>
    /// Initializes the grid layout when the component starts.
    /// </summary>
    private void Start()
    {
        ApplyLayout();
    }

    /// <summary>
    /// Calculates and applies the optimal layout for the grid based on current dimensions.
    /// Handles spacing calculations and grid container resizing.
    /// </summary>
    public void ApplyLayout()
    {
        // Calculate total dimensions
        float totalWidth = cols * cellSize.x;
        float totalHeight = rows * cellSize.y;

        // Calculate optimal spacing
        float horizontalSpacing = (screenSize.x - totalWidth) / (cols + 1);
        float verticalSpacing = (screenSize.y - totalHeight) / (rows + 1);

        // Configure grid layout
        ConfigureGridLayout(horizontalSpacing, verticalSpacing);

        // Update grid container dimensions
        UpdateGridContainer(totalWidth, totalHeight, horizontalSpacing, verticalSpacing);
    }

    /// <summary>
    /// Updates the grid's dimensions and applies new layout settings.
    /// </summary>
    /// <param name="newRows">New number of rows</param>
    /// <param name="newCols">New number of columns</param>
    public void SetGridSize(int newRows, int newCols)
    {
        rows = newRows;
        cols = newCols;
        ApplyLayout();
    }

    /// <summary>
    /// Configures the GridLayoutGroup component with calculated spacing and constraints.
    /// </summary>
    private void ConfigureGridLayout(float horizontalSpacing, float verticalSpacing)
    {
        grid.cellSize = cellSize;
        grid.spacing = new Vector2(horizontalSpacing, verticalSpacing);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = cols;
        grid.childAlignment = TextAnchor.UpperLeft;
    }

    /// <summary>
    /// Updates the grid container's size and position based on calculated dimensions.
    /// </summary>
    private void UpdateGridContainer(float totalWidth, float totalHeight, float horizontalSpacing, float verticalSpacing)
    {
        float layoutWidth = totalWidth + horizontalSpacing * (cols - 1);
        float layoutHeight = totalHeight + verticalSpacing * (rows - 1);

        gridRect.sizeDelta = new Vector2(layoutWidth, layoutHeight);
        gridRect.anchoredPosition = Vector2.zero;
    }
}