using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Targets")]
    public Transform player1;
    public Transform player2;

    [Header("Settings")]
    public float smoothSpeed = 5f;
    public float minZoom = 5f;
    public float maxZoom = 10f;
    public float zoomPadding = 2f;
    public float minX = -8f;
    public float maxX = 8f;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (player1 == null || player2 == null) return;

        MoveCamera();
        ZoomCamera();
    }

    private void MoveCamera()
    {
        Vector3 midpoint = (player1.position + player2.position) / 2f;
        midpoint.x = Mathf.Clamp(midpoint.x, minX, maxX);
        midpoint.y = 1f;
        midpoint.z = -10f;

        transform.position = Vector3.Lerp(transform.position, midpoint, smoothSpeed * Time.deltaTime);
    }

    private void ZoomCamera()
    {
        float distance = Vector3.Distance(player1.position, player2.position);
        float zoom = Mathf.Clamp(distance / 2f + zoomPadding, minZoom, maxZoom);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoom, smoothSpeed * Time.deltaTime);
    }
}