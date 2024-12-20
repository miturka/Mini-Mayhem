using System.Collections.Generic;
using UnityEngine;

public class CooldownManager : MonoBehaviour
{
    private Dictionary<string, float> cooldowns = new Dictionary<string, float>();

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

    public bool IsOnCooldown(string abilityName)
    {
        if (cooldowns.ContainsKey(abilityName))
        {
            return Time.time < cooldowns[abilityName];
        }
        return false;
    }

    public float GetCooldownRemaining(string abilityName)
    {
        if (IsOnCooldown(abilityName))
        {
            return cooldowns[abilityName] - Time.time;
        }
        return 0f;
    }
}
