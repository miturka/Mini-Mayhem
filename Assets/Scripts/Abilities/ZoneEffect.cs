using System.Collections.Generic;
using UnityEngine;

public class ZoneEffect : MonoBehaviour
{
    private float damagePerSecond;
    private float slowEffect;
    private float zoneRadius;
    private HashSet<PlayerMovement> affectedPlayers;
    private Transform targetOpponent; // Reference to the specific opponent

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
                affectedPlayers.Add(opponentMovement);
                opponentMovement.SetSpeedMultiplier(slowEffect);
                Debug.Log("Blight Zone applies slow to " + other.name);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Apply damage if the other object matches the passed opponent
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
