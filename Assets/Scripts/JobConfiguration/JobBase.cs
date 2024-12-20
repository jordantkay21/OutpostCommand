using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class JobBase
{
    public abstract SurvivorJob JobType { get; }
    public abstract void Assign(Survivor survivor);
    public abstract void PerformJob(Survivor survivor);
}

public class FarmerJob : JobBase
{
    public override SurvivorJob JobType => SurvivorJob.Farmer;

    public override void Assign(Survivor survivor)
    {
        survivor.AssignJob(JobType);
        Debug.Log($"{survivor.name} is farming. Select a region for {survivor.name} to farm.");
        RegionSelector.Instance.BeginRegionSelection(survivor, JobType);
    }

    public override void PerformJob(Survivor survivor)
    {
        //farmer-Specific Logic
        Debug.Log($"{survivor.name} is farming.");
    }
}

public class LumberjackJob : JobBase
{
    public override SurvivorJob JobType => SurvivorJob.Lumberjack;

    public override void Assign(Survivor survivor)
    {
        survivor.AssignJob(JobType);
        survivor.JobTask = this;
        Debug.Log($"{survivor.name} is now a Lumberjack!");

        RegionSelector.Instance.BeginRegionSelection(survivor, JobType);

    }


    public override void PerformJob(Survivor survivor)
    {
        RegionData assignedRegion = SurvivorManager.Instance.GetAssignedRegionForSurvivor(survivor);
        if (assignedRegion == null)
        {
            Debug.LogWarning("No region assigned for lumberjack.");
            return;
        }

        List<TaskBase> tasks = new List<TaskBase>();

        foreach (TileData tile in assignedRegion.Tiles)
        {
            if (tile.IsOccupied && tile.spawnedPrefab.CompareTag("Tree"))
            {
                tasks.Add(new MoveToTileTask(tile));
                tasks.Add(new ChopTreeTask(tile.spawnedPrefab, tile));
            }
        }

        if (tasks.Count > 0)
        {
            TaskHandler taskManager = survivor.GetComponent<TaskHandler>();
            taskManager.AddTasks(tasks);
        }
        else
        {
            Debug.Log("No trees found in assigned region.");
        }
    }
}
