using UnityEngine;

public class Missile : MonoBehaviour
{
    public float speed; // Speed of the projectile
    public float rotationSpeed = 200f; // Rotation speed for homing
    public int damage; // Damage dealt by the projectile
    public float lifetime; // Time before the projectile self-destructs

    private Transform target; // Target to home in on
    private BaseAbility parentAbility;

    public void Initialize(Transform newTarget, float newSpeed, int newDamage, float newLifeTime, BaseAbility ability)
    {
        target = newTarget;
        speed = newSpeed;
        damage = newDamage;
        lifetime = newLifeTime;
        parentAbility = ability;

        Destroy(gameObject, lifetime);
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
        Debug.Log("Nieco hitla raketa");
        if (other.CompareTag("Player") && other.transform != target)
        {
            return; // Ignore collisions with the firing player
        }

        // Apply damage if the projectile hits the target
        HealthManager health = other.GetComponent<HealthManager>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        CharacterController opponentController = other.GetComponent<CharacterController>();
        if (opponentController != null)
        {
            parentAbility.ApplyKnockback(opponentController, transform.position);
        }

        // Destroy the projectile on impact
        Destroy(gameObject);

        // Optionally, trigger an explosion or visual effect here
        Debug.Log("Missile hit: " + other.name);
    }
}
