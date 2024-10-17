using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5.0f;
    public Rigidbody rb;

    private Vector3 movementDirection;

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Convert input to isometric directions
        Vector3 direction = new Vector3(horizontal, 0, vertical);
        Vector3 isometricDirection = Quaternion.Euler(0, 45, 0) * direction;

        if (isometricDirection.magnitude > 0)
        {
            // Rotate player to face the movement direction
            transform.rotation = Quaternion.LookRotation(isometricDirection);
        }

        if (Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // Move the player
        transform.Translate(isometricDirection * moveSpeed * Time.deltaTime, Space.World);
    }
}
