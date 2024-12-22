using UnityEngine;
using System.Collections;

public class ChronoFlurry : BaseAbility
{
    public int numberOfHits = 5;              // Number of rapid hits in the flurry
    public float hitDamage = 3f;             // Damage dealt by each rapid hit
    public float finisherDamage = 12f;       // Damage dealt by the finishing attack
    public float timeBetweenHits = 0.2f;     // Time delay between each rapid hit
    public float attackRange = 2f;           // Maximum range within which the ability can hit the opponent

    public Transform opponent;               // Reference to the opponent's transform
    public GameObject attackEffectPrefab;    // Prefab for visual effects during hits
    public AudioClip flurrySound;            // Sound played during rapid hits
    public AudioClip finisherSound;          // Sound played during the finisher attack

    private AudioSource audioSource;         // Audio source component for playing sounds

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        opponent = GameLogic.Instance.GetOpponent(gameObject);
    }

    protected override void Execute()
    {
        if (opponent == null)
        {
            Debug.LogWarning("No opponent assigned for Chrono Flurry!");
            return;
        }

        // Check if the opponent is within the attack range
        if (Vector3.Distance(transform.position, opponent.position) > attackRange)
        {
            Debug.Log("Opponent is out of range.");
            return;
        }

        StartCoroutine(PerformChronoFlurry());
    }

    private IEnumerator PerformChronoFlurry()
    {
        // Perform the series of rapid hits
        for (int i = 0; i < numberOfHits; i++)
        {
            if (!IsOpponentInRange()) break;
            PerformHit(hitDamage, false);
            PlaySound(flurrySound);
            yield return new WaitForSeconds(timeBetweenHits);
        }

        // Perform the finisher if the opponent is still in range
        if (IsOpponentInRange())
        {
            PerformHit(finisherDamage, true);
            PlaySound(finisherSound);
        }
    }

    // Performs a hit on the opponent, applying damage and optional knockback for the finisher
    private void PerformHit(float damage, bool isFinisher)
    {
        if (!IsOpponentInRange()) return;

        // Apply damage
        HealthManager health = opponent.GetComponent<HealthManager>();
        if (health != null)
        {
            health.TakeDamage((int)damage);
         

        // Apply knockback on finisher
        if (isFinisher)
        {
            ApplyKnockback(opponent.GetComponent<CharacterController>(), transform.position);
        }

        // Play hit effect
        PlayHitEffect();
    }

    // Plays the visual effect for a hit
    private void PlayHitEffect()
    {
        if (attackEffectPrefab != null)
        {
            Instantiate(attackEffectPrefab, opponent.position, Quaternion.identity);
        }
    }

    // Plays the specified audio clip
    private void PlaySound(AudioClip sound)
    {
        if (audioSource != null && sound != null)
        {
            audioSource.PlayOneShot(sound);
        }
    }

    // Checks if the opponent is within attack range
    private bool IsOpponentInRange()
    {
        return opponent != null && Vector3.Distance(transform.position, opponent.position) <= attackRange;
    }

    // Draws a red wireframe sphere in the editor to visualize the attack range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
