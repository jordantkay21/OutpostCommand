using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class TaskBase 
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
    private TileData tile;
    private GameObject tree;
    private TreeData treeData;

    public ChopTreeTask(GameObject targetTree, TileData targetTile)
    {
        tree = targetTree;
        treeData = targetTree.GetComponent<TreeData>();
        tile = targetTile;
    }

    public override void Execute(Survivor survivor, System.Action onTaskComplete)
    {
        if (tree == null || tile == null)
        {
            Debug.LogWarning("Tree or tile is null in ChopTreeTask");
            onTaskComplete?.Invoke();
            return;
        }

        survivor.StartCoroutine(ChopTreeRoutine(onTaskComplete));
    }

    private IEnumerator ChopTreeRoutine(System.Action onTaskComplete)
    {
        Debug.Log("Chopping tree...");
        yield return new WaitForSeconds(2.0f); // Simulate chopping delay

        if (treeData != null)
        {
            // Determine resource counts
            int woodDrops = UnityEngine.Random.Range(treeData.minWoodDrops, treeData.maxWoodDrops + 1);
            int saplingDrops = UnityEngine.Random.value <= treeData.saplingDropChance
                ? UnityEngine.Random.Range(treeData.minSaplingDrops, treeData.maxSaplingDrops + 1)
                : 0;

            // Delegate resource spawning to ResourceManager
            ResourceManager.Instance.SpawnResources(tile, woodDrops, saplingDrops);
        }

        GameObject.Destroy(tree);
        Debug.Log($"Tree chopped.");
        onTaskComplete?.Invoke();
    }
}





