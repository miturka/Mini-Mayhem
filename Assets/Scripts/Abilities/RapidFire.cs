using System.Collections;
using UnityEngine;

public class RapidFire : BaseAbility
{
    [Header("Rapid Fire Settings")]
    public GameObject projectilePrefab;    // The projectile to shoot
    public float fireRate = 0.2f;          // Time between each shot
    public float abilityDuration = 3f;     // How long the rapid fire lasts
    public float projectileSpeed = 10f;    // Speed of the pcrojectile
    public int projectileDamage = 4;       // Damage of each projectile

    public float rotationSpeed = 200f; 

    private bool isFiring = false;

    protected override void Awake()
    {
        base.Awake();
        knockbackForce = 3f;
        speedMultiplier = 3f;
        projectilePrefab = Resources.Load<GameObject>("Prefabs/BasicMissile");
    }

    protected override void Execute()
    {

        if (firePoint == null)
        {
            Debug.LogError("FirePoint is not set. Please assign it in the Inspector or initialize it in the script.");
        }
        if (!isFiring)
        {
            StartCoroutine(RapidFireRoutine());
        }
    }

    private void Start()
{
    if (firePoint == null)
    {
        Debug.LogError("FirePoint is not set. Please assign it in the Inspector or initialize it in the script.");
    }
}

    private IEnumerator RapidFireRoutine()
    {
        isFiring = true;
        float elapsedTime = 0f;
        PlayerMovement movement = GetComponent<PlayerMovement>();
        movement.RapidFireEnabled();

        while (elapsedTime < abilityDuration)
        {
            elapsedTime += fireRate;

            // Stredný projektil (hlavný smer)
            CreateProjectile(firePoint.position, firePoint.rotation);

            // Ľavý projektil (mierne doľava)
            Quaternion leftRotation = Quaternion.Euler(firePoint.eulerAngles + new Vector3(0, -10, 0));
            CreateProjectile(firePoint.position, leftRotation);

            // Pravý projektil (mierne doprava)
            Quaternion rightRotation = Quaternion.Euler(firePoint.eulerAngles + new Vector3(0, 10, 0));
            CreateProjectile(firePoint.position, rightRotation);

            yield return new WaitForSeconds(fireRate);
        }

        isFiring = false;
        movement.RapidFireDisabled();
    }

    private void CreateProjectile(Vector3 spawnPosition, Quaternion rotation)
    {
        // Create a projectile
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, rotation);
        Bullet basicProjectile = projectile.GetComponent<Bullet>();

        // Initialize the projectile's properties
        if (basicProjectile != null)
        {
            basicProjectile.Initialize(projectileSpeed, projectileDamage, rotation * Vector3.forward, this);
        }
    }


}
