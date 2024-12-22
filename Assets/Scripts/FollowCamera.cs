using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform player1; // Hráč 1
    public Transform player2; // Hráč 2

    public float minSize = 4f; // minimal camera zoom
    public float maxSize = 20f; // max camera zoom
    public float minDistance = 1.04f; // Minimal player distance
    public float maxDistance = 24f; // max player distance
    public float sizePadding = 0f; // Padding 
    public float zoomSpeed = 5f; 
    public float followSpeed = 5f; 

    public Vector3 cameraOffset = new Vector3(-10, 12, -10); 

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (player1 == null || player2 == null)
            return;

        // midpoint between players
        Vector3 midpoint = (player1.position + player2.position) / 2f;
        Vector3 targetPosition = midpoint + cameraOffset;

        // move camera to target
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // count distance between players
        float distance = Vector3.Distance(player1.position, player2.position);

        // normalize distance between minDistance - maxDistance
        float normalizedDistance = Mathf.InverseLerp(minDistance, maxDistance, distance);

        // update zoom of the camera
        float targetSize = Mathf.Lerp(minSize, maxSize, normalizedDistance) + sizePadding;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);
    }
}
