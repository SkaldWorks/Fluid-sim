using UnityEngine;

public class SpawnWhenTilted : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject prefabToSpawn;
    public int spawnCount = 1;

    [Tooltip("Fastest spawn rate (seconds) when fully upside-down")]
    public float minSpawnInterval = 0.2f;

    [Tooltip("Slowest spawn rate (seconds) when just past 45° tilt")]
    public float maxSpawnInterval = 1.2f;

    [Tooltip("How long spawned objects live before being destroyed")]
    public float spawnedObjectLifetime = 4f;

    [Header("Tilt Thresholds (Degrees)")]
    [Tooltip("Spawn outside this range: -uprightThreshold to uprightThreshold")]
    public float uprightThreshold = 45f;

    private float spawnTimer;

    void Update()
    {
        if (transform.parent == null)
            return;

        // Get parent rotation normalized to -180 - 180
        float currentZ = Mathf.DeltaAngle(0f, transform.parent.eulerAngles.z);

        // Spawn when outside the upright threshold
        if (currentZ > uprightThreshold || currentZ < -uprightThreshold)
        {
            spawnTimer += Time.deltaTime;

            // Dynamic spawn speed: faster when closer to fully upside-down (180/-180)
            float spawnInterval = CalculateDynamicSpawnInterval(currentZ);

            if (spawnTimer >= spawnInterval)
            {
                SpawnPrefabs();
                spawnTimer = 0f;
            }
        }
        else
        {
            spawnTimer = 0f; // reset timer when within upright range
        }
    }

    void SpawnPrefabs()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            GameObject spawned = Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
            Destroy(spawned, spawnedObjectLifetime);
        }
    }

    float CalculateDynamicSpawnInterval(float currentZ)
    {
        // Distance from fully upside-down
        float distanceFromUpsideDown = Mathf.Abs(Mathf.DeltaAngle(currentZ, 180f));

        // Normalized: 0 = fully upside-down, 1 = just past 45° threshold
        float maxDistance = 180f - uprightThreshold;
        float normalized = Mathf.Clamp01(distanceFromUpsideDown / maxDistance);

        // Smooth ramp for natural feel
        normalized = Mathf.SmoothStep(0f, 1f, normalized);

        // Interpolate spawn interval
        return Mathf.Lerp(minSpawnInterval, maxSpawnInterval, normalized);
    }
}
