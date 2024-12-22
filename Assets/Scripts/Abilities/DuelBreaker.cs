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

        // Check if the opponent is within the charge range
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
        if (Physics.Raycast(ray, out hit, 8.0f)) 
        {
            Debug.Log($"Hit object: {hit.collider.name}");
            return true;
        }

        return false;
    }

    private IEnumerator PerformDuelBreaker()
    {
        
        PlayChargeEffects();

        // Charge toward the opponent
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

        // Check for collision with opponent
        if (Vector3.Distance(transform.position, opponent.position) <= 1.5f)
        {
            HandleCollisionWithOpponent();
        }
        else
        {
            Debug.Log("Charge missed the opponent.");
        }
    }

    // Plays the charge's visual and sound effects
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

    // Handles the interaction with the opponent when the charge collides
    private void HandleCollisionWithOpponent()
    {
        

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
