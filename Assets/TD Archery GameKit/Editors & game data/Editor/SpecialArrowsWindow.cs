using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

public class SpecialArrowsWindow : EditorWindow {
	
	MainGameData data;
	Vector2 scrollPos;
	
	Texture X;
	Texture arrowsIcon;
	Texture plus;
	
	List<bool> arrowSettings = new List<bool>();
	
	void OnEnable(){
		data = (MainGameData)Resources.Load("Global game data", typeof(MainGameData));
		if(data == null)
			Debug.LogError("data scriptable object missing");
		
		for(int i = 0; i < data.specialArrows.Count; i++){
			arrowSettings.Add(new bool());
		}
		
		X = Resources.Load("x") as Texture;
		arrowsIcon = Resources.Load("arrows icon") as Texture;
		plus = Resources.Load("plus") as Texture;
	}

    void OnGUI(){
		GUILayout.Label(new GUIContent("  Special Arrows:", arrowsIcon), EditorStyles.largeLabel, GUILayout.Height(30));
		
		GUI.color = new Color(1f, 1f, 1f, 0.5f);
		GUILayout.BeginVertical("Box");
		GUI.color = Color.white;
		
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(this.position.height - 90));
		for(int i = 0; i < data.specialArrows.Count; i++){
			newSettingsFoldout(i);
		}
		EditorGUILayout.EndScrollView();
		GUILayout.EndVertical();
		
		GUILayout.BeginHorizontal();
		GUI.color = new Color(0.7f, 1f, 0.7f, 1);
		if(GUILayout.Button(plus, GUILayout.Height(39), GUILayout.Width(39))){
			arrowSettings.Add(new bool());
			data.specialArrows.Add(new specialArrow{arrow = null, image = null, requiredCharacter = 0, delay = 0, lineColor = Color.white, button = null});
			arrowSettings[arrowSettings.Count - 1] = true;
		}
		
		GUI.color = Color.white;
		EditorGUILayout.HelpBox("This is a global list of special arrows. The arrows appear scene independent and based on the selected character.", MessageType.Info);
		GUILayout.EndHorizontal();
		
		EditorUtility.SetDirty(data);
    }
	
	void newSettingsFoldout(int i){
		GUILayout.BeginHorizontal();
		
		if(data.specialArrows[i].arrow){
			GUI.color = new Color(0.8f, 0.8f, 0.8f, 0.5f);
		}
		else{
			GUI.color = new Color(1f, 0.7f, 0.7f, 0.5f);
		}
		
		GUILayout.BeginVertical("Box");
		GUI.color = Color.white;
			
		string title = null;
		if(data.specialArrows[i].arrow){
			title = i + ": " + data.specialArrows[i].arrow.name;
		}
		else{
			title = i + ": (new arrow)";
		}
			
		if(GUILayout.Button(title, EditorStyles.largeLabel)){
			arrowSettings[i] = !arrowSettings[i];
			GUI.FocusControl(null);
		}
		
		if(arrowSettings[i]){
			newArrowSettings(i);
		}
		GUILayout.EndVertical();
		
		GUILayout.Space(-5);
		
		GUI.color = new Color(1f, 0.2f, 0.2f, 0.6f);
		GUILayout.BeginVertical("Box");
		GUI.color = Color.white;
		
		if(GUILayout.Button(X, EditorStyles.largeLabel, GUILayout.Width(19), GUILayout.Height(19)) && EditorUtility.DisplayDialog("Remove special arrow", "Are you sure you want to remove arrow " + i + "?", "Yes", "No")){
			data.specialArrows.RemoveAt(i);
			arrowSettings.RemoveAt(i);
		}
		
		GUILayout.EndVertical();
		
		GUILayout.EndHorizontal();
	}
	
	void newArrowSettings(int i){
		
		GUILayout.Space(5);
		
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Arrow", GUILayout.Width(170));
        data.specialArrows[i].arrow = EditorGUILayout.ObjectField(data.specialArrows[i].arrow, typeof(GameObject), false) as GameObject;
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Image", GUILayout.Width(170));
        data.specialArrows[i].image = EditorGUILayout.ObjectField(data.specialArrows[i].image, typeof(Sprite), false) as Sprite;
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Required character (index)", GUILayout.Width(170));
        data.specialArrows[i].requiredCharacter = EditorGUILayout.IntField(data.specialArrows[i].requiredCharacter);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Delay", GUILayout.Width(170));
        data.specialArrows[i].delay = EditorGUILayout.FloatField(data.specialArrows[i].delay);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Line color", GUILayout.Width(170));
        data.specialArrows[i].lineColor = EditorGUILayout.ColorField(data.specialArrows[i].lineColor);
		GUILayout.EndHorizontal();
		
		GUILayout.Space(5);
	}
}
