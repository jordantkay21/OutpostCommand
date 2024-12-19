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
    public List<Material> RegionMaterials = new List<Material>();

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

    #region Helper Methods
    public Material GetRandomMaterial(List<Material> materials)
    {
        return materials[Random.Range(0, materials.Count)];
    }
    #endregion

}
