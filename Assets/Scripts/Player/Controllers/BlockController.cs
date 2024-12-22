using UnityEngine;

public class BlockController : MonoBehaviour
{
    public bool isBlocking = false;       // Whether the player is currently blocking
    public float blockDuration = 1f;     // Duration for which the block lasts
    private bool canBlock = true;        // Whether the player can block

    private void Update()
    {
        // Detect blocking input (e.g., Right Mouse Button or another key)
        if (Input.GetKeyDown(KeyCode.B) && canBlock)
        {
            StartCoroutine(BlockCoroutine());
        }
    }

    private System.Collections.IEnumerator BlockCoroutine()
    {
        isBlocking = true;
        Debug.Log("Blocking!");
        yield return new WaitForSeconds(blockDuration);
        isBlocking = false;
        Debug.Log("Block ended.");
    }
}
