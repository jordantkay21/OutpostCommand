using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [Tooltip("X = Width | Y = Depth")]
    public Vector2Int GridSize;
    public float RegionSize = 5f;

    [Header("Prefabs and Materials")]
    public GameObject RegionPrefab;
    public Material highlightMaterial;
    public List<Material> forestMaterials;

    [Header("Grid Configuration")]
    public RegionData[,] grid;
    public List<Vector2Int> forestRegions = new List<Vector2Int>();

    private void Awake()
    {
        GenerateBaseLayer();
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

                RegionData region = regionObject.GetComponent<RegionData>();
                region.Initialize(new Vector2Int(x, y), RegionType.Forest, forestMaterials);

                grid[x, y] = region;

                forestRegions.Add(new Vector2Int(x, y));
            }
        }

        HighlightCenterRegion(highlightMaterial);
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


}
