using UnityEngine;

// Player fires missile auto-aimed to the opponent
public class FireMissile : BaseAbility
{
    [Header("Abillity Settings")]
    public GameObject missilePrefab; // Reference to the projectile prefab
    public float spawnOffset = 1f;
    public Transform opponent; // Direct reference to the opponent

    public float projectileSpeed = 10f;
    public int projectileDamage = 10;
    public float projectileLifetime = 5f;

    private string missilePrefabPath = "Prefabs/Missile";

    protected override void Awake()
    {
        base.Awake();
        cooldown = 5f;
        knockbackForce = 3f;
        speedMultiplier = 3f;
    }

    private void Start()
    {
        opponent = GameLogic.Instance.GetOpponent(gameObject);
        animator = GetComponentInChildren<Animator>(); 

        // add audio component
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        AudioClip abilitySound = Resources.Load<AudioClip>("Sounds/missile"); // assign missile sound

        if (abilitySound != null)
        {
            audioSource.clip = abilitySound; 
        }
        else
        {
            Debug.LogError("Ability sound not found at Resources/Sounds/sound!");
        }

        if (opponent == null)
        {
            Debug.LogError("Opponent is not assigned.");
        }
    }

    protected override void Execute()
    {
        player.isAttacking = true;  // mark that player is using an ability
        missilePrefab = Resources.Load<GameObject>(missilePrefabPath);
        if (missilePrefab == null)
        {
            Debug.LogError($"Missile prefab not found at path: {missilePrefabPath}");
        }

        if (opponent == null)
        {
            Debug.LogWarning("No opponent to target.");
            return;
        }
        Vector3 directionToOpponent = (opponent.position - transform.position).normalized; // Direction toward the opponent
        Vector3 spawnPosition = transform.position + directionToOpponent * spawnOffset; // Offset position along the direction

        if (animator != null)
        {
            animator.SetTrigger("FireMissile");
        }

        // Instantiate and initialize the projectile
        PlaySegment(0.5f, 0.42f);
        GameObject missileGO = Instantiate(missilePrefab, spawnPosition, Quaternion.LookRotation(directionToOpponent));
        Missile missile = missileGO.GetComponent<Missile>();

        if (missile != null)
        {
            missile.Initialize(opponent, projectileSpeed, projectileDamage, projectileLifetime, this); // Directly pass the opponent as the target
        }

        Debug.Log("Projectile launched at opponent: " + opponent.name);
        player.isAttacking = false; // mark that player stopped using an ability
    }

    
}
