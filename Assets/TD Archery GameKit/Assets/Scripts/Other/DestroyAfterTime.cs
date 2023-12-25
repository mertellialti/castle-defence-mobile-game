using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour {
	
	public float lifetime;

	// Use this for initialization
	void Start () {
		//destroy object after lifetime
		Destroy(gameObject, lifetime);
	}
}
