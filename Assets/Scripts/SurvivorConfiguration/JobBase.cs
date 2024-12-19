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
        Debug.Log($"{survivor.name} is now a Lumberjack! Select a region for {survivor.name} to chop wood.");
        RegionSelector.Instance.BeginRegionSelection(survivor, JobType);
    }

    public override void PerformJob(Survivor survivor)
    {
        // Lumberjack-specific logic, e.g., chopping trees
        Debug.Log($"{survivor.name} is chopping wood.");
    }
}
