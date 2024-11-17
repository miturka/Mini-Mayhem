using System.Collections.Generic;
using UnityEngine;

public class SpinAttack :  BaseAbility
{
    [Header("Abillity Settings")]
    public Collider hitbox;
    public int attackDamage = 10;
    public float attackDuration = 0.2f;
    public float spinDuration = 0.5f;

    private bool isAttacking = false;
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>(); // Track objects hit during an attack

    private void Start()
    {
        if (hitbox != null)
        {
            hitbox.enabled = false;
        }
    }

    protected override void Execute()
    {
        if (!isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private System.Collections.IEnumerator PerformAttack()
    {
        isAttacking = true;
        hitTargets.Clear(); // Clear previously hit targets

        if (hitbox != null)
        {
            hitbox.enabled = true;  // Enable the hitbox for detecting hits
        }

        yield return StartCoroutine(SpinAnimation()); // Perform spin animation
        yield return new WaitForSeconds(attackDuration); // Wait for the attack duration

        if (hitbox != null)
        {
            hitbox.enabled = false; // Disable the hitbox after the attack
        }

        isAttacking = false;
    }

    private System.Collections.IEnumerator SpinAnimation()
    {
        float elapsedTime = 0f;
        float initialRotation = transform.eulerAngles.y;
        float targetRotation = initialRotation + 360f;

        while (elapsedTime < spinDuration)
        {
            elapsedTime += Time.deltaTime;
            float currentRotation = Mathf.Lerp(initialRotation, targetRotation, elapsedTime / spinDuration);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, currentRotation, transform.eulerAngles.z);
            yield return null;
        }

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, targetRotation, transform.eulerAngles.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isAttacking && other.CompareTag("Player") && other.gameObject != gameObject && !hitTargets.Contains(other.gameObject))
        {
            Debug.Log("Hit another player: " + other.name);

            // Add the target to the hit list to prevent repeated hits
            hitTargets.Add(other.gameObject);

            // Apply damage
            HealthManager opponentHealth = other.GetComponent<HealthManager>();
            if (opponentHealth != null)
            {
                opponentHealth.TakeDamage(attackDamage);
            }

            // Apply knockback
            if (opponentHealth.IsAlive())
            {
                CharacterController opponentController = other.GetComponent<CharacterController>();
                if (opponentController != null)
                {
                    ApplyKnockback(opponentController, transform.position);
                }
            }
            
        }
    }
}
