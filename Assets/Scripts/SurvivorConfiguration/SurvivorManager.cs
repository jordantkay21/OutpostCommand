using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SurvivorManager : MonoBehaviour
{
    public static SurvivorManager Instance { get; private set; }

    [Header("Survivor Settings")]
    public GameObject survivorPrefab;
    public LayerMask survivorLayer; // Layer mask for detecting survivors
    public GameObject JobPanel;
    public List<string> SurvivorNames = new List<string>();

    [Header("Job Materials")]
    public Material HighlightMaterial;
    public Material FarmerMaterial;
    public Material LumberJackMaterial;

    private Dictionary<SurvivorJob, JobBase> jobRegistry = new Dictionary<SurvivorJob, JobBase>();
    private Survivor selectedSurvivor;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Register jobs dynamically
        RegisterJob(new FarmerJob());
        RegisterJob(new LumberjackJob());        
    }

    void Update()
    {
        HandleSelection();
    }

    public void SpawnSurvivorsAroundHouse(Vector3 houseCenter, float regionSize)
    {
        //Offset Positions: move 1.5 cells away (house boundry + half a cell)
        float edgeOffset = regionSize / 2 - .5f;

        Vector3 leftCenter = houseCenter + new Vector3(-edgeOffset, 0, 0);
        Vector3 rightCenter = houseCenter + new Vector3(edgeOffset, 0, 0);

        GameObject survivor1 = Instantiate(survivorPrefab, leftCenter + new Vector3(0, GetPrefabHeight(survivorPrefab) / 2, 0), Quaternion.identity);
        survivor1.name = GetSurvivorName();
        
        GameObject survivor2 = Instantiate(survivorPrefab, rightCenter + new Vector3(0, GetPrefabHeight(survivorPrefab) / 2, 0), Quaternion.identity);
        survivor2.name = GetSurvivorName();
    }

    private void RegisterJob(JobBase job)
    {
        if (!jobRegistry.ContainsKey(job.JobType))
        {
            jobRegistry[job.JobType] = job;
        }
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
            selectedSurvivor.SetMaterial(selectedSurvivor.originalMaterial);
        }

        // Select new survivor
        selectedSurvivor = survivor;
        selectedSurvivor.SetMaterial(HighlightMaterial, false);
        JobPanel.SetActive(true);
        Debug.Log($"Selected {survivor.name}");
    }

    public void AssignJobToSelectedSurvivor(SurvivorJob jobType)
    {
        if (selectedSurvivor == null)
        {
            Debug.LogWarning("No survivor selected.");
            return;
        }

        if(jobRegistry.TryGetValue(jobType, out JobBase job))
        {
            job.Assign(selectedSurvivor);
        }
        else
        {
            Debug.LogWarning($"Job type {jobType} not registered.");
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

    public float GetPrefabHeight(GameObject prefab)
    {
        return prefab.GetComponentInChildren<Renderer>().bounds.size.y;
    }

    public string GetSurvivorName()
    {
        int randomIndex = Random.Range(0, SurvivorNames.Count);
        string name = SurvivorNames[randomIndex];

        SurvivorNames.RemoveAt(randomIndex);

        return name;
        
    }
}
