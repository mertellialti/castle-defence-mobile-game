using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class specialArrow{
	public GameObject arrow;
	public Sprite image;
	public int requiredCharacter;
	public float delay;
	public Color lineColor;
	
	[HideInInspector]
	public GameObject button;
}

[System.Serializable]
public class Character{
	public GameObject character;
	public GameObject characterRagdoll;
	public string characterName;
	public int price;
	
	[TextArea(2, 5)]
	public string description;
}

public class MainGameData : ScriptableObject {
	
	[HideInInspector]
	public GameObject defaultArrow;
	[HideInInspector]
	public Color iceColor;
	[HideInInspector]
	public float camRotationSpeed;
	
	[HideInInspector]
	public float sensitivity;
	[HideInInspector]
	public int maxAimRotation;
	[HideInInspector]
	public int maxPredictionLineSteps;
	
	[HideInInspector]
	public float fadeDelay;
	[HideInInspector]
	public float fadeSpeed;
	
	[HideInInspector]
	public float killCamTimescale;
	[HideInInspector]
	public float killCamDuration;
	[HideInInspector]
	public int killCamChance;
	
	[HideInInspector]
	public List<Character> characters;
	
	[HideInInspector]
	public Color buttonLoadingColor;
	[HideInInspector]
	public Color buttonReadyColor;
	[HideInInspector]
	public Color buttonOutlineColor;
	[HideInInspector]
	public Color selectedButtonOutlineColor;
	
	[HideInInspector]
	public List<specialArrow> specialArrows;
	
	[HideInInspector]
	public Vector3[] TPose = new Vector3[19];
	
	[HideInInspector]
	public Vector3[] rotations = new Vector3[19];
}
