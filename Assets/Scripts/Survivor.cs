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
    private Renderer survivorRenderer;
    public Material originalMaterial;

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
}
