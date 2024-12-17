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
    [Header("Cell Properties")]
    public Vector2Int GridPosition;
    public RegionType RegionType;
    public List<Transform> GridNodes = new List<Transform>();

    private Renderer RegionRenderer;

    public void Initialize(Vector2Int gridPosition, RegionType regionType, Material regionMaterial)
    {
        GridPosition = gridPosition;
        SetRegionType(regionType, regionMaterial);
    }

    private void Awake()
    {
        RegionRenderer = GetComponent<Renderer>();
        if (RegionRenderer == null)
        {
            Debug.LogError("Renderer component not found on cell prefab.");
        }
    }

    public void SetRegionType(RegionType newType, Material newMaterial)
    {
        RegionType = newType;
        if (RegionRenderer != null && newMaterial != null)
        {
            RegionRenderer.material = newMaterial;
        }
    }
}
