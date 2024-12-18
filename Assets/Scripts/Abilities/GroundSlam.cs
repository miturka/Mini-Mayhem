using UnityEngine;
using System.Collections;

public class GroundSlam : MonoBehaviour, IAbility
{
    public float grappleRange = 3f;           // Range within which the ability can be used
    public float windUpTime = 0.8f;          // Time the ability takes to wind up
    public float slamDamage = 20f;           // Damage dealt by the slam
    public float immobilizeDuration = 0.5f; // Duration for which the enemy is immobilized
    public float cooldownTime = 3f;         // Cooldown time for the ability

    public Transform opponent;               // Direct reference to the opponent
    public GameObject arenaObject;           // Specific arena object to check for obstacles

    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;

    public GameObject windUpEffectPrefab;     // Optional: visual effect for wind-up
    public AudioClip windUpSound;             // Optional: sound effect for wind-up
    private GameObject currentWindUpEffect;   // Store the effect instance to destroy it later
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (opponent == null)
        {
            Debug.LogError("Opponent not assigned for GroundSlam ability.");
        }

        if (arenaObject == null)
        {
            Debug.LogError("Arena object not assigned for GroundSlam ability.");
        }
    }

    public void Activate()
    {
        if (IsOnCooldown())
        {
            Debug.Log("Ability is on cooldown.");
            return;
        }

        if (opponent == null)
        {
            Debug.LogWarning("No opponent assigned for Grapple and Slam.");
            return;
        }

        // Check if opponent is within range
        if (Vector3.Distance(transform.position, opponent.position) > grappleRange)
        {
            Debug.Log("Opponent is out of range for Grapple and Slam.");
            return;
        }

        // Check if there are obstacles in the way
        if (IsObstacleInPath())
        {
            Debug.Log("Obstacle detected between player and opponent. Grapple canceled.");
            return;
        }

        StartCoroutine(GrappleAndSlamCoroutine(opponent));
    }

    private bool IsObstacleInPath()
    {
        // Calculate the direction and distance to the opponent
        Vector3 directionToOpponent = (opponent.position - transform.position).normalized;
        float distanceToOpponent = Vector3.Distance(transform.position, opponent.position);

        // Perform a raycast from the player's position to the opponent's position
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToOpponent, out hit, distanceToOpponent))
        {
            Debug.Log($"Raycast hit something: {hit.collider.name}");
            return true; // Obstacle detected
        }

        // No obstacle in the way
        return false;
    }


    public bool IsOnCooldown()
    {
        return isOnCooldown;
    }

    public float GetCooldown()
    {
        return cooldownTimer;
    }

    private IEnumerator GrappleAndSlamCoroutine(Transform opponent)
    {
        // Start ability logic
        isOnCooldown = true;
        cooldownTimer = cooldownTime;

        // Wind-up phase
        PlayWindUpEffects();

        float elapsedTime = 0f;
        while (elapsedTime < windUpTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Check if opponent is still within range
        if (Vector3.Distance(transform.position, opponent.position) > grappleRange)
        {
            Debug.Log("Opponent dodged the Grapple and Slam.");
            ResetWindUpEffects();
            ResetCooldown();
            yield break;
        }

        // Grapple opponent
        Debug.Log("Opponent grappled!");
        ResetWindUpEffects();
        yield return PullOpponentToPlayer(opponent);

        // Immobilize and deal damage
        ImmobilizeAndDamageOpponent(opponent);

        // Start cooldown timer
        yield return new WaitForSeconds(cooldownTime);
        ResetCooldown();
    }

    private void PlayWindUpEffects()
    {
        if (windUpEffectPrefab != null)
        {
            currentWindUpEffect = Instantiate(windUpEffectPrefab, transform.position, Quaternion.identity, transform);
        }
        if (audioSource != null && windUpSound != null)
        {
            audioSource.PlayOneShot(windUpSound);
        }
    }

    private void ResetWindUpEffects()
    {
        if (currentWindUpEffect != null)
        {
            Destroy(currentWindUpEffect);
        }
    }


    private IEnumerator PullOpponentToPlayer(Transform opponent)
    {
        float pullDuration = 0.5f;
        float elapsedTime = 0f;

        Vector3 startPosition = opponent.position;
        Vector3 targetPosition = transform.position;

        while (elapsedTime < pullDuration)
        {
            elapsedTime += Time.deltaTime;

            // Move opponent toward the player
            opponent.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / pullDuration);

            yield return null;
        }
    }










    private void ImmobilizeAndDamageOpponent(Transform opponent)
    {
        Rigidbody opponentRb = opponent.GetComponent<Rigidbody>();
        if (opponentRb != null)
        {
            opponentRb.isKinematic = true;
        }

        StartCoroutine(WaitAndRestoreOpponent(opponentRb));

        HealthManager opponentHealth = opponent.GetComponent<HealthManager>();
        if (opponentHealth != null)
        {
            opponentHealth.TakeDamage((int)slamDamage);
            Debug.Log($"Opponent slammed and took {slamDamage} damage!");
        }
    }

    private IEnumerator WaitAndRestoreOpponent(Rigidbody opponentRb)
    {
        yield return new WaitForSeconds(immobilizeDuration);
        if (opponentRb != null)
        {
            opponentRb.isKinematic = false;
        }
    }

    private void ResetCooldown()
    {
        isOnCooldown = false;
    }

    private void Update()
    {
        if (isOnCooldown)
        {
            cooldownTimer = Mathf.Max(0, cooldownTimer - Time.deltaTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, grappleRange);
    }
}
