using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClamp : MonoBehaviour
{
    [SerializeField] float minX, maxX, minY, maxY = 0.0f;

    private CameraFollow2 cameraFollow2;

    private void Start()
    {
        cameraFollow2 = Camera.main.GetComponent<CameraFollow2>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(gameObject.name + " - " + other.gameObject.name + " collided!");

            if (cameraFollow2 == null) return;

            if (minX != 0) cameraFollow2.minX = minX;
            if (maxX != 0) cameraFollow2.maxX = maxX;
            if (minY != 0) cameraFollow2.minY = minY;
            if (maxY != 0) cameraFollow2.maxY = maxY;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (cameraFollow2 == null) return;

            if (minX != 0) cameraFollow2.minX = Mathf.NegativeInfinity;
            if (maxX != 0) cameraFollow2.maxX = Mathf.Infinity;
            if (minY != 0) cameraFollow2.minY = Mathf.NegativeInfinity;
            if (maxY != 0) cameraFollow2.maxY = Mathf.Infinity;
        }
    }
}
