using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow2 : MonoBehaviour
{
    public Transform target;
    public float lerpSpeed = 1.0f;

    public float offsetX = 0.0f;
    public float offsetY = 1.0f;

    public float minX = Mathf.NegativeInfinity;
    public float maxX = Mathf.Infinity;
    public float minY = Mathf.NegativeInfinity;
    public float maxY = Mathf.Infinity;

    private void Update()
    {
        if (target == null) return;

        Vector3 offset = new Vector3(offsetX, offsetY, transform.position.z);
        Vector3 targetPos = target.position + offset;
        Vector3 newPos = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);

        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

        transform.position = newPos;
    }

}