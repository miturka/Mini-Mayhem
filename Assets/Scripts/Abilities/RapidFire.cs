using System.Collections;
using UnityEngine;

// RapidFire ability fires rapidly mutiple projectiles 
public class RapidFire : BaseAbility
{
    [Header("Rapid Fire Settings")]
    public GameObject projectilePrefab;    // Bullet projectile
    public float fireRate = 0.2f;          // Time between each shot
    public float abilityDuration = 3f;     // How long the rapid fire lasts
    public float projectileSpeed = 10f;    // Speed of the projectile
    public int projectileDamage = 4;       // Damage of each projectile
    private bool isFiring = false;


    protected override void Awake()
    {
        base.Awake();
        knockbackForce = 3f;
        speedMultiplier = 3f;
        projectilePrefab = Resources.Load<GameObject>("Prefabs/Bullet");

        if (animator == null)
        {
            Debug.LogError("Animator not found on this object or its children!");
        }
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
        // add sound effect
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = 0.4f;
        AudioClip abilitySound = Resources.Load<AudioClip>("Sounds/rapidfire");

        if (abilitySound != null)
        {
            audioSource.clip = abilitySound;
        }
        else
        {
            Debug.LogError("Ability sound not found at Resources/Sounds/sound!");
        }
    }

    // Start rapid firing
    private IEnumerator RapidFireRoutine()
    {
        isFiring = true;
        player.isAttacking = true;
        float elapsedTime = 0f;
        PlayerMovement movement = GetComponent<PlayerMovement>();
        movement.RapidFireEnabled();

        if (animator != null)
        {
            animator.SetBool("isRapidFiring", true); // start animation
        }

        // fire three projectiles repeatedly
        while (elapsedTime < abilityDuration)
        {
            elapsedTime += fireRate;
            audioSource.Play();


            CreateProjectile(firePoint.position, firePoint.rotation);

            Quaternion leftRotation = Quaternion.Euler(firePoint.eulerAngles + new Vector3(0, -10, 0));
            CreateProjectile(firePoint.position, leftRotation);

            Quaternion rightRotation = Quaternion.Euler(firePoint.eulerAngles + new Vector3(0, 10, 0));
            CreateProjectile(firePoint.position, rightRotation);

            yield return new WaitForSeconds(fireRate);
        }

        if (animator != null)
        {
            animator.SetBool("isRapidFiring", false); // stop animation
        }

        isFiring = false;
        player.isAttacking = false;
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
