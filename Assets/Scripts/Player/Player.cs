using UnityEngine;
using UnityEngine.UI;
using System;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    protected PlayerMovement movement;
    protected HealthManager health;
    protected AbilityController cooldown;


    [Header("Abilities")]
    [SerializeField] public BaseAbility primaryAbility;   // Selectable in Inspector
    [SerializeField] public BaseAbility secondaryAbility;

    public void UsePrimaryAbility() => primaryAbility?.Activate();
    public void UseSecondaryAbility() => secondaryAbility?.Activate();
    public void TakeDamage(int amount) => health?.TakeDamage(amount);
    public bool IsAlive() => (bool)(health?.IsAlive());
    public void setPrimaryAbility(BaseAbility ability) => primaryAbility = ability;
    public void setSecondaryAbility(BaseAbility ability) => secondaryAbility = ability;


    public void Initialize(UnityEngine.UI.Image healthBar, UnityEngine.UI.Image primaryCooldownBar, UnityEngine.UI.Image secondaryCooldownBar, int maxHealth=100, float moveSpeed=5.0f, float jumpHeight=2.0f){
        movement = GetComponent<PlayerMovement>();
        health = GetComponent<HealthManager>();
        cooldown = GetComponent<AbilityController>();


        if (movement != null)
        {
            movement.moveSpeed = moveSpeed;
            movement.jumpHeight = jumpHeight;
        }

        if (health != null)
        {
            health.maxHealth = maxHealth;
            health.healthBar = healthBar;
            health.InitializeHealth();

        }
        else{
            Debug.Log("health neni");
        }

        if(cooldown != null)
        {
            cooldown.primaryCooldownBar = primaryCooldownBar;
            cooldown.secondaryCooldownBar = secondaryCooldownBar;
        }
        else{
            Debug.Log("cooldown neni");
        }



        Debug.Log("Character initialized.");
    }

    public void AddAbility(string abilityName, bool isPrimary)
    {
        Type abilityType = Type.GetType(abilityName); // Nájde typ podľa názvu

        if (abilityType == null)
        {
            Debug.LogError($"Ability type '{abilityName}' not found.");
            return;
        }

        if (!typeof(BaseAbility).IsAssignableFrom(abilityType)) // Skontroluje, či trieda dedí z BaseAbility
        {
            Debug.LogError($"{abilityName} is not a valid ability.");
            return;
        }

        // Dynamické pridanie komponentu
        BaseAbility ability = (BaseAbility)gameObject.AddComponent(abilityType);

        if (isPrimary)
        {
            primaryAbility = ability;
        }
        else
        {
            secondaryAbility = ability;
        }
    }
}
