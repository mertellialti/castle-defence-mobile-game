using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

public class CharactersWindow : EditorWindow {
	
	MainGameData data;
	Vector2 scrollPos;
	
	Texture X;
	Texture charactersIcon;
	Texture plus;
	
	List<bool> characterSettings = new List<bool>();
	
	void OnEnable(){
		data = (MainGameData)Resources.Load("Global game data", typeof(MainGameData));
		if(data == null)
			Debug.LogError("data scriptable object missing");
		
		for(int i = 0; i < data.characters.Count; i++){
			characterSettings.Add(new bool());
		}
		
		X = Resources.Load("x") as Texture;
		charactersIcon = Resources.Load("characters icon") as Texture;
		plus = Resources.Load("plus") as Texture;
	}

    void OnGUI(){
		GUILayout.Label(new GUIContent("  Characters:", charactersIcon), EditorStyles.largeLabel, GUILayout.Height(30));
		
		GUI.color = new Color(0.7f, 0.7f, 0.7f, 0.5f);
		GUILayout.BeginVertical("Box");
		GUI.color = Color.white;
		
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(this.position.height - 90));
		for(int i = 0; i < data.characters.Count; i++){
			newSettingsFoldout(i);
		}
		EditorGUILayout.EndScrollView();
		GUILayout.EndVertical();
		
		GUILayout.BeginHorizontal();
		GUI.color = new Color(0.7f, 1f, 0.7f, 1);
		if(GUILayout.Button(plus, GUILayout.Height(39), GUILayout.Width(39))){
			characterSettings.Add(new bool());
			data.characters.Add(new Character{character = null, characterRagdoll = null, characterName = "", price = 0, description = "DESCRIPTION"});
			characterSettings[characterSettings.Count - 1] = true;
		}
		
		GUI.color = Color.white;
		EditorGUILayout.HelpBox("This is a global list of characters, so the selected character appears in every scene. Players can unlock your characters with the prices specified.", MessageType.Info);
		GUILayout.EndHorizontal();
		
		EditorUtility.SetDirty(data);
    }
	
	void newSettingsFoldout(int i){
		GUILayout.BeginHorizontal();
		GUI.color = new Color(1f, 1f, 1f, 0.6f);
		GUILayout.BeginVertical("Box");
		GUI.color = Color.white;
			
		string title = null;
		if(data.characters[i].characterName != ""){
			title = i + ": " + data.characters[i].characterName;
		}
		else{
			title = i + ": (new character)";
		}
			
		if(GUILayout.Button(title, EditorStyles.largeLabel)){
			GUI.FocusControl(null);
			characterSettings[i] = !characterSettings[i];
		}
		
		if(characterSettings[i]){
			newCharacterSettings(i);
		}
		GUILayout.EndVertical();
		
		GUILayout.Space(-5);
		
		GUI.color = new Color(1f, 0.2f, 0.2f, 0.6f);
		GUILayout.BeginVertical("Box");
		GUI.color = Color.white;
		
		if(GUILayout.Button(X, EditorStyles.largeLabel, GUILayout.Width(19), GUILayout.Height(19)) && EditorUtility.DisplayDialog("Remove special arrow", "Are you sure you want to remove arrow " + i + "?", "Yes", "No")){
			data.characters.RemoveAt(i);
			characterSettings.RemoveAt(i);
		}
		
		GUILayout.EndVertical();
		
		GUILayout.EndHorizontal();
	}
	
	void newCharacterSettings(int i){
		
		GUILayout.Space(5);
		
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Character", GUILayout.Width(170));
        data.characters[i].character = EditorGUILayout.ObjectField(data.characters[i].character, typeof(GameObject), false) as GameObject;
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Character Ragdoll", GUILayout.Width(170));
        data.characters[i].characterRagdoll = EditorGUILayout.ObjectField(data.characters[i].characterRagdoll, typeof(GameObject), false) as GameObject;
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Character Name", GUILayout.Width(170));
        data.characters[i].characterName = EditorGUILayout.TextField(data.characters[i].characterName);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Price", GUILayout.Width(170));
        data.characters[i].price = EditorGUILayout.IntField(data.characters[i].price);
		GUILayout.EndHorizontal();
		
		GUI.color = new Color(0.9f, 0.95f, 0.97f, 1);
		GUILayout.Space(5);
        data.characters[i].description = EditorGUILayout.TextArea(data.characters[i].description);
		GUI.color = Color.white;
	}
}
