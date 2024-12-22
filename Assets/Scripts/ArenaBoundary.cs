using UnityEngine;

// give knockback to player trying to jump out of arena
public class ArenaBoundary : MonoBehaviour
{
    public float knockbackForce = 10f; // Force of the knockback

    public float speedMultiplier = 1.5f;

    public Vector3 knockbackDirectionOffset = new Vector3(0, 1, 0); 

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            playerMovement.ReceiveArenaKnockback( knockbackDirectionOffset, knockbackForce, speedMultiplier);
            Debug.Log("Leavol player;");
        }
        
    }
}
