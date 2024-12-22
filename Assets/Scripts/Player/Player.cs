using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;

public class Player : MonoBehaviour
{
    protected PlayerMovement movement;
    protected HealthManager health;
    protected AbilityController cooldown;
    
    [Header("Abilities")]
    [SerializeField] public BaseAbility primaryAbility;   
    [SerializeField] public BaseAbility secondaryAbility;
    [SerializeField] public BaseAbility tertiaryAbility;

    // renderers for changing skin
    public Renderer rendererbody;
    public Renderer rendererlegl;
    public Renderer rendererlegr;
    public Renderer rendererglasses;
    public Material blueCatMaterial;

    public bool isAttacking = false; // bool for blocking other abilities when one is active

    public bool isHitBySpinAttack = false;

    public void UsePrimaryAbility()
    {
        if (!isAttacking)
        {
            primaryAbility?.Activate();
        }
    }

    public void UseSecondaryAbility()
    {
        if (!isAttacking)
        {
            secondaryAbility?.Activate();
        }
    }

    public void UseTertiaryAbility()
    {
        if (!isAttacking)
        {
            tertiaryAbility?.Activate();
        }
    }
    public void TakeDamage(int amount) => health?.TakeDamage(amount);
    public bool IsAlive() => (bool)(health?.IsAlive());
    public void setPrimaryAbility(BaseAbility ability) => primaryAbility = ability;
    public void setSecondaryAbility(BaseAbility ability) => secondaryAbility = ability;
    public void setTertiaryAbility(BaseAbility ability) => secondaryAbility = ability;
 
    public void Initialize(UnityEngine.UI.Image healthBar, TextMeshProUGUI healthNum, UnityEngine.UI.Image primaryCooldownBar, UnityEngine.UI.Image secondaryCooldownBar, UnityEngine.UI.Image tertiaryCooldownBar, int maxHealth=100, float moveSpeed=5.0f, float jumpHeight=2.0f){
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
            health.healthNum = healthNum;
            health.InitializeHealth();

        }
        else{
            Debug.Log("health neni");
        }

        if(cooldown != null)
        {
            cooldown.primaryCooldownBar = primaryCooldownBar;
            cooldown.secondaryCooldownBar = secondaryCooldownBar;
            cooldown.tertiaryCooldownBar = tertiaryCooldownBar;
        }
        else{
            Debug.Log("cooldown neni");
        }

        Debug.Log("Character initialized.");
    }

    // Add ability by string name
    public void AddAbility(string abilityName, int num)
    {
        Type abilityType = Type.GetType(abilityName); // find type by name

        if (abilityType == null)
        {
            Debug.LogError($"Ability type '{abilityName}' not found.");
            return;
        }

        if (!typeof(BaseAbility).IsAssignableFrom(abilityType)) // check if its baseability
        {
            Debug.LogError($"{abilityName} is not a valid ability.");
            return;
        }

        // Add component
        BaseAbility ability = (BaseAbility)gameObject.AddComponent(abilityType);

        if (num == 0)
        {
            primaryAbility = ability;
        }
        else if (num ==1)
        {
            secondaryAbility = ability;
        }
        else{
            tertiaryAbility = ability;
        }
    }

    public void HitBySpinAttack(){
        isHitBySpinAttack = true;
        StartCoroutine(WaitOnly());

    }

    public void changeMaterial(){
        rendererbody.material = blueCatMaterial;
        rendererlegl.material = blueCatMaterial;
        rendererlegr.material = blueCatMaterial;
        rendererglasses.material = blueCatMaterial;
    }

    public int getHealth(){
        return health.getCurrentHealth();
    }

    IEnumerator WaitOnly()
    {
        yield return new WaitForSeconds(0.2f);
        isHitBySpinAttack = false;
    }
}
