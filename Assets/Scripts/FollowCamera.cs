using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform player1; // Hráč 1
    public Transform player2; // Hráč 2

    public float minSize = 4f; // Minimálny ortografický zoom
    public float maxSize = 20f; // Maximálny ortografický zoom
    public float minDistance = 1.04f; // Minimálna vzdialenosť medzi hráčmi
    public float maxDistance = 24f; // Maximálna vzdialenosť medzi hráčmi pre maxSize
    public float sizePadding = 0f; // Padding okolo hráčov
    public float zoomSpeed = 5f; // Rýchlosť priblíženia
    public float followSpeed = 5f; // Rýchlosť sledovania hráčov

    public Vector3 cameraOffset = new Vector3(-10, 12, -10); // Ofset kamery (uprav podľa potreby)

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (player1 == null || player2 == null)
            return;

        // Vypočítať stred medzi hráčmi
        Vector3 midpoint = (player1.position + player2.position) / 2f;

        // Použiť ofset na posun kamery podľa izometrického pohľadu
        Vector3 targetPosition = midpoint + cameraOffset;

        // Plynulo presunúť kameru na cieľovú pozíciu
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Vypočítať vzdialenosť medzi hráčmi
        float distance = Vector3.Distance(player1.position, player2.position);

        //Debug.Log($"Distance between players: {distance}");

        // Normalizovať vzdialenosť od minDistance po maxDistance
        float normalizedDistance = Mathf.InverseLerp(minDistance, maxDistance, distance);

        // Dynamicky upraviť ortografický zoom
        float targetSize = Mathf.Lerp(minSize, maxSize, normalizedDistance) + sizePadding;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);
    }
}
