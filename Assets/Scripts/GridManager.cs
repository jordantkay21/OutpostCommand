using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Grid Settings")]
    public int gridWidth;
    public int gridHeight;
    public float cellSize = 5f;

    [Header("Prefabs and Materials")]
    public GameObject cellPrefab;
    public Material grassMaterial;
    public Material dirtMaterial;
    public Material stoneMaterial;

    [Header("Gameplay Prefabs")]
    public GameObject housePrefab;
    public GameObject farmPlotPrefab;
    public GameObject treePrefab;
    public GameObject survivorPrefab;

    [Header("Spawn Settings")]
    public int dirtCellCount = 10;
    public int stoneCellCount = 5;
    public int farmCellCount = 3;
    public int treeCount = 5;

    private CellData[,] grid;
    private List<Vector2Int> dirtCells = new List<Vector2Int>();
    private List<Vector2Int> stoneCells = new List<Vector2Int>();
    private List<Vector2Int> grassCells = new List<Vector2Int>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        GenerateBaseLayer();
        PlaceRandomDirtAndStone();
        PlaceHouseOnStone();
        PlaceFarmsOnDirt();
        PlantTreesOnGrass();
    }

    private void GenerateBaseLayer()
    {
        grid = new CellData[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 worldPosition = new Vector3(x * cellSize, 0, y * cellSize);
                GameObject cellObject = Instantiate(cellPrefab, worldPosition, Quaternion.identity, transform);

                CellData cell = cellObject.GetComponent<CellData>();
                cell.Initialize(new Vector2Int(x, y), CellType.Grass, grassMaterial);
                grid[x, y] = cell;

                grassCells.Add(new Vector2Int(x, y));
            }
        }
    }

    private void PlaceRandomDirtAndStone()
    {
        for (int i = 0; i < dirtCellCount; i++)
            AssignRandomCell(CellType.Dirt, dirtCells);

        for (int i = 0; i < stoneCellCount; i++)
            AssignRandomCell(CellType.Stone, stoneCells);
    }

    private void AssignRandomCell(CellType type, List<Vector2Int> targetList)
    {
        List<Vector2Int> availableCells = new List<Vector2Int>(grassCells);
        if (type == CellType.Dirt)
            availableCells.RemoveAll(pos => dirtCells.Contains(pos));
        if (type == CellType.Stone)
            availableCells.RemoveAll(pos => stoneCells.Contains(pos));

        if (availableCells.Count > 0)
        {
            Vector2Int chosenPos = availableCells[Random.Range(0, availableCells.Count)];
            grid[chosenPos.x, chosenPos.y].SetCellType(type, GetMaterialForCellType(type));

            targetList.Add(chosenPos);
            grassCells.Remove(chosenPos);
        }
    }

    private void PlaceHouseOnStone()
    {
        if (stoneCells.Count > 0)
        {
            Vector2Int housePosition = stoneCells[Random.Range(0, stoneCells.Count)];
            Instantiate(housePrefab, GetCenterGridPosition(housePosition) + new Vector3(0, GetPrefabHeight(housePrefab) / 2, 0), Quaternion.identity);

            SpawnSurvivorsAroundHouse(housePosition);
        }
    }

    private void PlaceFarmsOnDirt()
    {
        for (int i = 0; i < farmCellCount && dirtCells.Count > 0; i++)
        {
            
            Vector2Int farmPosition = dirtCells[Random.Range(0, dirtCells.Count)];
            Instantiate(farmPlotPrefab, GetCenterGridPosition(farmPosition) + new Vector3(0, GetPrefabHeight(farmPlotPrefab) / 2, 0), Quaternion.identity);
            dirtCells.Remove(farmPosition);
        }
    }

    private void PlantTreesOnGrass()
    {
        for (int i = 0; i < treeCount && grassCells.Count > 0; i++)
        {
            Vector2Int treePosition = grassCells[Random.Range(0, grassCells.Count)];
            Instantiate(treePrefab, GetCenterGridPosition(treePosition) + new Vector3(0, GetPrefabHeight(treePrefab)/2, 0), Quaternion.identity);
            grassCells.Remove(treePosition);
        }
    }

    private void SpawnSurvivorsAroundHouse(Vector2Int houseCenter)
    {
        //Calculate the center position of the house in world space
        Vector3 houseWorldPosition = GetCenterGridPosition(houseCenter);

        //Offset positions: move 1.5 cells away (house boundary + half a cell)
        float edgeOffset = cellSize / 2 - .5f;

        Vector3 leftCenter = houseWorldPosition + new Vector3(-edgeOffset, 0, 0);
        Vector3 rightCenter = houseWorldPosition + new Vector3(edgeOffset, 0, 0);

        Instantiate(survivorPrefab, leftCenter + new Vector3(0, GetPrefabHeight(survivorPrefab) / 2, 0), Quaternion.identity);
        Instantiate(survivorPrefab, rightCenter + new Vector3(0, GetPrefabHeight(survivorPrefab) / 2, 0), Quaternion.identity);
    }

    private Vector3 GetCenterGridPosition(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x * cellSize + cellSize / 2f, 0, gridPosition.y * cellSize + cellSize / 2f);
    }

    private float GetPrefabHeight(GameObject prefab)
    {
        return prefab.GetComponentInChildren<Renderer>().bounds.size.y;
    }

    public Material GetMaterialForCellType(CellType cellType)
    {
        switch (cellType)
        {
            case CellType.Grass: return grassMaterial;
            case CellType.Dirt: return dirtMaterial;
            case CellType.Stone: return stoneMaterial;
            default: return grassMaterial;
        }
    }
}
