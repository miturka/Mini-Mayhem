using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform player;           // Reference to the player's transform
    public Vector3 offset = new Vector3(-10, 14, -10); // Set offset to maintain isometric view
    public bool followEnabled = true;  // Toggle to enable/disable follow

    void LateUpdate()
    {
        // Only follow the player if followEnabled is true
        if (followEnabled && player != null)
        {
            // Calculate the target position using only the player's X and Z positions
            Vector3 targetPosition = new Vector3(player.position.x + offset.x, transform.position.y, player.position.z + offset.z);

            // Update the camera's position
            transform.position = targetPosition;
        }
    }

    // Method to toggle the follow camera on/off
    public void ToggleFollow()
    {
        followEnabled = !followEnabled;
    }
}
