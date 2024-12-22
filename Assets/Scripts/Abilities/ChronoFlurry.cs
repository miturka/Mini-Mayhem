using UnityEngine;
using System.Collections;

public class ChronoFlurry : BaseAbility
{
    public int numberOfHits = 5;
    public float hitDamage = 3f;
    public float finisherDamage = 12f;
    public float timeBetweenHits = 0.2f;
    public float attackRange = 2f;

    public Transform opponent;
    public GameObject attackEffectPrefab;
    public AudioClip flurrySound;
    public AudioClip finisherSound;

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
            Debug.LogWarning("No opponent assigned for Chrono Flurry!");
            return;
        }

        if (Vector3.Distance(transform.position, opponent.position) > attackRange)
        {
            Debug.Log("Opponent is out of range.");
            return;
        }

        StartCoroutine(PerformChronoFlurry());
    }

    private IEnumerator PerformChronoFlurry()
    {
        Debug.Log("Chrono Flurry activated!");

        for (int i = 0; i < numberOfHits; i++)
        {
            if (!IsOpponentInRange()) break;
            PerformHit(hitDamage, false);
            PlaySound(flurrySound);
            yield return new WaitForSeconds(timeBetweenHits);
        }

        if (IsOpponentInRange())
        {
            PerformHit(finisherDamage, true);
            PlaySound(finisherSound);
        }
    }

    private void PerformHit(float damage, bool isFinisher)
    {
        if (!IsOpponentInRange()) return;

        // Apply damage
        HealthManager health = opponent.GetComponent<HealthManager>();
        if (health != null)
        {
            health.TakeDamage((int)damage);
            Debug.Log($"{(isFinisher ? "Finisher" : "Hit")} dealt {damage} damage to {opponent.name}.");
        }

        // Apply knockback on finisher
        if (isFinisher)
        {
            ApplyKnockback(opponent.GetComponent<CharacterController>(), transform.position);
        }

        // Play hit effect
        PlayHitEffect();
    }

    private void PlayHitEffect()
    {
        if (attackEffectPrefab != null)
        {
            Instantiate(attackEffectPrefab, opponent.position, Quaternion.identity);
        }
    }

    private void PlaySound(AudioClip sound)
    {
        if (audioSource != null && sound != null)
        {
            audioSource.PlayOneShot(sound);
        }
    }

    private bool IsOpponentInRange()
    {
        return opponent != null && Vector3.Distance(transform.position, opponent.position) <= attackRange;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
