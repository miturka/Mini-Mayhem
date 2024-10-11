using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    // Direction vectors for isometric movement
    private Vector3 moveUpRight = new Vector3(1, 0, 1).normalized;
    private Vector3 moveUpLeft = new Vector3(-1, 0, 1).normalized;
    private Vector3 moveDownRight = new Vector3(1, 0, -1).normalized;
    private Vector3 moveDownLeft = new Vector3(-1, 0, -1).normalized;

    void Update()
    {
        Vector3 movement = Vector3.zero;

        // WASD input handling for isometric movement
        if (Input.GetKey(KeyCode.W))
        {
            movement += moveUpRight;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement += moveDownLeft;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movement += moveUpLeft;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement += moveDownRight;
        }

        // Move the player in the calculated direction
        transform.Translate(movement * moveSpeed * Time.deltaTime);
    }
}
