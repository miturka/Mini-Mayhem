using UnityEngine;
using System.Collections;

public class CircleScript : MonoBehaviour
{
    public float expansionSpeed = 2f; // Speed at which the ring expands
    public float maxRadius = 5f; // Maximum radius of the ring
    public int segments = 100; // Number of segments to draw the ring (smoothness)
    public float ringWidth = 0.2f; // Thickness of the rin
    public float hitYMaxHeight = 2.2f;

    public float hitCooldown = 0.5f; // Cooldown medzi zásahmi
    private bool canHit = true;

    [HideInInspector]
    public BaseAbility parentAbility; // Reference to the ability that spawned this circle

    private LineRenderer lineRenderer;
    private float currentRadius = 0.5f; // Starting radius of the ring

    private PlayerMovement playerMovement;

    public void Initialize(float newSpeed, float newRadius, float newWidth, BaseAbility ability, PlayerMovement movement, float YMax)
    {
        expansionSpeed = newSpeed;
        maxRadius = newRadius ;
        ringWidth = newWidth;
        parentAbility = ability;
        playerMovement = movement;
        hitYMaxHeight = YMax;
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
        playerMovement.FreezeMovement();

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
            playerMovement.UnfreezeMovement();
        }
    }

    void DrawRing(float radius)
    {
        float angle = 0f;;
        for (int i = 0; i < segments; i++)
        {
            // Calculate the position of each point along the ring
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            float z = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            lineRenderer.SetPosition(i, new Vector3(x, 0, z)); // Y is 0 for a flat ring
            angle += 360f / segments;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Ignore self or unrelated collisions
        if (canHit && other.CompareTag("Player") && other.gameObject != parentAbility.gameObject && other.transform.position.y < hitYMaxHeight)
        {
            Vector3 position1 = other.transform.position;
            Vector3 position2 = gameObject.transform.position;

            // Nastav y na rovnakú hodnotu (napr. 0) pre obidva objekty
            position1.y = 0;
            position2.y = 0;

            // Vypočítaj vzdialenosť na základe X a Z
            float distance = Vector3.Distance(position1, position2);
            float offset = 0.1f;
            if (distance < currentRadius - offset){
                return;
            }
            // Apply damage
            HealthManager health = other.GetComponent<HealthManager>();
            if (health != null )
            {
                health.TakeDamage(12);

                // Apply knockback if the player is still alive
                if (health.IsAlive())
                {
                    CharacterController opponentController = other.GetComponent<CharacterController>();
                    if (opponentController != null)
                    {
                        parentAbility.ApplyKnockback(opponentController, transform.position);
                    }
                }
                StartCoroutine(HitCooldown());
            }
        }
    }

    private IEnumerator HitCooldown()
    {
        canHit = false; // Zablokuj hitovanie
        yield return new WaitForSeconds(hitCooldown); // Čakaj definovaný čas
        canHit = true; // Povoli hitovanie znova
    }

}