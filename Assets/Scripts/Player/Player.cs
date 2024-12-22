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
    [SerializeField] public BaseAbility primaryAbility;   // Selectable in Inspector
    [SerializeField] public BaseAbility secondaryAbility;
    [SerializeField] public BaseAbility tertiaryAbility;

    public Renderer rendererbody;
    public Renderer rendererlegl;
    public Renderer rendererlegr;
    public Renderer rendererglasses;

    public Material blueCatMaterial;

    public bool isAttacking = false;



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

    public bool isHitBySpinAttack = false;

    
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

    public void AddAbility(string abilityName, int num)
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
