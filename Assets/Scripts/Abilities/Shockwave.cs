using UnityEngine;
using System.Collections;

public class Shockwave : BaseAbility
{
    [Header("Circle Settings")]
    public GameObject circlePrefab; // The prefab for the expanding circle
    public float spawnOffsetY = 0.5f; // Offset to avoid spawning inside the ground

    public float expansionSpeed = 3.5f; // Speed at which the ring expands
    public float maxRadius = 6f; // Maximum radius of the ring
    public float ringWidth = 0.2f; // Thickness of the ring,

    public int numberOfCircles = 3;

    public float delayBetweenCircles = 0.5f;

    public float hitYMaxHeight = 2.2f;

    protected override void Awake(){
        knockbackForce = 3f;
        speedMultiplier = 3f;
    }

    protected override void Execute()
    {
        circlePrefab = Resources.Load<GameObject>("Prefabs/ExpandingCircle");

        if (circlePrefab == null)
        {
            Debug.LogError("Circle Prefab is not assigned!");
            return;
        }

        StartCoroutine(SpawnCirclesCoroutine()); // 3 kruhy, každých 1 sekundu
    }

    private IEnumerator SpawnCirclesCoroutine()
    {
        for (int i = 0; i < numberOfCircles; i++)
        {
            // Spawn the circle at the player's current position with a small Y offset
            Vector3 spawnPosition = transform.position + new Vector3(0, spawnOffsetY, 0);
            GameObject circle = Instantiate(circlePrefab, spawnPosition, Quaternion.identity);

            // Pass this ability to the circle for knockback handling
            CircleScript circleScript = circle.GetComponent<CircleScript>();
            PlayerMovement movement = GetComponent<PlayerMovement>();
            if (circleScript != null)
            {
                circleScript.Initialize(expansionSpeed, maxRadius, ringWidth, this, movement, hitYMaxHeight);
            }

            // Čakaj pred vytvorením ďalšieho kruhu
            yield return new WaitForSeconds(delayBetweenCircles);
        }
    }

    
}