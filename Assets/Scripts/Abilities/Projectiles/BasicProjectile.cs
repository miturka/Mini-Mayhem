using UnityEngine;

public class BasicProjectile : MonoBehaviour
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
        Destroy(gameObject, 5f); // Destroy after 5 seconds to prevent clutter
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Nieco hitla raketa");
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

        

        // Destroy the projectile on impact
        Destroy(gameObject);

        // Optionally, trigger an explosion or visual effect here
        Debug.Log("Missile hit: " + other.name);
    }
}
