using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [Tooltip("X = Width | Y = Depth")]
    public Vector2Int GridSize;
    public float RegionSize = 5f;

    [Header("Environment Prefabs")]
    public GameObject RegionPrefab;
    public GameObject HousePrefab;
    public GameObject TreePrefab;

    [Header("Environment Materials")]
    public Material highlightMaterial;
    public List<Material> grassMaterial;
    public List<Material> forestMaterials;
    public List<Material> stoneMaterials;

    [Header("Grid Configuration")]
    public RegionData[,] grid;
    public int minGrassRegionCount;
    public int maxGrassRegionCount;
    public List<Vector2Int> grassRegions = new List<Vector2Int>();
    public List<Vector2Int> forestRegions = new List<Vector2Int>();
    public List<Vector2Int> stoneRegions = new List<Vector2Int>();

    [Header("Prefab Configuration")]
    public int minTreesToSpawn;
    public int maxTreesToSpawn;

    private void Awake()
    {
        GenerateBaseLayer();
        ConvertForestToGrass();
        SpawnHouseInRegion();
        SpawnTreesInforestRegions();
    }

    private void GenerateBaseLayer()
    {
        grid = new RegionData[GridSize.x, GridSize.y];

        for(int x = 0; x < GridSize.x; x++)
        {
            for (int y=0; y < GridSize.y; y++)
            {
                Vector3 worldPosition = new Vector3(x * RegionSize, 0, y * RegionSize);
                GameObject regionObject = Instantiate(RegionPrefab, worldPosition, Quaternion.identity, transform);
                regionObject.name = $"Region_{x},{y}";

                RegionData region = regionObject.GetComponent<RegionData>();
                region.Initialize(new Vector2Int(x, y), RegionType.Forest, forestMaterials);

                grid[x, y] = region;

                forestRegions.Add(new Vector2Int(x, y));
            }
        }

        RegionData centerRegion = GetCenterRegion();
        centerRegion.SetRegionType(RegionType.Stone, stoneMaterials);
        forestRegions.Remove(centerRegion.GridPosition);
        stoneRegions.Add(centerRegion.GridPosition);
    }

    public void SpawnHouseInRegion()
    {
        //Fetch the center region
        RegionData centerRegion = GetCenterRegion();
        if(centerRegion == null)
        {
            Debug.LogError("Cannot spawn house: Center region not found.");
            return;
        }

        //Calculate the world position at the center of the region
        Vector3 centerWorldPosition = GetCenterGridPosition(centerRegion.GridPosition);
        

        //Instantiate the house prefab at the calculated position
        if(HousePrefab != null)
        {
            Instantiate(HousePrefab, centerWorldPosition + new Vector3(0,GetPrefabHeight(HousePrefab)/2,0), Quaternion.identity);
            Debug.Log($"House spawned at {centerWorldPosition}");
        }
        else
        {
            Debug.LogWarning("HousePrefab is not assigned.");
        }

        SurvivorManager.Instance.SpawnSurvivorsAroundHouse(centerWorldPosition, RegionSize);
    }



    public void ConvertForestToGrass()
    {
        int numberOfGrassRegions = Random.Range(minGrassRegionCount, maxGrassRegionCount);

        if (forestRegions.Count < numberOfGrassRegions)
        {
            Debug.LogWarning("Not enough forest regions to convert.");
            return;
        }

        // Randomly select forest regions to convert
        List<Vector2Int> selectedRegions = new List<Vector2Int>();

        while (selectedRegions.Count < numberOfGrassRegions)
        {
            int randomIndex = Random.Range(0, forestRegions.Count);
            Vector2Int selectedPosition = forestRegions[randomIndex];

            if (!selectedRegions.Contains(selectedPosition))
            {
                selectedRegions.Add(selectedPosition);
            }
        }

        // Convert selected forest regions to grass
        foreach (Vector2Int position in selectedRegions)
        {
            RegionData forestRegion = grid[position.x, position.y];
            if (forestRegion != null)
            {
                forestRegion.SetRegionType(RegionType.Grass, grassMaterial);
                forestRegions.Remove(position);
                grassRegions.Add(position);
            }
        }

        Debug.Log($"{numberOfGrassRegions} forest regions converted to grass.");
    }

    private void SpawnTreesInforestRegions()
    {
        Transform TreeContainter = new GameObject("Tree Container").transform;

        foreach(Vector2Int forestPosition in forestRegions)
        {
            //Access the forest region
            RegionData forestRegion = grid[forestPosition.x, forestPosition.y];

            if (forestRegion == null || forestRegion.Tiles.Count == 0)
                continue;

            //Determine the number of trees for this forest region
            int treesToSpawn = Random.Range(minTreesToSpawn, maxTreesToSpawn + 1);

            //Randomly spawn trees in available GridNodes
            List<TileData> availableTiles = new List<TileData>(forestRegion.Tiles);
            foreach (TileData tile in forestRegion.Tiles)
            {
                if (!tile.IsOccupied)
                {
                    availableTiles.Add(tile);
                }
            }

            Transform forestRegionContainer = new GameObject($"{forestRegion}_TreeContainer").transform;
            forestRegionContainer.parent = TreeContainter;

            for (int i = 0; i < treesToSpawn && availableTiles.Count > 0; i++)
            {
                //Choose a random Tile
                int randomIndex = Random.Range(0, availableTiles.Count);
                TileData chosenTile = availableTiles[randomIndex];

                //Spawn treePrefab at the node's position
                Vector3 spawnPosition = chosenTile.transform.position + new Vector3(0, GetPrefabHeight(TreePrefab) / 2, 0);
                GameObject spawnedTree = Instantiate(TreePrefab, spawnPosition, Quaternion.identity, forestRegionContainer);

                //Mark the tile as occupied and store the reference
                chosenTile.OccupyTile(spawnedTree);

                //Remove the tile to avoid spawning multiple trees at the same spot
                availableTiles.RemoveAt(randomIndex);
            }
        }
    }

    public void HighlightCenterRegion(Material highlightMaterial)
    {
        RegionData centerRegion = GetCenterRegion();
        if (centerRegion != null)
        {
            foreach (var tile in centerRegion.Tiles)
            {
                tile.SetMaterial(highlightMaterial);
            }
        }
    }

    #region Helper Methods
    public Vector3 GetCenterGridPosition(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x * RegionSize + RegionSize/2 -.5f, 0, gridPosition.y * RegionSize + RegionSize/2 - .5f);
    }
    public RegionData GetCenterRegion()
    {
        // Calculate the center of the grid
        int centerX = Mathf.FloorToInt(GridSize.x / 2);
        int centerY = Mathf.FloorToInt(GridSize.y / 2);

        // Retrieve the RegionData at the center
        if (grid != null && grid[centerX, centerY] != null)
        {
            return grid[centerX, centerY];
        }

        Debug.LogWarning("Center region not found or grid not initialized.");
        return null;
    }
    public float GetPrefabHeight(GameObject prefab)
    {
        return prefab.GetComponentInChildren<Renderer>().bounds.size.y;
    }
    #endregion

}
