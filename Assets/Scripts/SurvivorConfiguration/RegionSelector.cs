using UnityEngine;
using UnityEngine.InputSystem;

public class RegionSelector : MonoBehaviour
{
    public static RegionSelector Instance;

    private Survivor currentSurvivor;
    private SurvivorJob currentJob;
    private RegionData currentlyHoveredRegion;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void BeginRegionSelection(Survivor survivor, SurvivorJob job)
    {
        currentSurvivor = survivor;
        currentJob = job;

        Debug.Log($"Enter region selection mode for {job}.");
    }

    private void Update()
    {
        if (currentSurvivor == null || currentJob == SurvivorJob.None) return;

        HandleRegionHover();
        HandleRegionSelection();
    }

    private void HandleRegionHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            RegionData region = hit.collider.GetComponent<RegionData>();
            if (region != null)
            {
                bool isValid = IsRegionValidForJob(region.RegionType, currentJob);

                if (region != currentlyHoveredRegion)
                {
                    if (currentlyHoveredRegion != null)
                    {
                        currentlyHoveredRegion.Highlight(false); // Dehighlight the previously hovered region
                    }

                    region.Highlight(true, isValid); // Highlight with validity check
                    currentlyHoveredRegion = region;
                }
            }
            else if (currentlyHoveredRegion != null) // Dehighlight if no valid region is hit
            {
                currentlyHoveredRegion.Highlight(false);
                currentlyHoveredRegion = null;
            }
        }
        else if (currentlyHoveredRegion != null) // Dehighlight if the raycast doesn't hit anything
        {
            currentlyHoveredRegion.Highlight(false);
            currentlyHoveredRegion = null;
        }
    }

    private void HandleRegionSelection()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                RegionData region = hit.collider.GetComponent<RegionData>();
                if (region != null)
                {
                    if (!IsRegionValidForJob(region.RegionType, currentJob))
                    {
                        Debug.LogWarning("Invalid region type for this job.");
                        return;
                    }

                    if (!region.IsAssigned)
                    {
                        region.AssignToJob(currentJob);
                        Debug.Log($"Region {region.name} assigned to {currentSurvivor.name} as {currentJob}.");

                        SurvivorManager.Instance.AssignRegionToSurvivor(currentSurvivor, region);
                        ResetSelection();
                    }
                    else
                    {
                        Debug.LogWarning("Region is already assigned.");
                    }
                }
            }
        }
    }

    private void ResetSelection()
    {
        if (currentlyHoveredRegion != null)
        {
            currentlyHoveredRegion.Highlight(false);
            currentlyHoveredRegion = null;
        }

        currentSurvivor = null;
        currentJob = SurvivorJob.None;
        Debug.Log("Exit region selection mode.");
    }

    private bool IsRegionValidForJob(RegionType regionType, SurvivorJob job)
    {
        return (job == SurvivorJob.Lumberjack && regionType == RegionType.Forest) ||
               (job == SurvivorJob.Farmer && regionType == RegionType.Grass);
    }
}
