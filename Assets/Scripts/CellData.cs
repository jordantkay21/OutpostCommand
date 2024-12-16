using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellType
{
    Grass,
    Dirt,
    Stone
}

public class CellData : MonoBehaviour
{
    [Header("Cell Properties")]
    public Vector2Int GridPosition;
    public CellType CellType;
    public bool IsOccupied = false;

    private Renderer cellRenderer;

    public void Initialize(Vector2Int gridPosition, CellType cellType, Material cellMaterial)
    {
        GridPosition = gridPosition;
        SetCellType(cellType, cellMaterial);
    }

    private void Awake()
    {
        cellRenderer = GetComponent<Renderer>();
        if (cellRenderer == null)
        {
            Debug.LogError("Renderer component not found on cell prefab.");
        }
    }

    private void OnValidate()
    {
        // Ensure the renderer reference is set
        if (cellRenderer == null)
        {
            cellRenderer = GetComponent<Renderer>();
        }

        // Update the material whenever the CellType is changed
        UpdateMaterial();
    }

    private void OnMouseDown()
    {
        //Cycle through CellType
        CellType nextType = GetNextCellType(CellType);

        //Update the cell type using GridManager
        if (GridManager.Instance != null)
        {
            GridManager.Instance.UpdateCellType(GridPosition, nextType);
        }
    }
    private CellType GetNextCellType(CellType currentType)
    {
        // Get the next CellType in the enum, looping back to the start if at the end
        int nextTypeIndex = ((int)currentType + 1) % System.Enum.GetValues(typeof(CellType)).Length;
        return (CellType)nextTypeIndex;
    }

    private void UpdateMaterial()
    {
        if (cellRenderer != null)
        {
            Material newMaterial = GridManager.Instance?.GetMaterialForCellType(CellType);

            if (newMaterial != null)
            {
                cellRenderer.sharedMaterial = newMaterial; // Use sharedMaterial to apply changes in edit mode
            }
            else
            {
                Debug.LogWarning($"Material not found for CellType {CellType}.");
            }
        }
    }

    public void SetCellType(CellType newType, Material newMaterial)
    {
        CellType = newType;
        if (cellRenderer != null && newMaterial != null)
        {
            cellRenderer.material = newMaterial;
        }
    }

    public void SetOccupied(bool occupied)
    {
        IsOccupied = occupied;
    }


}
