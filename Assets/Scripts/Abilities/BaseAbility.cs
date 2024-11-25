using UnityEngine;

public abstract class BaseAbility : MonoBehaviour, IAbility
{
    public float cooldown = 3f; // Cooldown duration for the ability
    private float lastActivationTime = -Mathf.Infinity; // Last time the ability was activated

    [Header("Knockback Settings")]
    public float knockbackForce = 10f; // Force of the knockback
    public float knockbackDuration = 0.2f; // Duration of the knockback effect
    public Vector3 knockbackDirectionOffset = new Vector3(0, 1, 0); // Default upward offset

    protected Transform firePoint;

    private void Awake()
    {
        // Automatically find the FirePoint child object
        firePoint = transform.Find("FirePoint");

        if (firePoint == null)
        {
            Debug.LogError($"FirePoint not found on {gameObject.name}. Make sure it exists as a child object.");
        }
    }

    public void Activate()
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

    internal void ApplyKnockback(CharacterController target, Vector3 knockbackOrigin)
    {
        Vector3 knockbackDirection = (target.transform.position - knockbackOrigin).normalized + knockbackDirectionOffset;
        StartCoroutine(ApplyKnockbackCoroutine(target, knockbackDirection, knockbackForce, knockbackDuration));
    }

    private System.Collections.IEnumerator ApplyKnockbackCoroutine(CharacterController target, Vector3 direction, float force, float duration)
    {
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
    }
}
