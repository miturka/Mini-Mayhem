using System.Collections.Generic;
using UnityEngine;

public class ZoneEffect : MonoBehaviour
{
    private float damagePerSecond;            // Damage dealt per second to the opponent
    private float slowEffect;                 // Slow multiplier applied to the opponent
    private float zoneRadius;                 // Radius of the zone
    private HashSet<PlayerMovement> affectedPlayers; // Set of players affected by the zone
    private Transform targetOpponent;         // Reference to the specific opponent affected by the zone

    public void InitializeZone(float radius, float damage, float slow, float duration, HashSet<PlayerMovement> players, Transform opponent)
    {
        damagePerSecond = damage;
        slowEffect = slow;
        zoneRadius = radius;
        affectedPlayers = players;
        targetOpponent = opponent; // Set the target opponent

        // Set up the zone's collider
        SphereCollider collider = gameObject.AddComponent<SphereCollider>();
        collider.isTrigger = true;
        collider.radius = zoneRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the other object matches the passed opponent
        if (other.transform == targetOpponent)
        {
            PlayerMovement opponentMovement = other.GetComponent<PlayerMovement>();
            if (opponentMovement != null && !affectedPlayers.Contains(opponentMovement))
            {
                // Apply the slow effect to the opponent and track them
                affectedPlayers.Add(opponentMovement);
                opponentMovement.SetSpeedMultiplier(slowEffect);
                Debug.Log("Blight Zone applies slow to " + other.name);
            }
        }
    }

    // Triggered continuously while an object remains in the zone
    private void OnTriggerStay(Collider other)
    {
        if (other.transform == targetOpponent)
        {
            HealthManager opponentHealth = other.GetComponent<HealthManager>();
            if (opponentHealth != null)
            {
                opponentHealth.TakeDamage((int)(damagePerSecond * Time.deltaTime));
                Debug.Log("Blight Zone damages " + other.name);
            }
        }
    }

    // Triggered when an object exits the zone
    private void OnTriggerExit(Collider other)
    {
        // Remove slow if the other object matches the passed opponent
        if (other.transform == targetOpponent)
        {
            PlayerMovement opponentMovement = other.GetComponent<PlayerMovement>();
            if (opponentMovement != null && affectedPlayers.Contains(opponentMovement))
            {
                opponentMovement.ResetSpeedMultiplier();
                affectedPlayers.Remove(opponentMovement);
                Debug.Log("Blight Zone removes slow from " + other.name);
            }
        }
    }
}
