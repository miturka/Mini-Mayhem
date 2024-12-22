using UnityEngine;
using System.Collections;

public class DuelBreaker : BaseAbility
{
    public float chargeSpeed = 15f;          // Speed of the charge
    public float chargeRange = 8f;          // Maximum distance for the charge
    public float damage = 15f;              // Base damage of the ability
    public float bonusDamage = 5f;         // Additional damage for hitting during an opponent's ability

    public Transform opponent;              // Direct reference to the opponent

    public GameObject chargeEffectPrefab;   // Visual effect for the charge
    public AudioClip chargeSound;           // Sound effect for the charge

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        opponent = GameLogic.Instance.GetOpponent(gameObject);
    }

    protected override void Execute()
    {
        if (opponent == null)
        {
            Debug.LogWarning("Opponent not assigned! Cannot activate Duel Breaker.");
            return;
        }

        float distanceToOpponent = Vector3.Distance(transform.position, opponent.position);
        if (distanceToOpponent > chargeRange)
        {
            Debug.Log("Opponent is out of range.");
            return;
        }

        StartCoroutine(PerformDuelBreaker());
    }

    // Function to check for an obstacle in the direction of the opponent
    private bool IsObstacleInDirection(Vector3 targetPosition)
    {
        // Calculate the direction to the opponent
        Vector3 directionToOpponent = (targetPosition - transform.position).normalized;
        Ray ray = new Ray(transform.position, directionToOpponent);
        RaycastHit hit;

        // Raycast toward the opponent to detect obstacles
        if (Physics.Raycast(ray, out hit, 1.0f)) // Adjust distance (1.0f) as needed
        {
            Debug.Log($"Hit object: {hit.collider.name}");
            return true;
        }

        return false;
    }

    private IEnumerator PerformDuelBreaker()
    {
        Debug.Log("Duel Breaker activated!");

        // Step 1: Play charge effect and sound
        PlayChargeEffects();

        // Step 2: Charge toward the opponent
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = opponent.position;
        float elapsedTime = 0f;
        float chargeDuration = Vector3.Distance(startPosition, targetPosition) / chargeSpeed;

        while (elapsedTime < chargeDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / chargeDuration);

            // Check if there is an obstacle in the direction of the opponent
            if (IsObstacleInDirection(targetPosition))
            {
                Debug.Log("Obstacle detected in the direction of the opponent. Stopping charge.");
                yield break;
            }

            // Check if opponent moved out of range
            if (Vector3.Distance(transform.position, targetPosition) > chargeRange)
            {
                Debug.Log("Opponent moved out of range during the charge.");
                yield break;
            }

            yield return null;
        }

        // Step 3: Check for collision with opponent
        if (Vector3.Distance(transform.position, opponent.position) <= 1.5f)
        {
            HandleCollisionWithOpponent();
        }
        else
        {
            Debug.Log("Charge missed the opponent.");
        }
    }

    private void PlayChargeEffects()
    {
        if (chargeEffectPrefab != null)
        {
            Instantiate(chargeEffectPrefab, transform.position, Quaternion.identity);
        }
        if (audioSource != null && chargeSound != null)
        {
            audioSource.PlayOneShot(chargeSound);
        }
    }

    private void HandleCollisionWithOpponent()
    {
        Debug.Log("Duel Breaker hit the opponent!");

        // Apply knockback
        Rigidbody opponentRb = opponent.GetComponent<Rigidbody>();
        if (opponentRb != null)
        {
            Vector3 knockbackDirection = (opponent.position - transform.position).normalized;
            opponentRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
        }

        // Apply damage
        HealthManager opponentHealth = opponent.GetComponent<HealthManager>();
        if (opponentHealth != null)
        {
            if (!opponentHealth.IsAlive())
            {
                Debug.Log("Opponent is already defeated.");
                return;
            }

            float totalDamage = damage;

            // Apply bonus damage if the opponent is using an ability
            if (opponent.TryGetComponent(out IAbility opponentAbility) && opponentAbility.IsOnCooldown())
            {
                totalDamage += bonusDamage;
                Debug.Log("Opponent was using an ability! Bonus damage applied.");
            }

            opponentHealth.TakeDamage((int)totalDamage);
            Debug.Log($"Opponent took {totalDamage} damage! Remaining health: {opponentHealth}");

            if (!opponentHealth.IsAlive())
            {
                Debug.Log("Opponent has been defeated!");
            }
        }
        else
        {
            Debug.LogWarning("Opponent does not have a HealthManager component!");
        }
    }
}
