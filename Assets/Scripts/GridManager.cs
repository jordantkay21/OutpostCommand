using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Grid Settings")]
    public int gridWidth;
    public int gridHeight;
    public float RegionSize = 5f;

    [Header("Prefabs and Materials")]
    public GameObject RegionPrefab;
    public Material grassMaterial;
    public Material dirtMaterial;
    public Material stoneMaterial;
    public Material forestMaterial;

    [Header("Gameplay Prefabs")]
    public GameObject housePrefab;
    public GameObject farmPlotPrefab;
    public GameObject treePrefab;
    public GameObject survivorPrefab;

    [Header("Spawn Region Settings")]
    public int dirtRegionCount = 10;
    public int stoneRegionCount = 5;
    public int farmRegionCount = 3;
    public int forestRegionCount = 5;

    [Header("Forest Region Settings")]
    public int minTreesPerForest = 3;
    public int maxTreesPerForest = 8;

    private RegionData[,] grid;
    private List<Vector2Int> dirtRegions = new List<Vector2Int>();
    private List<Vector2Int> stoneRegions = new List<Vector2Int>();
    private List<Vector2Int> grassRegions = new List<Vector2Int>();
    private List<Vector2Int> forestRegions = new List<Vector2Int>();

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
        ConvertGrassToForest();
        SpawnTreesInForestRegions();
    }

    private void GenerateBaseLayer()
    {
        grid = new RegionData[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 worldPosition = new Vector3(x * RegionSize, 0, y * RegionSize);
                GameObject cellObject = Instantiate(RegionPrefab, worldPosition, Quaternion.identity, transform);

                RegionData cell = cellObject.GetComponent<RegionData>();
                cell.Initialize(new Vector2Int(x, y), RegionType.Grass, grassMaterial);
                grid[x, y] = cell;

                grassRegions.Add(new Vector2Int(x, y));
            }
        }
    }

    private void PlaceRandomDirtAndStone()
    {
        for (int i = 0; i < dirtRegionCount; i++)
            AssignRandomCell(RegionType.Dirt, dirtRegions);

        for (int i = 0; i < stoneRegionCount; i++)
            AssignRandomCell(RegionType.Stone, stoneRegions);
    }

    private void AssignRandomCell(RegionType type, List<Vector2Int> targetList)
    {
        List<Vector2Int> availableCells = new List<Vector2Int>(grassRegions);
        if (type == RegionType.Dirt)
            availableCells.RemoveAll(pos => dirtRegions.Contains(pos));
        if (type == RegionType.Stone)
            availableCells.RemoveAll(pos => stoneRegions.Contains(pos));

        if (availableCells.Count > 0)
        {
            Vector2Int chosenPos = availableCells[Random.Range(0, availableCells.Count)];
            grid[chosenPos.x, chosenPos.y].SetRegionType(type, GetMaterialForCellType(type));

            targetList.Add(chosenPos);
            grassRegions.Remove(chosenPos);
        }
    }

    private void PlaceHouseOnStone()
    {
        if (stoneRegions.Count > 0)
        {
            Vector2Int housePosition = stoneRegions[Random.Range(0, stoneRegions.Count)];
            Instantiate(housePrefab, GetCenterGridPosition(housePosition) + new Vector3(0, GetPrefabHeight(housePrefab) / 2, 0), Quaternion.identity);

            SpawnSurvivorsAroundHouse(housePosition);
        }
    }

    private void PlaceFarmsOnDirt()
    {
        for (int i = 0; i < farmRegionCount && dirtRegions.Count > 0; i++)
        {
            
            Vector2Int farmPosition = dirtRegions[Random.Range(0, dirtRegions.Count)];
            Instantiate(farmPlotPrefab, GetCenterGridPosition(farmPosition) + new Vector3(0, GetPrefabHeight(farmPlotPrefab) / 2, 0), Quaternion.identity);
            dirtRegions.Remove(farmPosition);
        }
    }

    private void ConvertGrassToForest()
    {
        for (int i = 0; i < forestRegionCount && grassRegions.Count > 0; i++)
        {
            // Randomly select a grass region
            Vector2Int forestPosition = grassRegions[Random.Range(0, grassRegions.Count)];

            // Update the region type and apply the forest material
            grid[forestPosition.x, forestPosition.y].SetRegionType(RegionType.Forest, forestMaterial);

            // Update region lists
            forestRegions.Add(forestPosition);
            grassRegions.Remove(forestPosition);
        }
    }


    private void SpawnSurvivorsAroundHouse(Vector2Int houseCenter)
    {
        //Calculate the center position of the house in world space
        Vector3 houseWorldPosition = GetCenterGridPosition(houseCenter);

        //Offset positions: move 1.5 cells away (house boundary + half a cell)
        float edgeOffset = RegionSize / 2 - .5f;

        Vector3 leftCenter = houseWorldPosition + new Vector3(-edgeOffset, 0, 0);
        Vector3 rightCenter = houseWorldPosition + new Vector3(edgeOffset, 0, 0);

        Instantiate(survivorPrefab, leftCenter + new Vector3(0, GetPrefabHeight(survivorPrefab) / 2, 0), Quaternion.identity);
        Instantiate(survivorPrefab, rightCenter + new Vector3(0, GetPrefabHeight(survivorPrefab) / 2, 0), Quaternion.identity);
    }

    private void SpawnTreesInForestRegions()
    {
        foreach (Vector2Int forestPosition in forestRegions)
        {
            // Access the forest region
            RegionData forestRegion = grid[forestPosition.x, forestPosition.y];

            if (forestRegion == null || forestRegion.GridNodes.Count == 0) continue;

            // Determine the number of trees for this forest region
            int treesToSpawn = Random.Range(minTreesPerForest, maxTreesPerForest + 1);

            // Randomly spawn trees in available GridNodes
            List<Transform> availableNodes = new List<Transform>(forestRegion.GridNodes);

            for (int i = 0; i < treesToSpawn && availableNodes.Count > 0; i++)
            {
                // Choose a random GridNode
                int randomIndex = Random.Range(0, availableNodes.Count);
                Transform chosenNode = availableNodes[randomIndex];

                // Spawn treePrefab at the node's position
                Instantiate(treePrefab, chosenNode.position + new Vector3(0, GetPrefabHeight(treePrefab) / 2, 0), Quaternion.identity);

                // Remove the node to avoid spawning multiple trees at the same spot
                availableNodes.RemoveAt(randomIndex);
            }
        }
    }


    private Vector3 GetCenterGridPosition(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x * RegionSize + RegionSize / 2f, 0, gridPosition.y * RegionSize + RegionSize / 2f);
    }

    private float GetPrefabHeight(GameObject prefab)
    {
        return prefab.GetComponentInChildren<Renderer>().bounds.size.y;
    }

    public Material GetMaterialForCellType(RegionType cellType)
    {
        switch (cellType)
        {
            case RegionType.Grass: return grassMaterial;
            case RegionType.Dirt: return dirtMaterial;
            case RegionType.Stone: return stoneMaterial;
            default: return grassMaterial;
        }
    }
}
