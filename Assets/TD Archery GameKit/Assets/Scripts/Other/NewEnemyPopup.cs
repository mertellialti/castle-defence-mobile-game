using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewEnemyPopup : MonoBehaviour {
	
	//not visible in the inspector
	[HideInInspector]
	public Sprite bigImage;
	[HideInInspector]
	public string enemyName;
	[HideInInspector]
	public string description;
	[HideInInspector]
	public Enemy enemy;
	
	[HideInInspector]
	public Animator popupAnimator;

	public void openPopup(){
		//start the popup
		StartCoroutine(startPopup());
	}
	
	IEnumerator startPopup(){
		//show the popup
		popupAnimator.SetBool("visible", true);
		
		//assign the images and text
		GameObject.Find("big image").GetComponent<Image>().sprite = bigImage;
		GameObject.Find("name").GetComponent<Text>().text = enemyName;
		GameObject.Find("description").GetComponent<Text>().text = description;
		
		//also show the enemy stats
		string stats = "- Lives: " + enemy.startLives + "\n\n- Run Speed: " + enemy.runSpeed + "\n\n- Walk Speed: " + enemy.walkSpeed + "\n\n- Coins: " + enemy.coins;
		GameObject.Find("stats").GetComponent<Text>().text = stats;
		
		//wait a moment for the animation
		yield return new WaitForSeconds(1);
		
		//set timescale and destroy the button
		Time.timeScale = 0;
		Destroy(gameObject);
	}
}
