using UnityEngine;
using System.Collections;

// Ability that creates three expanding rings that give hits
public class Shockwave : BaseAbility
{
    [Header("Circle Settings")]
    public GameObject circlePrefab; // Prefab for the expanding circle
    public float spawnOffsetY = 0.5f; // Offset to avoid spawning inside the ground

    public float expansionSpeed = 3.5f; // Speed at which the ring expands
    public float maxRadius = 6f; // Maximum radius of the ring
    public float ringWidth = 0.2f; // Thickness of the ring,

    public int numberOfCircles = 3;

    public float delayBetweenCircles = 0.5f;

    public float hitYMaxHeight = 2.2f;  // Max height of player when circle hits player

    protected override void Awake(){
        base.Awake();
        knockbackForce = 3f;
        speedMultiplier = 3f;
    }

    private void Start(){
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        AudioClip abilitySound = Resources.Load<AudioClip>("Sounds/shockwave");

        if (abilitySound != null)
        {
            audioSource.clip = abilitySound; // assign sound effect
        }
        else
        {
            Debug.LogError("Ability sound not found at Resources/Sounds/sound!");
        }
    }

    public override void Activate()
    {
        // cancel if player is not on the ground
        if (transform.position.y > 1.48){
            return;
        }
        if (IsOnCooldown())
        {
            Debug.Log($"{GetType().Name} is on cooldown.");
            return;
        }
        lastActivationTime = Time.time; // Set the last activation time
        Execute(); 
    }
    protected override void Execute()
    {
        player.isAttacking = true;

        // load circle prefab
        circlePrefab = Resources.Load<GameObject>("Prefabs/ExpandingCircle");
        if (circlePrefab == null)
        {
            Debug.LogError("Circle Prefab is not assigned!");
            return;
        }

        audioSource.Play();
        StartCoroutine(SpawnCirclesCoroutine()); // spawns 3 circles, each after 1 second
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

            // wait before spawning another one
            yield return new WaitForSeconds(delayBetweenCircles);
        }
        player.isAttacking = false;
    }

    
}