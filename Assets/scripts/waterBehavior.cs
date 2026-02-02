using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class WaterParticle : MonoBehaviour
{
    [Header("Water Settings")]
    [SerializeField] private float stickDistance = 0.3f;
    [SerializeField] private float stickStrength = 5f;
    [SerializeField] private float maxVelocity = 5f;
    [SerializeField] private int minConnections = 4;

    private Rigidbody2D rb;
    private Collider2D col;

    private static readonly Collider2D[] overlapResults = new Collider2D[12];

    private int updateOffset;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        rb.linearDamping = 0.5f;
        rb.angularDamping = 0.5f;

        // Spread work across frames
        updateOffset = GetInstanceID() & 1;
    }

    private void FixedUpdate()
    {
        if ((Time.frameCount & 1) != updateOffset)
            return;

        // Use the new OverlapCircle API
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            rb.position,
            stickDistance
        );

        int connections = 0;
        float stickDistSqr = stickDistance * stickDistance;

        for (int i = 0; i < hits.Length; i++)
        {
            var otherCol = hits[i];
            if (otherCol == col) continue;

            if (!otherCol.TryGetComponent(out WaterParticle other))
                continue;

            // Prevent double force application
            if (other.GetInstanceID() <= GetInstanceID())
                continue;

            Vector2 dir = other.rb.position - rb.position;
            float distSqr = dir.sqrMagnitude;

            if (distSqr <= 0f || distSqr >= stickDistSqr)
                continue;

            connections++;

            float dist = Mathf.Sqrt(distSqr);
            float forceMag = (stickDistance - dist) * stickStrength;
            Vector2 force = dir * (forceMag / dist) * Time.fixedDeltaTime;

            rb.linearVelocity += force;
            other.rb.linearVelocity -= force;
        }

        if (connections < minConnections)
        {
            Vector2 centerPull = Vector2.zero;

            for (int i = 0; i < hits.Length; i++)
            {
                var otherCol = hits[i];
                if (otherCol == col) continue;

                if (!otherCol.TryGetComponent(out WaterParticle other))
                    continue;

                centerPull += (other.rb.position - rb.position);
            }

            if (centerPull != Vector2.zero)
            {
                rb.linearVelocity +=
                    centerPull.normalized *
                    stickStrength *
                    0.4f *
                    Time.fixedDeltaTime;
            }
        }

        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxVelocity);
    }

}

