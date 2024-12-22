using System.Collections.Generic;
using UnityEngine;

// Basic quick jab ability attack
public class QuickJab :  BaseAbility
{
    [Header("Abillity Settings")]
    public Collider hitbox;
    public int attackDamage = 2;
    public float attackDuration = 0.2f;
    private bool isAttacking = false;
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>(); // Track objects hit during an attack

    protected override void Awake()
    {
        base.Awake();
        cooldown = 0.5f;
        knockbackForce = 1.6f;
        speedMultiplier = 5.0f;
    }

    private void Start()
    {
        if (hitbox != null)
        {
            hitbox.enabled = false;
        }

        // add audio component
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        AudioClip abilitySound = Resources.Load<AudioClip>("Sounds/basicattack2");

        if (abilitySound != null)
        {
            audioSource.clip = abilitySound; 
        }
        else
        {
            Debug.LogError("Ability sound not found at Resources/Sounds/x!");
        }
    }

    protected override void Execute()
    {
        player.isAttacking = true;  // mark that player is using an ability
        hitbox = transform.Find("PunchHitbox").GetComponent<CapsuleCollider>();
        Player playerScript = GetComponent<Player>();
        
        if (!isAttacking && !playerScript.isHitBySpinAttack)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private System.Collections.IEnumerator PerformAttack()
    {
        isAttacking = true;
        hitTargets.Clear(); // Clear previously hit targets

        if (animator != null)
        {
            animator.SetTrigger("BaseAttack");  //trigger animation
        }
        else
        {
            Debug.LogError("Animator not found on the player!");
        }

        if (hitbox != null)
        {
            hitbox.enabled = true;  // Enable the hitbox for detecting hits
        }
        else{
            Debug.Log("hitbox je nullllllll");
        }

        yield return new WaitForSeconds(attackDuration); // Wait for the attack duration

        if (hitbox != null)
        {
            hitbox.enabled = false; // Disable the hitbox after the attack
        }

        isAttacking = false;
        player.isAttacking = false; // mark that player stopped using an ability
    }

    /*private System.Collections.IEnumerator SpinAnimation()
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
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (isAttacking && other.CompareTag("Player") && other.gameObject != gameObject && !hitTargets.Contains(other.gameObject))
        {
            audioSource.Play();
            Debug.Log("Hit another player: " + other.name);

            Player otherplayerScript = other.GetComponent<Player>();
            otherplayerScript.HitBySpinAttack();

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
