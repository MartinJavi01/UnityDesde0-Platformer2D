using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Vector2 minPos;
    [SerializeField]
    private Vector2 maxPos;
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float heightDiff;

    public void FixedUpdate() {
        Vector3 currentPos = transform.position;
        currentPos.x = Mathf.Lerp(currentPos.x, target.position.x, GlobalVariables.CAMERA_SMOTHNESS);
        currentPos.y = Mathf.Lerp(currentPos.y, target.position.y + heightDiff, GlobalVariables.CAMERA_SMOTHNESS);

        if(currentPos.x < minPos.x) currentPos.x = minPos.x;
        if(currentPos.x > maxPos.x) currentPos.x = maxPos.x;
        if(currentPos.y < minPos.y) currentPos.y = minPos.y;
        if(currentPos.y > maxPos.y) currentPos.y = maxPos.y;
        transform.position = currentPos;
    }
}
