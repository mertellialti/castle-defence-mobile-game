using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour {
	
	//variables visible in the inspector
	public Transform head;
	public Transform legs;
	public Transform chest;
	public float randomRotation;
	public float lifetime;
	
	void Start(){
		//rotate the ragdoll randomly for some effect
		transform.eulerAngles = new Vector3(transform.eulerAngles.x + Random.Range(-randomRotation, randomRotation), transform.eulerAngles.y + Random.Range(-randomRotation/4, randomRotation/4), transform.eulerAngles.z);
		//destroy the ragdoll after its lifetime
		Destroy(gameObject, lifetime);
	}
}
