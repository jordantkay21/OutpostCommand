using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RegionType
{
    Grass,
    Dirt,
    Stone,
    Forest
}

public class RegionData : MonoBehaviour
{
    public List<TileData> Tiles = new List<TileData>();
    [Header("Region Properties")]
    public Vector2Int GridPosition;
    public RegionType RegionType;
    public bool IsAssigned = false;
    public GameObject Border;
    public Material validMaterial;
    public Material invalidMaterial;
    public List<Material> RegionMaterials = new List<Material>();

    private Renderer borderRenderer;

    private void Awake()
    {
        if (Border != null)
        {
            Border.SetActive(false);
            borderRenderer = Border.GetComponent<Renderer>();
        }
        else
        {
            Debug.Log($"{name} does not have a Border Assigned");
        }
    }


    public void Initialize(Vector2Int gridPos, RegionType regionType, List<Material> regionMaterials)
    {
        GridPosition = gridPos;
        RegionType = regionType;
        RegionMaterials = regionMaterials;

        SetRegionType(RegionType,RegionMaterials);
    }

    public void SetRegionType(RegionType newType, List<Material> newMaterials)
    {
        RegionType = newType;

        foreach (var tile in Tiles)
        {
            tile.SetMaterial(GetRandomMaterial(newMaterials));
        }
    }

    public void Highlight(bool isHovered, bool isValid = true)
    {
        if (Border != null)
        {
            Border.SetActive(isHovered);
            
            // Loop through all child objects of the border and set their materials
            foreach (Transform child in Border.transform)
            {
                Renderer renderer = child.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = isValid ? validMaterial : invalidMaterial;
                }
            }
        }
    }

    public void AssignToJob(SurvivorJob job)
    {
        IsAssigned = true;
    }

    #region Helper Methods
    public Material GetRandomMaterial(List<Material> materials)
    {
        return materials[Random.Range(0, materials.Count)];
    }

    public TileData GetNearestAvailableTile(Vector3 position)
    {
        TileData nearestTile = null;
        float minDistance = float.MaxValue;

        foreach (var tile in Tiles)
        {
            if (!tile.IsOccupied)
            {
                float distance = Vector3.Distance(position, tile.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestTile = tile;
                }
            }
        }

        return nearestTile;
    }
    #endregion

}
