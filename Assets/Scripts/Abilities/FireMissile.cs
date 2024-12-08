using UnityEngine;

public class FireMissile : BaseAbility
{
    [Header("Abillity Settings")]
    public GameObject missilePrefab; // Reference to the projectile prefab
    public float spawnOffset = 1f;
    public Transform opponent; // Direct reference to the opponent

    public float projectileSpeed = 10f;
    public int projectileDamage = 20;
    public float projectileLifetime = 5f;

    private string missilePrefabPath = "Prefabs/Missile";

    private void Start()
    {
        opponent = GameLogic.Instance.GetOpponent(gameObject);
        if (opponent == null)
        {
            Debug.LogError("Opponent is not assigned.");
        }
    }

    protected override void Execute()
    {

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

        // Instantiate and initialize the projectile
        GameObject missileGO = Instantiate(missilePrefab, spawnPosition, Quaternion.LookRotation(directionToOpponent));
        Missile missile = missileGO.GetComponent<Missile>();

        if (missile != null)
        {
            missile.Initialize(opponent, projectileSpeed, projectileDamage, projectileLifetime, this); // Directly pass the opponent as the target
        }

        Debug.Log("Projectile launched at opponent: " + opponent.name);
    }
}
