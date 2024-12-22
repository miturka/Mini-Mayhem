using UnityEngine;

public abstract class BaseAbility : MonoBehaviour, IAbility
{
    public float cooldown = 3f; // Cooldown duration for the ability
    public float lastActivationTime = -Mathf.Infinity; // Last time the ability was activated

    public Player player;

    public AudioSource audioSource;

    [Header("Knockback Settings")]
    public float knockbackForce = 10f; // Force of the knockback
    public float speedMultiplier = 1.5f;
    public Vector3 knockbackDirectionOffset = new Vector3(0, 1, 0); // Default upward offset
    protected Animator animator; 

    protected Transform firePoint;

    protected virtual void Awake()
    {
        // Automatically find the FirePoint child object
        firePoint = transform.Find("FirePoint");

        if (firePoint == null)
        {
            Debug.LogError($"FirePoint not found on {gameObject.name}. Make sure it exists as a child object.");
        }

        player = GetComponent<Player>();

        if (player == null)
        {
            Debug.LogError($"Player not found on {gameObject.name}. Make sure it exists as a componotenetnetnentetntn.");
        }
        animator = GetComponentInChildren<Animator>();
    }

    public virtual void Activate()
    {

        if (IsOnCooldown())
        {
            Debug.Log($"{GetType().Name} is on cooldown.");
            return;
        }
        lastActivationTime = Time.time; // Set the last activation time
        Execute(); // Call the custom logic of the derived ability
    }

    // Cooldown check
    public bool IsOnCooldown()
    {
        return Time.time < lastActivationTime + cooldown;
    }

    public float GetCooldown()
    {
        return cooldown;
    }

    protected abstract void Execute();

    internal void ApplyKnockbackLegacy(CharacterController target, Vector3 knockbackOrigin)
    {
        Vector3 knockbackDirection = (target.transform.position - knockbackOrigin).normalized + knockbackDirectionOffset;
        float knockbackDuration = 0.2f;
        StartCoroutine(ApplyKnockbackCoroutineLegacy(target, knockbackDirection, knockbackForce, knockbackDuration));
    }

    private System.Collections.IEnumerator ApplyKnockbackCoroutineLegacy(CharacterController target, Vector3 direction, float force, float duration)
    {
        PlayerMovement targetMovement = target.gameObject.GetComponent<PlayerMovement>();
        targetMovement.FreezeMovement();
        float elapsedTime = 0f;
        Vector3 velocity = direction.normalized * force;

        while (elapsedTime < duration)
        {
            target.Move(velocity * Time.deltaTime);

            // Apply gravity over time for more realistic effect
            velocity.y += Physics.gravity.y * Time.deltaTime;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        targetMovement.UnfreezeMovement();
    }

    internal void ApplyKnockback(CharacterController target, Vector3 knockbackOrigin)
    {
        PlayerMovement p = target.GetComponent<PlayerMovement>();
        p.ReceiveKnockback(knockbackOrigin, knockbackDirectionOffset, knockbackForce, speedMultiplier);
    }

    public void PlaySegment(float startTime, float duration)
    {
        if (audioSource.clip == null)
        {
            Debug.LogError("No audio clip assigned to the AudioSource.");
            return;
        }

        if (startTime < 0 || startTime > audioSource.clip.length)
        {
            Debug.LogError("Invalid start time.");
            return;
        }

        if (duration <= 0 || (startTime + duration) > audioSource.clip.length)
        {
            Debug.LogError("Invalid duration.");
            return;
        }

        // Nastavenie začiatku prehrávania
        audioSource.time = startTime;
        audioSource.Play();

        // Naplánovanie zastavenia zvuku po trvaní segmentu
        Invoke(nameof(StopAudio), duration);
    }

    private void StopAudio()
    {
        audioSource.Stop();
    }

    /*internal void ApplyKnockbackV2(CharacterController target, Vector3 knockbackOrigin)
    {
        Vector3 knockbackDirection = (target.transform.position - knockbackOrigin).normalized + knockbackDirectionOffset;
        StartCoroutine(ApplyKnockbackCoroutineV2(target, knockbackDirection, knockbackForce));
    }
    
    private System.Collections.IEnumerator ApplyKnockbackCoroutineV2(CharacterController target, Vector3 direction, float force)
    {
        PlayerMovement targetMovement = target.gameObject.GetComponent<PlayerMovement>();
        targetMovement.FreezeMovement();

        // Zvýšenie rýchlosti pomocou multiplier
        Vector3 horizontalVelocity = new Vector3(direction.x, 0, direction.z).normalized * force * speedMultiplier;
        float verticalVelocity = Mathf.Max(0, direction.y) * force * speedMultiplier;

        float knockbackTimer = 0.2f; // Čas, kedy je hráč "nútene vo vzduchu"
        bool forceAirborne = true;

        while (forceAirborne || !target.isGrounded)
        {
            if (knockbackTimer > 0)
            {
                knockbackTimer -= Time.deltaTime;
            }
            else
            {
                forceAirborne = false;
            }

            // Rýchlejšie pridávanie gravitácie (zrýchlený pohyb dole)
            verticalVelocity += Physics.gravity.y * Time.deltaTime * speedMultiplier;

            // Kombinácia zrýchlenej horizontálnej a vertikálnej zložky
            Vector3 velocity = horizontalVelocity + Vector3.up * verticalVelocity;

            // Rýchlejší pohyb
            target.Move(velocity * Time.deltaTime);

            yield return null;
        }

        targetMovement.UnfreezeMovement();
    }*/
}
