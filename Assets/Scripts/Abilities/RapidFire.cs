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
        isFiring = true;
        float elapsedTime = 0f;

        while (elapsedTime < abilityDuration)
        {
            elapsedTime += fireRate;

            // Create a projectile
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            BasicProjectile basicProjectile = projectile.GetComponent<BasicProjectile>();

            // Initialize the projectile's properties
            if (basicProjectile != null)
            {
                basicProjectile.Initialize(projectileSpeed, projectileDamage, firePoint.forward, this);
            }

            yield return new WaitForSeconds(fireRate);
        }

        isFiring = false;
    }
}
