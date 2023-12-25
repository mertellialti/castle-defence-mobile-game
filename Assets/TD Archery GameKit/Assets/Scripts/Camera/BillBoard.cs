using UnityEngine;
using System.Collections;

public class BillBoard : MonoBehaviour {

	void Update () {
		//look at the main camera
		transform.LookAt(Camera.main.transform);
	}
}
