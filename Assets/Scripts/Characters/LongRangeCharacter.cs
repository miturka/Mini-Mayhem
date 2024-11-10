using UnityEngine;

public class LongRangeCharacter : Character
{
    protected override void InitializeCharacter()
    {
        if (movement != null)
        {
            movement.moveSpeed = 3.1f;
            movement.jumpHeight = 1f;
        }

        // Configure health
        if (health != null)
        {
            health.maxHealth = 80;
            health.InitializeHealth();
        }

        primaryAbility = GetComponent<SpinAttack>(); 
        //secondaryAbility = GetComponent<Shield>();   

        Debug.Log("LongRangeCharacter initialized with SpinAttack ability.");
    }
}
