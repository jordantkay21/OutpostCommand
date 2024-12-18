using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData : MonoBehaviour
{
    public bool IsOccupied;
    public GameObject gridNode;
    public Material TileMaterial;
    public GameObject spawnedPrefab;

    private Renderer renderer;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        if (renderer == null) 
            Debug.LogError($"Renderer component not found on {name}");
    }

    public void SetMaterial(Material regionMaterial)
    {
        TileMaterial = regionMaterial;
        renderer.material = regionMaterial;
    }
}