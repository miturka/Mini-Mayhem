using UnityEngine;
using System.Collections;

// Script for one circle wave of ability Shockwave
public class CircleScript : MonoBehaviour
{
    public float expansionSpeed = 2f; // Speed at which the ring expands
    public float maxRadius = 5f; // Maximum radius of the ring
    public int segments = 100; // Number of segments to draw the ring (smoothness)
    public float ringWidth = 0.2f; // Thickness of the ring
    public float hitYMaxHeight = 2.2f;  // Max height of player when circle hits player
    public int damage = 10;
    public float hitCooldown = 0.5f; 
    private bool canHit = true;
    private LineRenderer lineRenderer;
    private float currentRadius = 0.5f; // Starting radius of the ring
    private PlayerMovement playerMovement;

    [HideInInspector]
    public BaseAbility parentAbility; // Reference to the ability that spawned this circle
    
    
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
        lineRenderer.useWorldSpace = false;
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
            lineRenderer.SetPosition(i, new Vector3(x, 0, z)); 
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

            // Making sure only the outer part of the sphere collider makes a hit
            position1.y = 0;
            position2.y = 0;
            float distance = Vector3.Distance(position1, position2);
            float offset = 0.1f;
            if (distance < currentRadius - offset){
                return;
            }

            // Apply damage
            HealthManager health = other.GetComponent<HealthManager>();
            if (health != null )
            {
                health.TakeDamage(damage);

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
        canHit = false; 
        yield return new WaitForSeconds(hitCooldown); 
        canHit = true; 
    }

}