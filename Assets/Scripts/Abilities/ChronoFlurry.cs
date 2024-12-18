using UnityEngine;
using System.Collections;

public class ChronoFlurry : MonoBehaviour, IAbility
{
    public int numberOfHits = 5;
    public float hitDamage = 10f;
    public float finisherDamage = 30f;
    public float timeBetweenHits = 0.2f;
    public float knockbackForce = 5f;
    public float cooldownTime = 6f;
    public float attackRange = 2f;

    public Transform opponent;
    public GameObject attackEffectPrefab;
    public AudioClip flurrySound;
    public AudioClip finisherSound;

    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Activate()
    {
        if (IsOnCooldown())
        {
            Debug.Log("Chrono Flurry is on cooldown.");
            return;
        }

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

    public bool IsOnCooldown()
    {
        return isOnCooldown;
    }

    public float GetCooldown()
    {
        return cooldownTimer;
    }

    private IEnumerator PerformChronoFlurry()
    {
        isOnCooldown = true;
        cooldownTimer = cooldownTime;
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

        while (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            yield return null;
        }

        isOnCooldown = false;
        Debug.Log("Chrono Flurry is ready to use again.");
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
        if (isFinisher) ApplyKnockback();

        // Play hit effect
        PlayHitEffect();
    }

    private void ApplyKnockback()
    {
        Rigidbody rb = opponent.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 knockbackDirection = (opponent.position - transform.position).normalized;
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
        }
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

    private void Update()
    {
        if (isOnCooldown)
        {
            cooldownTimer = Mathf.Max(0, cooldownTimer - Time.deltaTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
