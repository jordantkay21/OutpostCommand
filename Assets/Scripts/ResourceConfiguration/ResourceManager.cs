using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    [Header("Resource Prefabs")]
    public GameObject woodPrefab;
    public GameObject saplingPrefab;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Spawns resources on the given tile.
    /// </summary>
    /// <param name="tile">The tile where resources will be spawned.</param>
    /// <param name="woodCount">Number of wood resources to spawn.</param>
    /// <param name="saplingCount">Number of saplings to spawn.</param>
    public void SpawnResources(TileData tile, int woodCount, int saplingCount)
    {
        if (tile == null)
        {
            Debug.LogWarning("Tile is null. Cannot spawn resources.");
            return;
        }

        Vector3 spawnPosition = tile.transform.position;
        float tileHeight = GetPrefabHeight(tile.gameObject);

        // Spawn wood resources
        for (int i = 0; i < woodCount; i++)
        {
            Vector3 position = spawnPosition + new Vector3(Random.Range(-0.2f, 0.2f), tileHeight, Random.Range(-0.2f, 0.2f));
            Instantiate(woodPrefab, position, Quaternion.identity);
        }

        // Spawn saplings
        for (int i = 0; i < saplingCount; i++)
        {
            Vector3 position = spawnPosition + new Vector3(Random.Range(-0.2f, 0.2f), tileHeight, Random.Range(-0.2f, 0.2f));
            Instantiate(saplingPrefab, position, Quaternion.identity);
        }
    }

    private float GetPrefabHeight(GameObject prefab)
    {
        // Calculate the height of the prefab from its renderer
        Renderer renderer = prefab.GetComponentInChildren<Renderer>();
        return renderer != null ? renderer.bounds.size.y : 0f;
    }
}
