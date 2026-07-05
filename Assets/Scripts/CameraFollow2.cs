using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow2 : MonoBehaviour
{
    public Transform target;
    public float lerpSpeed = 5.0f;

    public float offsetX = 0.0f;
    public float offsetY = 1.0f;

    [Header("Manual Limits (If Map Collider is not used)")]
    public float minX = Mathf.NegativeInfinity;
    public float maxX = Mathf.Infinity;
    public float minY = Mathf.NegativeInfinity;
    public float maxY = Mathf.Infinity;

    [Header("Automatic Limits (Optional)")]
    public BoxCollider2D mapBoundaryCollider;

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();

        if (target == null && PlayerController.Instance != null)
        {
            target = PlayerController.Instance.transform;
        }
        else if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }

        if (target == null) return;

        // Camera always start from player's position
        MoveCamera(false);
    }

    private void Update()
    {
        if (target == null)
        {
            if (PlayerController.Instance != null)
            {
                target = PlayerController.Instance.transform;
                MoveCamera(false); // Snap immediately when found
            }
            else
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    target = player.transform;
                    MoveCamera(false);
                }
            }
        }
    }

    private void LateUpdate()
    {
        MoveCamera(true);
    }

    // When isLerping is true, the camera position will interpolate to player's position
    private void MoveCamera(bool isLerping)
    {
        if (target == null) return;

        Vector3 offset = new Vector3(offsetX, offsetY, 0.0f);
        Vector3 targetPos = target.position + offset;
        
        Vector3 newPos = isLerping ? Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime) : targetPos;

        float currentMinX = minX;
        float currentMaxX = maxX;
        float currentMinY = minY;
        float currentMaxY = maxY;

        // If we have a map boundary collider, calculate boundaries automatically based on screen aspect ratio
        if (mapBoundaryCollider != null && cam != null)
        {
            float halfHeight = cam.orthographicSize;
            float halfWidth = halfHeight * cam.aspect;

            Bounds bounds = mapBoundaryCollider.bounds;

            currentMinX = bounds.min.x + halfWidth;
            currentMaxX = bounds.max.x - halfWidth;
            currentMinY = bounds.min.y + halfHeight;
            currentMaxY = bounds.max.y - halfHeight;

            // Prevent boundaries from collapsing if map is smaller than screen
            if (currentMinX > currentMaxX)
            {
                float centerX = bounds.center.x;
                currentMinX = centerX;
                currentMaxX = centerX;
            }
            if (currentMinY > currentMaxY)
            {
                float centerY = bounds.center.y;
                currentMinY = centerY;
                currentMaxY = centerY;
            }
        }

        newPos.x = Mathf.Clamp(newPos.x, currentMinX, currentMaxX);
        newPos.y = Mathf.Clamp(newPos.y, currentMinY, currentMaxY);
        newPos.z = transform.position.z; // Keep camera Z depth

        transform.position = newPos;
    }
}