using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform target; 
    public Vector3 targetPos = new Vector3(-12,12,15); // The target position to move the camera towards.
    public float lerpTime = 1f; // The amount of time it should take for the camera to reach the target position.
    private Vector3 cameraStartPos;
    private float cameraSize;
    [SerializeField] GameObject scout;
    

    private void Start()
    {
        
        cameraSize = Camera.main.orthographicSize;
        cameraStartPos = new Vector3(12,12,-15);
    }

    private IEnumerator LerpToTarget(Vector3 from, Vector3 to)
    {
        float elapsedTime = 0f;
        var targetPosition = new Vector3(to.x, cameraStartPos.y, cameraStartPos.z);
        while (elapsedTime < lerpTime)
        {
            transform.position = Vector3.Lerp(from, targetPosition, (elapsedTime / lerpTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
        yield return new WaitForSeconds(2f);
        yield return LerpBack(transform.position, cameraStartPos);

    }
    
    private IEnumerator LerpBack(Vector3 from, Vector3 to)
    {
        var elapsedTime = 0f;
        var targetPosition = new Vector3(cameraStartPos.x, cameraStartPos.y, cameraStartPos.z);
        while (elapsedTime < lerpTime)
        {
            transform.position = Vector3.Lerp(from, targetPosition, (elapsedTime / lerpTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = to;
        yield return new WaitForSeconds(2f);

    }

    public void StartLerp()
    {
        // cameraStartPos = transform.position;
        StartCoroutine(LerpToTarget(cameraStartPos,targetPos));
    }
}
