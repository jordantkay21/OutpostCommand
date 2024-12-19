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
    private Color originalColor;

    private void Awake()
    {
        survivorRenderer = GetComponent<Renderer>();
        if (survivorRenderer != null)
        {
            originalColor = survivorRenderer.material.color;
        }
    }

    public void AssignJob(SurvivorJob job)
    {
        CurrentJob = job;
        Debug.Log($"{name} assigned as {job}");
    }

    public void SetMaterial(Material newMaterial)
    {
        if (survivorRenderer != null)
        {
            survivorRenderer.material = newMaterial;
        }
    }
}
