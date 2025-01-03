using UnityEngine;

// Missile script for FireMissile ability
public class Missile : MonoBehaviour
{
    public float speed; // Speed of the projectile
    public float rotationSpeed = 200f; // Rotation speed for homing
    public int damage; 
    public float lifetime; // Time before the projectile self-destructs

    private AudioSource audioSource;
    private Transform target; // Target to home in on
    private BaseAbility parentAbility;

    public void Initialize(Transform newTarget, float newSpeed, int newDamage, float newLifeTime, BaseAbility ability)
    {
        target = newTarget;
        speed = newSpeed;
        damage = newDamage;
        lifetime = newLifeTime;
        parentAbility = ability;

        Destroy(gameObject, lifetime);  // Destroy rocket after lifetime
    }

    private void Update()
    {
        if (target == null)
        {
            // If target is lost (e.g., destroyed), destroy the projectile
            Destroy(gameObject);
            return;
        }

        // Homing logic
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

        // Move the projectile forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.transform != target)
        {
            return; // Ignore collisions with the firing player
        }

        if (other.CompareTag("ArenaArea")){
            return; // Ignore arena area
        }

        // Apply damage if the projectile hits the target
        HealthManager health = other.GetComponent<HealthManager>();
        if (health != null)
        {
            health.TakeDamage(damage);
            if (health.IsAlive()){
            CharacterController opponentController = other.GetComponent<CharacterController>();
            if (opponentController != null)
            {
                parentAbility.ApplyKnockback(opponentController, transform.position);
            }
        }
        }
        parentAbility.PlaySegment(1.0f, 2.3f);  // play explosion sound

        // Destroy the projectile on impact
        Debug.Log("Missile hit: " + other.name);

        Destroy(gameObject);        
    }
}
