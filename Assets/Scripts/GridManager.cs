using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Grid Settings")]
    public int gridWidth;
    public int gridHeight;
    public float cellSize = 5f;

    [Header("Prefab and Materials")]
    public GameObject cellPrefab;
    public Material grassMaterial;
    public Material dirtMaterial;
    public Material stoneMaterial;

    [Header("Randomization")]
    public int seed;

    private CellData[,] grid; // Array to store grid cells

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); //Ensure only one instance exists
        }
    }
    private void Start()
    {
        GenerateBaseLayer();
    }

    private void GenerateBaseLayer()
    {
        // Initialize grid array
        grid = new CellData[gridWidth, gridHeight];

        // Set seed for repeatable randomization
        Random.InitState(seed);

        for (int x = 0; x < gridWidth; x++)
        {
            //Create a parent GameObject for the current column
            GameObject columnParent = new GameObject($"Column_{x}");
            columnParent.transform.parent = transform; //Make it a child of GridManager for hierarchy organization

            for (int y = 0; y < gridHeight; y++)
            {
                // Determine the position in the world
                Vector3 worldPosition = new Vector3(x * cellSize, 0, y * cellSize);

                // Instantiate the cell prefab
                GameObject cellObject = Instantiate(cellPrefab, worldPosition, Quaternion.identity);
                cellObject.transform.parent = columnParent.transform; // Make it a child of this object for hierarchy organization
                cellObject.name = $"Cell_{x}_{y}";

                // Randomly assign a cell type
                CellType cellType = CellType.Grass;
                Material cellMaterial = grassMaterial;

                // Initialize the CellData script on the prefab
                CellData cell = cellObject.GetComponent<CellData>();
                if (cell != null)
                {
                    cell.Initialize(new Vector2Int(x, y), cellType, cellMaterial);
                    grid[x, y] = cell;
                }
            }
        }
    }

    private CellType GetRandomCellType()
    {
        int randomValue = Random.Range(0, 3); // 0 = Grass, 1 = Dirt, 2 = Stone
        return (CellType)randomValue;
    }

    public Material GetMaterialForCellType(CellType cellType)
    {
        switch (cellType)
        {
            case CellType.Grass:
                return grassMaterial;
            case CellType.Dirt:
                return dirtMaterial;
            case CellType.Stone:
                return stoneMaterial;
            default:
                return grassMaterial;
        }
    }

    public void UpdateCellType(Vector2Int gridPosition, CellType newType)
    {
        if (gridPosition.x >= 0 && gridPosition.x < gridWidth && gridPosition.y >= 0 && gridPosition.y < gridHeight)
        {
            CellData cell = grid[gridPosition.x, gridPosition.y];
            if (cell != null)
            {
                Material newMaterial = GetMaterialForCellType(newType);
                cell.SetCellType(newType, newMaterial);
            }
        }
        else
        {
            Debug.LogWarning("Invalid grid position.");
        }
    }

    public void SetCellOccupied(Vector2Int gridPosition, bool occupied)
    {
        if (gridPosition.x >= 0 && gridPosition.x < gridWidth && gridPosition.y >= 0 && gridPosition.y < gridHeight)
        {
            grid[gridPosition.x, gridPosition.y].SetOccupied(occupied);
        }
    }

    public bool TryGetCellFromWorldPosition(Vector3 worldPosition, out CellData cellData)
    {
        int x = Mathf.FloorToInt(worldPosition.x / cellSize);
        int y = Mathf.FloorToInt(worldPosition.z / cellSize);

        if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
        {
            cellData = grid[x, y];
            return true;
        }

        cellData = null;
        return false;
    }

    public bool IsValidCell(Vector2Int position)
    {
        return position.x >= 0 && position.x < gridWidth && position.y >= 0 && position.y < gridHeight;
    }

    public bool IsCellOccupied(Vector2Int position)
    {
        if (IsValidCell(position))
        {
            return grid[position.x, position.y].IsOccupied;
        }
        return true;
    }

    public Vector3 GetWorldPositionFromGrid(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x * cellSize, 0, gridPosition.y * cellSize);
    }
}
