using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SurvivorJob
{
    None,
    Farmer,
    Lumberjack
}

public class Survivor : MonoBehaviour
{
    public SurvivorJob CurrentJob = SurvivorJob.None; // Default job
    public JobBase JobTask;
    private Renderer survivorRenderer;
    public Material originalMaterial;
    public float movementSpeed;

    private void Awake()
    {
        survivorRenderer = GetComponent<Renderer>();
    }

    public void AssignJob(SurvivorJob job)
    {
        CurrentJob = job;
        Debug.Log($"{name} assigned as {job}");
    }
    public void SetMaterial(Material newMaterial, bool cacheMaterial = true)
    {
        if (survivorRenderer != null)
        {
            if(cacheMaterial) originalMaterial = newMaterial;
            survivorRenderer.material = newMaterial;
        }
    }

    public void MoveToTile(TileData targetTile, System.Action onArrival = null)
    {
        if (targetTile == null)
        {
            Debug.LogWarning("Invalid tile selected for movement.");
            return;
        }

        // Allow movement if the tile is unoccupied or if it's occupied by a tree and the survivor's job involves trees
        bool canMoveToTile = !targetTile.IsOccupied ||
                             (targetTile.IsOccupied && targetTile.spawnedPrefab != null &&
                              IsTileRelevantToJob(targetTile));

        if (!canMoveToTile)
        {
            Debug.LogWarning("Cannot move to tile; it's occupied and not part of the job logic.");
            return;
        }

        // Calculate the destination position with height adjustment
        float survivorHeight = GetComponent<Collider>().bounds.size.y; // Get survivor's height from the collider
        Vector3 destination = targetTile.transform.position + new Vector3(0, survivorHeight / 2, 0);

        StartCoroutine(MoveToPosition(destination, () =>
        {
            Debug.Log($"{name} moved to tile at {destination}");
            onArrival?.Invoke();
        }));
    }


    private bool IsTileRelevantToJob(TileData tile)
    {
        if (CurrentJob == SurvivorJob.Lumberjack && tile.spawnedPrefab.CompareTag("Tree")) return true;
        // Add additional job-specific conditions here
        return false;
    }



    private IEnumerator MoveToPosition(Vector3 destination, System.Action onReachDestination)
    {
        while (Vector3.Distance(transform.position, destination) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, movementSpeed * Time.deltaTime);
            yield return null;
        }

        onReachDestination?.Invoke();
    }
}
