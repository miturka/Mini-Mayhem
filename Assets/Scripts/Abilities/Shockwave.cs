using UnityEngine;

public class Shockwave : BaseAbility
{
    [Header("Circle Settings")]
    public GameObject circlePrefab; // The prefab for the expanding circle
    public float spawnOffsetY = 0.5f; // Offset to avoid spawning inside the ground

    public float expansionSpeed = 2f; // Speed at which the ring expands
    public float maxRadius = 5f; // Maximum radius of the ring
    public float ringWidth = 0.2f; // Thickness of the ring

    protected override void Execute()
    {
        if (circlePrefab == null)
        {
            Debug.LogError("Circle Prefab is not assigned!");
            return;
        }

        // Spawn the circle at the player's current position with a small Y offset
        Vector3 spawnPosition = transform.position + new Vector3(0, spawnOffsetY, 0);
        GameObject circle = Instantiate(circlePrefab, spawnPosition, Quaternion.identity);

        // Pass this ability to the circle for knockback handling
        CircleScript circleScript = circle.GetComponent<CircleScript>();
        if (circleScript != null)
        {
            circleScript.Initialize(expansionSpeed, maxRadius, ringWidth, this);
        }
    }
}
