using UnityEngine;

// Bullet Script for RapidFire ability
public class Bullet : MonoBehaviour
{
    private float speed;
    private int damage;
    private Vector3 direction;
    private BaseAbility parentAbility;
    public void Initialize(float newSpeed, int newDamage, Vector3 newDirection, BaseAbility ability)
    {
        speed = newSpeed;
        damage = newDamage;
        direction = newDirection;
        parentAbility = ability;
        Destroy(gameObject, 5f); // Destroy after 5 seconds 
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Player") && other.transform == parentAbility.transform) || other.CompareTag("Bullet")) 
        {
            return; // Ignore collisions with the firing player or other bullet
        }

        if (other.CompareTag("ArenaArea")){
            return; // Ignore area of the arena
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
        Destroy(gameObject);
    }
}
