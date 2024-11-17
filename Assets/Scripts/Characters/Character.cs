using UnityEngine;

public abstract class Character : MonoBehaviour
{
    protected PlayerMovement movement;
    protected HealthManager health;
    public IAbility primaryAbility;
    public IAbility secondaryAbility;

    protected virtual void Start()
    {
        movement = GetComponent<PlayerMovement>();
        health = GetComponent<HealthManager>();

        InitializeCharacter(); 
    }

    protected abstract void InitializeCharacter();
    public void UsePrimaryAbility() => primaryAbility?.Activate();
    public void UseSecondaryAbility() => secondaryAbility?.Activate();
    public void TakeDamage(int amount) => health?.TakeDamage(amount);
}
