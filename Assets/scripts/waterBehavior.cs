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

    private static readonly Collider2D[] results = new Collider2D[16];

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        rb.linearDamping = 0.5f;
        rb.angularDamping = 0.5f;
    }

    private void FixedUpdate()
    {
        Collider2D[] results = Physics2D.OverlapCircleAll(rb.position, stickDistance);
        int connections = 0;

        foreach (var otherCol in results)
        {
            if (otherCol == col) continue;
            if (!otherCol.TryGetComponent(out WaterParticle other)) continue;

            Vector2 dir = other.rb.position - rb.position;
            float distSqr = dir.sqrMagnitude;

            if (distSqr <= 0f || distSqr >= stickDistance * stickDistance)
                continue;

            connections++;

            float dist = Mathf.Sqrt(distSqr);
            float strength = (stickDistance - dist) * stickStrength;
            Vector2 force = dir * (strength / dist) * Time.fixedDeltaTime;

            if (other.GetInstanceID() > GetInstanceID())
                other.rb.linearVelocity -= force;

            rb.linearVelocity += force;
            rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxVelocity);
        }

        if (connections < minConnections)
        {
            foreach (var otherCol in results)
            {
                if (otherCol == col) continue;
                if (!otherCol.TryGetComponent(out WaterParticle other)) continue;

                Vector2 dir = other.rb.position - rb.position;
                float dist = dir.magnitude;
                if (dist > 0f)
                {
                    Vector2 extraForce = dir.normalized * stickStrength * 0.5f * Time.fixedDeltaTime;
                    rb.linearVelocity += extraForce;
                    rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxVelocity);
                }
            }
        }
    }

}