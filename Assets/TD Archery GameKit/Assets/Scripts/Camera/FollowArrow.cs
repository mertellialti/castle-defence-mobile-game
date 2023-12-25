using UnityEngine;
using System.Collections;

public class FollowArrow : MonoBehaviour {

	//Variables visible in the inspector
    public float distance;
    public float height;
    public float heightDamping;
    public float rotationDamping;
	
	//not visible
	[HideInInspector]
	public Transform camTarget;
 
    void LateUpdate(){
		
		//Check if the camera has a target to follow
        if(!camTarget)
            return;
		
		//Some private variables for the rotation and position of the camera
        float wantedRotationAngle = camTarget.eulerAngles.y;
        float wantedHeight = camTarget.position.y + height;
        float currentRotationAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;
		
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
 
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);
 
        Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);
     
        transform.position = camTarget.position;
        transform.position -= currentRotation * Vector3.forward * distance;
		
		//Set camera postition
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
		
		//Look at the arrow
        transform.LookAt(camTarget);
    }
}
