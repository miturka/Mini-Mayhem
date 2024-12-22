using System.Collections.Generic;
using UnityEngine;

public class CooldownManager : MonoBehaviour
{
    private Dictionary<string, float> cooldowns = new Dictionary<string, float>();

    // Starts the cooldown for a specific ability
    public void StartCooldown(string abilityName, float cooldownTime)
    {
        if (cooldowns.ContainsKey(abilityName))
        {
            cooldowns[abilityName] = Time.time + cooldownTime;
        }
        else
        {
            cooldowns.Add(abilityName, Time.time + cooldownTime);
        }
    }

    // Checks if a specific ability is currently on cooldown
    public bool IsOnCooldown(string abilityName)
    {
        if (cooldowns.ContainsKey(abilityName))
        {
            return Time.time < cooldowns[abilityName];
        }
        return false;
    }

    // Gets the remaining cooldown time for a specific ability
    public float GetCooldownRemaining(string abilityName)
    {
        if (IsOnCooldown(abilityName))
        {
            return cooldowns[abilityName] - Time.time;
        }
        return 0f;
    }
}
