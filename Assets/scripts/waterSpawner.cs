using UnityEngine;

[RequireComponent(typeof(Transform))]
public sealed class SpawnWhenTilted : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private int spawnCount = 1;
    [SerializeField] private float minSpawnInterval = 0.2f;
    [SerializeField] private float maxSpawnInterval = 1.2f;
    [SerializeField] private float spawnedObjectLifetime = 4f;

    [Header("Tilt Thresholds (Degrees)")]
    [SerializeField] private float uprightThreshold = 45f;

    private Transform parentTransform;
    private float spawnTimer;

    // Cached constants
    private float maxDistance;
    private float invMaxDistance;

    private void Awake()
    {
        parentTransform = transform.parent;

        // Precompute constants (avoids recalculation every frame)
        maxDistance = 180f - uprightThreshold;
        invMaxDistance = 1f / maxDistance;
    }

    private void Update()
    {
        if (parentTransform == null)
            return;

        float z = parentTransform.eulerAngles.z;
        z = z > 180f ? z - 360f : z; // Faster than Mathf.DeltaAngle

        float absZ = z >= 0f ? z : -z;

        // Inside upright range -- no spawn
        if (absZ <= uprightThreshold)
        {
            spawnTimer = 0f;
            return;
        }

        spawnTimer += Time.deltaTime;

        // Distance from fully upside-down (180)
        float distance = absZ >= 180f ? 0f : 180f - absZ;

        // Normalize [0..1]
        float t = distance * invMaxDistance;
        t = t < 0f ? 0f : (t > 1f ? 1f : t);

        // SmoothStep inline (faster than Mathf.SmoothStep)
        t = t * t * (3f - 2f * t);

        float spawnInterval = minSpawnInterval + (maxSpawnInterval - minSpawnInterval) * t;

        if (spawnTimer >= spawnInterval)
        {
            SpawnPrefabs();
            spawnTimer = 0f;
        }
    }

    private void SpawnPrefabs()
    {
        Vector3 pos = transform.position;

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject obj = Instantiate(prefabToSpawn, pos, Quaternion.identity);
            Destroy(obj, spawnedObjectLifetime);
        }
    }
}
