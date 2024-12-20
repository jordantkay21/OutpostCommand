using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class TaskBase : MonoBehaviour
{
    public abstract void Execute(Survivor survivor, System.Action onTaskComplete);
}

public class MoveToTileTask : TaskBase
{
    private TileData targetTile;

    public MoveToTileTask(TileData tile)
    {
        targetTile = tile;
    }

    public override void Execute(Survivor survivor, Action onTaskComplete)
    {
        // Allow movement if the tile is unoccupied or if it's occupied by a tree and the survivor is a lumberjack
        bool canMoveToTile = !targetTile.IsOccupied ||
                             (targetTile.IsOccupied && targetTile.spawnedPrefab != null &&
                              targetTile.spawnedPrefab.CompareTag("Tree") &&
                              survivor.CurrentJob == SurvivorJob.Lumberjack);

        if (!canMoveToTile)
        {
            Debug.LogWarning("Invalid tile for MoveToTileTask.");
            onTaskComplete?.Invoke(); // Prevent task execution from halting
            return;
        }

        // Execute movement
        survivor.MoveToTile(targetTile, () =>
        {
            Debug.Log($"{survivor.name} reached target tile.");
            onTaskComplete?.Invoke(); // Trigger the next task
        });
    }

}

public class ChopTreeTask : TaskBase
{
    private GameObject tree;

    public ChopTreeTask(GameObject targetTree)
    {
        tree = targetTree;
    }

    public override void Execute(Survivor survivor, Action onTaskComplete)
    {
        if (tree == null)
        {
            Debug.LogWarning("Tree is null in ChopTreeTask");
            onTaskComplete?.Invoke();
            return;
        }

        survivor.StartCoroutine(ChopTreeRoutine(onTaskComplete));
    }

    private IEnumerator ChopTreeRoutine(System.Action onTaskComplete)
    {
        Debug.Log("Chopping tree...");
        yield return new WaitForSeconds(2.0f); // Simulate chopping delay
        GameObject.Destroy(tree);
        Debug.Log("Tree chopped.");
        onTaskComplete?.Invoke();
    }
}



