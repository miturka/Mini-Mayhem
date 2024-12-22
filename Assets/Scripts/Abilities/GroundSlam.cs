using UnityEngine;
using System.Collections;

public class GroundSlam : BaseAbility
{
    public float grappleRange = 7f;           // Range within which the ability can be used
    public float windUpTime = 0.8f;          // Time the ability takes to wind up
    public float slamDamage = 10f;           // Damage dealt by the slam
    public float immobilizeDuration = 0.5f; // Duration for which the enemy is immobilized

    public Transform opponent;               // Direct reference to the opponent
    public GameObject arenaObject;           // Specific arena object to check for obstacles

    public GameObject windUpEffectPrefab;     // Optional: visual effect for wind-up
    public AudioClip windUpSound;             // Optional: sound effect for wind-up
    private GameObject currentWindUpEffect;   // Store the effect instance to destroy it later

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        opponent = GameLogic.Instance.GetOpponent(gameObject);
        arenaObject = GameObject.Find("Arena");
    }

    protected override void Execute()
    {
        if (opponent == null)
        {
            Debug.LogWarning("No opponent assigned for GroundSlam.");
            return;
        }

        // Check if opponent is within range
        if (Vector3.Distance(transform.position, opponent.position) > grappleRange)
        {
            Debug.Log("Opponent is out of range for GroundSlam.");
            return;
        }

        // Check if there are obstacles in the way
        if (IsObstacleInPath())
        {
            Debug.Log("Obstacle detected between player and opponent. GroundSlam canceled.");
            return;
        }

        StartCoroutine(GrappleAndSlamCoroutine(opponent));
    }

    // Checks for obstacles between the player and the opponent using a raycast
    private bool IsObstacleInPath()
    {
        Vector3 directionToOpponent = (opponent.position - transform.position).normalized;
        float distanceToOpponent = Vector3.Distance(transform.position, opponent.position);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToOpponent, out hit, distanceToOpponent))
        {
            return true; // Obstacle detected
        }

        return false;
    }

    private IEnumerator GrappleAndSlamCoroutine(Transform opponent)
    {
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
            ResetWindUpEffects();
            yield break;
        }

        // Grapple opponent
        ResetWindUpEffects();
        yield return PullOpponentToPlayer(opponent);

        // Immobilize and deal damage
        ImmobilizeAndDamageOpponent(opponent);
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

    // Resets and removes any wind-up effects
    private void ResetWindUpEffects()
    {
        if (currentWindUpEffect != null)
        {
            Destroy(currentWindUpEffect);
        }
    }

    // Gradually pulls the opponent toward the player
    private IEnumerator PullOpponentToPlayer(Transform opponent)
    {
        float pullDuration = 0.5f;
        float elapsedTime = 0f;

        Vector3 startPosition = opponent.position;
        Vector3 targetPosition = transform.position;

        while (elapsedTime < pullDuration)
        {
            elapsedTime += Time.deltaTime;

            // Move the opponent toward the player
            opponent.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / pullDuration);
            yield return null;
        }
    }

    private void ImmobilizeAndDamageOpponent(Transform opponent)
    {
        
        // Deal damage to the opponent
        HealthManager opponentHealth = opponent.GetComponent<HealthManager>();
        if (opponentHealth != null)
        {
            opponentHealth.TakeDamage((int)slamDamage);
            Debug.Log($"Opponent slammed and took {slamDamage} damage!");
        }
    }

}
