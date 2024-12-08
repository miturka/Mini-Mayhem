using System.Collections;
using UnityEngine;

public class RapidFire : BaseAbility
{
    [Header("Rapid Fire Settings")]
    public GameObject projectilePrefab;    // The projectile to shoot
    public float fireRate = 0.2f;          // Time between each shot
    public float abilityDuration = 3f;     // How long the rapid fire lasts
    public float projectileSpeed = 10f;    // Speed of the pcrojectile
    public int projectileDamage = 5;       // Damage of each projectile

    public float rotationSpeed = 200f; 

    private bool isFiring = false;

    protected override void Execute()
    {
        if (!isFiring)
        {
            StartCoroutine(RapidFireRoutine());
        }
    }

    private IEnumerator RapidFireRoutine()
    {
        projectilePrefab = Resources.Load<GameObject>("Prefabs/BasicMissile");
        isFiring = true;
        float elapsedTime = 0f;
        PlayerMovement movement = GetComponent<PlayerMovement>();
        movement.RapidFireEnabled();

        while (elapsedTime < abilityDuration)
        {
            elapsedTime += fireRate;

            // Create a projectile
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Bullet basicProjectile = projectile.GetComponent<Bullet>();

            // Initialize the projectile's properties
            if (basicProjectile != null)
            {
                basicProjectile.Initialize(projectileSpeed, projectileDamage, firePoint.forward, this);
            }

            yield return new WaitForSeconds(fireRate);
        }

        isFiring = false;
        movement.RapidFireDisabled();
    }

}
