using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlightZone : BaseAbility
{
    public Transform opponent;               // Direct reference to the opponent

    public float zoneRadius = 5f;             // Radius of the blight zone effect
    public float windUpTime = 0.3f;             // Wind-up time before the zone activates
    public float zoneDamage = 5f;             // Damage dealt per second to enemies in the zone
    public float slowEffect = 0.5f;           // Slow multiplier (e.g., 0.5 means 50% slower)
    public float zoneDuration = 3f;           // Duration of the zone effect

    public GameObject windUpEffectPrefab;     // Visual effect during the wind-up
    public AudioClip windUpSound;             // Sound effect for the wind-up
    public GameObject zoneEffectPrefab;       // Visual effect for the active zone
    private string zoneEffectPrefabPath = "Prefabs/BlightZoneArea";

    private HashSet<PlayerMovement> affectedPlayers = new HashSet<PlayerMovement>(); // Track affected players
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        zoneEffectPrefab = Resources.Load<GameObject>(zoneEffectPrefabPath);
        animator = GetComponentInChildren<Animator>(); 

        // Assign the opponent by finding it in the scene
        opponent = GameLogic.Instance.GetOpponent(gameObject);
        if (opponent == null)
        {
            Debug.LogError($"Opponent GameObject not found in the scene!");

        }
    }

    protected override void Execute()
    {
        if (opponent == null)
        {
            Debug.LogWarning("Opponent not assigned! Cannot activate Blight Zone.");
            return;
        }

        StartCoroutine(ActivateBlightZone());
    }

    private IEnumerator ActivateBlightZone()
    {

        // Play wind-up effect and sound
        PlayWindUpEffects();

        // Wind-up period before the Blight Zone activates
        yield return new WaitForSeconds(windUpTime);

        // Create the Blight Zone at the opponent's position
        Vector3 zonePosition = transform.position; 
        GameObject zoneInstance = Instantiate(zoneEffectPrefab, zonePosition, Quaternion.identity);

        ZoneEffect zoneEffect = zoneInstance.AddComponent<ZoneEffect>();
        zoneInstance.SetActive(true);

        // Configure the zone effect properties
        zoneEffect.InitializeZone(zoneRadius, zoneDamage, slowEffect, zoneDuration, affectedPlayers, opponent);

        // Zone duration management
        yield return new WaitForSeconds(zoneDuration);
        Destroy(zoneInstance);

        // Clear affected players
        ResetAffectedPlayers();

        Debug.Log("Blight Zone ended.");
    }

    // Plays visual and audio effects for the wind-up phase
    private void PlayWindUpEffects()
    {
        if (windUpEffectPrefab != null)
        {
            Instantiate(windUpEffectPrefab, opponent.position, Quaternion.identity, transform); 
        }
        if (audioSource != null && windUpSound != null)
        {
            audioSource.PlayOneShot(windUpSound);
        }
    }

    // Reset the speed multiplier for all affected players and clears the tracking set
    private void ResetAffectedPlayers()
    {
        foreach (var player in affectedPlayers)
        {
            if (player != null) player.ResetSpeedMultiplier();
        }
        affectedPlayers.Clear();
    }

    // Draw a visual representation of the zone's radius in the editor when selected
    private void OnDrawGizmosSelected()
    {
        // Draw the zone radius around the opponent's position in the editor
        if (opponent != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(opponent.position, zoneRadius);
        }
    }
}
