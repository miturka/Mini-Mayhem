using UnityEngine;

public class CircleScript : MonoBehaviour
{
    public float expansionSpeed = 2f; // Speed at which the ring expands
    public float maxRadius = 5f; // Maximum radius of the ring
    public int segments = 100; // Number of segments to draw the ring (smoothness)
    public float ringWidth = 0.2f; // Thickness of the ring

    [HideInInspector]
    public BaseAbility parentAbility; // Reference to the ability that spawned this circle

    private LineRenderer lineRenderer;
    private float currentRadius = 0.5f; // Starting radius of the ring

    public void Initialize(float newSpeed, float newRadius, float newWidth, BaseAbility ability)
    {
        expansionSpeed = newSpeed;
        maxRadius = newRadius ;
        ringWidth = newWidth;
        parentAbility = ability;
    }

    void Start()
    {
        // Get or add the LineRenderer component
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // Configure the LineRenderer
        lineRenderer.loop = true;
        lineRenderer.startWidth = ringWidth;
        lineRenderer.endWidth = ringWidth;
        lineRenderer.useWorldSpace = false; // Keeps the ring local to the GameObject
        lineRenderer.positionCount = segments;
    }

    void Update()
    {
        if (currentRadius < maxRadius)
        {
            // Expand the radius
            currentRadius += expansionSpeed * Time.deltaTime;

            // Update the ring's vertices
            DrawRing(currentRadius);

            // Update the SphereCollider's radius
            SphereCollider sphereCollider = GetComponent<SphereCollider>();
            if (sphereCollider != null)
            {
                sphereCollider.radius = currentRadius;
            }
        }
        else
        {
            // Destroy the ring when it reaches its maximum size
            Destroy(gameObject);
        }
    }

    void DrawRing(float radius)
    {
        float angle = 0f;
        for (int i = 0; i < segments; i++)
        {
            // Calculate the position of each point along the ring
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            float z = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            lineRenderer.SetPosition(i, new Vector3(x, 0, z)); // Y is 0 for a flat ring
            angle += 360f / segments;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore self or unrelated collisions
        if (other.CompareTag("Player") && other.gameObject != parentAbility.gameObject)
        {
            // Apply damage
            HealthManager health = other.GetComponent<HealthManager>();
            if (health != null)
            {
                health.TakeDamage(10);

                // Apply knockback if the player is still alive
                if (health.IsAlive())
                {
                    CharacterController opponentController = other.GetComponent<CharacterController>();
                    if (opponentController != null)
                    {
                        parentAbility.ApplyKnockback(opponentController, transform.position);
                    }
                }
            }
        }
    }
}
