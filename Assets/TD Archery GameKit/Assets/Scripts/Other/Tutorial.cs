using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {
	
	//variables visible in the inspector
	public Animator animator;
	public GameObject hand;
	public AudioSource audioSource;
	
	void Update(){
		//if the screen is touched or clicked
		if(Input.GetMouseButton(0)){
			//disable the hand object
			if(hand.activeSelf)
				hand.SetActive(false);
		}
	}

	public void hit(){
		//play audio
		audioSource.Play();
		//play animation
		animator.enabled = true;
		
		//end the tutorial by loading the main menu
		StartCoroutine(endTutorial());
	}
	
	IEnumerator endTutorial(){
		//wait for the animation
		yield return new WaitForSeconds(1);
		//load the main menu
		GameObject.FindObjectOfType<Manager>().home();
	}
}
