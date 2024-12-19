using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SurvivorManager : MonoBehaviour
{
    [Header("Survivor Settings")]
    public LayerMask survivorLayer; // Layer mask for detecting survivors
    public GameObject JobPanel;

    [Header("Job Materials")]
    public Material HighlightMaterial;
    public Material FarmerMaterial;
    public Material LumberJackMaterial;

    private Survivor selectedSurvivor;

    void Update()
    {
        HandleSelection();
    }

    private void HandleSelection()
    {
        // Check for mouse input
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, survivorLayer))
            {
                Survivor survivor = hit.collider.GetComponent<Survivor>();
                Debug.Log($"{hit.collider.name} has been selected");

                if (survivor != null)
                {
                    SelectSurvivor(survivor);
                }
            }
        }
    }

    private void SelectSurvivor(Survivor survivor)
    {
        // Deselect previous survivor
        if (selectedSurvivor != null)
        {
            selectedSurvivor.SetMaterial(HighlightMaterial);
        }

        // Select new survivor
        selectedSurvivor = survivor;
        selectedSurvivor.SetMaterial(HighlightMaterial);
        JobPanel.SetActive(true);
        Debug.Log($"Selected {survivor.name}");
    }

    public void AssignJobToSelectedSurvivor(SurvivorJob job)
    {
        if (selectedSurvivor != null)
        {
            selectedSurvivor.AssignJob(job);
        }
        else
        {
            Debug.LogWarning("No survivor selected to assign a job.");
        }

        JobPanel.SetActive(false);
    }

    public void AssignFarmerJob()
    {
        AssignJobToSelectedSurvivor(SurvivorJob.Farmer);
        selectedSurvivor.SetMaterial(FarmerMaterial);
    }

    public void AssignLumberJack()
    {
        AssignJobToSelectedSurvivor(SurvivorJob.Lumberjack);
        selectedSurvivor.SetMaterial(LumberJackMaterial);
    }
}
