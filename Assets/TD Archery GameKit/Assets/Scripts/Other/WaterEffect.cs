using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterEffect : MonoBehaviour {
	
	//visible in the inspector
	public Material water;

	void Update () {
		//scroll the water texture for a wave effect
		water.mainTextureOffset = new Vector2(Time.time * -0.08f, Time.time * 0.05f);
	}
}
