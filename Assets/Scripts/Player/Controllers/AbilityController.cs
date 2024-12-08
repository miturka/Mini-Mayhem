using UnityEngine;

public class AbilityController : MonoBehaviour
{
    private Player character;

    public KeyCode primaryAbilityKey = KeyCode.Q;
    public KeyCode secondaryAbilityKey = KeyCode.E;

    void Start()
    {
        character = GetComponent<Player>();
    }

    void Update()
    {
        if (Input.GetKeyDown(primaryAbilityKey))
        {
            character.UsePrimaryAbility();
        }

        if (Input.GetKeyDown(secondaryAbilityKey))
        {
            character.UseSecondaryAbility();
        }
    }
}
