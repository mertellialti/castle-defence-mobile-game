using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

public class SettingsWindow : EditorWindow {
	
	MainGameData data;
	
	AnimBool general;
	AnimBool shooting;
	AnimBool killCam;
	AnimBool buttonColors;
	
	Texture charactersIcon;
	Texture arrowsIcon;
	
	Rect buttonRect;
	
	public CharactersWindow charactersWindowInstance;
	public SpecialArrowsWindow specialArrowsWindowInstance;
	
	void OnEnable(){
		general = new AnimBool(true);
		general.valueChanged.AddListener(Repaint);
		
		shooting = new AnimBool(false);
		shooting.valueChanged.AddListener(Repaint);
		
		killCam = new AnimBool(false);
		killCam.valueChanged.AddListener(Repaint);
		
		buttonColors = new AnimBool(false);
		buttonColors.valueChanged.AddListener(Repaint);
		
		data = (MainGameData)Resources.Load("Global game data", typeof(MainGameData));
		if(data == null)
			Debug.LogError("data scriptable object missing");
		
		charactersIcon = Resources.Load("characters icon") as Texture;
		arrowsIcon = Resources.Load("arrows icon") as Texture;
	}

	[MenuItem("Window/TD Archery GameKit %#t")]
    static void Init(){
        SettingsWindow window = (SettingsWindow)EditorWindow.GetWindow(typeof(SettingsWindow), false, "TDA GameKit");
        window.Show();
		window.minSize = new Vector2(350, 270);
    }

    void OnGUI(){
		GUI.color = new Color(1, 1, 1, 0.7f);
		GUILayout.BeginVertical("Box");
		GUI.color = Color.white;
		
		DisplayFoldout(general, 0, "General");
		DisplayFoldout(shooting, 1, "Shooting");
		DisplayFoldout(killCam, 2, "Kill Cam");
		DisplayFoldout(buttonColors, 3, "Special arrow button colors");
		
		GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2));
		GUILayout.Space(-3);
		
		GUILayout.BeginHorizontal();
		
		if(charactersWindowInstance == null){
			GUI.color = Color.white;
		}
		else{
			GUI.color = new Color(0.5f, 0.75f, 0.9f, 1);
		}
		
		if(GUILayout.Button(new GUIContent("  Edit Characters", charactersIcon), GUILayout.Height(25), GUILayout.Width(this.position.width/2 - 10))){
			if(charactersWindowInstance == null){
				CharactersWindow charactersWindow = (CharactersWindow)EditorWindow.GetWindow(typeof(CharactersWindow), true, "TD Archery GameKit - Characters");
				charactersWindow.minSize = new Vector2(400, 400);
				charactersWindow.Show();
				charactersWindowInstance = charactersWindow;
			}
			else{
				charactersWindowInstance.Close();
			}
		}
		
		if(specialArrowsWindowInstance == null){
			GUI.color = Color.white;
		}
		else{
			GUI.color = new Color(0.5f, 0.75f, 0.9f, 1);
		}
	
		if(GUILayout.Button(new GUIContent("  Edit Special Arrows", arrowsIcon), GUILayout.Height(25), GUILayout.Width(this.position.width/2 - 10))){
			if(specialArrowsWindowInstance == null){
				SpecialArrowsWindow specialArrowsWindow = (SpecialArrowsWindow)EditorWindow.GetWindow(typeof(SpecialArrowsWindow), true, "TD Archery GameKit - Special Arrows");
				specialArrowsWindow.minSize = new Vector2(400, 400);
				specialArrowsWindow.Show();
				specialArrowsWindowInstance = specialArrowsWindow;
			}
			else{
				specialArrowsWindowInstance.Close();
			}
		}
		GUILayout.EndHorizontal(); 
		GUI.color = Color.white;
		
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
		
		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();
		
		if(GUILayout.Button("Set pose", EditorStyles.toolbarButton, GUILayout.Width(170))){
            PopupWindow.Show(buttonRect, new PosePopup());
        }
        if(Event.current.type == EventType.Repaint) 
			buttonRect = GUILayoutUtility.GetLastRect();
		
		if(GUILayout.Button("Delete playerprefs", EditorStyles.toolbarButton) && EditorUtility.DisplayDialog("Delete PlayerPrefs", "Are you sure you want to delete PlayerPrefs?", "Yes", "No")){
			PlayerPrefs.DeleteAll();
			PlayerPrefs.SetInt("audio", 1);
			Debug.LogWarning("All PlayerPrefs deleted");
		}
		if(GUILayout.Button("Close", EditorStyles.toolbarButton, GUILayout.Width(70))){
			this.Close();
		}
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		
		EditorUtility.SetDirty(data);
    }
	
	void DisplayFoldout(AnimBool animBool, int content, string title){
		if(animBool.target){
			GUI.color = new Color(0.8f, 0.8f, 0.8f, 0.6f);
		}
		else{
			GUI.color = new Color(1f, 1f, 1f, 0.3f);
		}
		GUILayout.BeginVertical("Box");
		GUI.color = Color.white;
		
		if(animBool.target){
			if(GUILayout.Button(title, EditorStyles.boldLabel)){
				CloseAll();
			}
		}
		else{
			if(GUILayout.Button(title, EditorStyles.label)){
				CloseAll();
				animBool.target = true;
			}
		}
		
		if(EditorGUILayout.BeginFadeGroup(animBool.faded)){
			GUILayout.Space(5);
			switch(content){
				case 0: General(); break;
				case 1: Shooting(); break;
				case 2: KillCam(); break;
				case 3: ButtonColors(); break;
			}
		}
		EditorGUILayout.EndFadeGroup();
		GUILayout.EndVertical();
	}
	
	void CloseAll(){
		general.target = false;
		shooting.target = false;
		killCam.target = false;
		buttonColors.target = false;
	}
	
	void General(){
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Default arrow", GUILayout.Width(120));
        data.defaultArrow = EditorGUILayout.ObjectField(data.defaultArrow, typeof(GameObject), false) as GameObject;
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Ice color", GUILayout.Width(120));
        data.iceColor = EditorGUILayout.ColorField(data.iceColor);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Cam rotation speed", GUILayout.Width(120));
        data.camRotationSpeed = EditorGUILayout.FloatField(data.camRotationSpeed);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Fade delay", GUILayout.Width(120));
        data.fadeDelay = EditorGUILayout.FloatField(data.fadeDelay);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Fadespeed", GUILayout.Width(120));
        data.fadeSpeed = EditorGUILayout.FloatField(data.fadeSpeed);
		GUILayout.EndHorizontal();
		
		GUILayout.Space(5);
	}
	
	void Shooting(){
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Sensitivity", GUILayout.Width(120));
        data.sensitivity = EditorGUILayout.FloatField(data.sensitivity);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Max aim rotation", GUILayout.Width(120));
        data.maxAimRotation = EditorGUILayout.IntField(data.maxAimRotation);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Max line steps", GUILayout.Width(120));
        data.maxPredictionLineSteps = EditorGUILayout.IntField(data.maxPredictionLineSteps);
		GUILayout.EndHorizontal();
		
		GUILayout.Space(5);
	}
	
	void KillCam(){
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Kill cam timescale", GUILayout.Width(120));
        data.killCamTimescale = EditorGUILayout.FloatField(data.killCamTimescale);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Kill cam duration", GUILayout.Width(120));
        data.killCamDuration = EditorGUILayout.FloatField(data.killCamDuration);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Kill cam chance", GUILayout.Width(120));
        data.killCamChance = EditorGUILayout.IntField(data.killCamChance);
		GUILayout.EndHorizontal();
		
		GUILayout.Space(5);
	}
	
	void ButtonColors(){
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Button loading color", GUILayout.Width(170));
        data.buttonLoadingColor = EditorGUILayout.ColorField(data.buttonLoadingColor);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Button ready color", GUILayout.Width(170));
        data.buttonReadyColor = EditorGUILayout.ColorField(data.buttonReadyColor);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Button outline color", GUILayout.Width(170));
        data.buttonOutlineColor = EditorGUILayout.ColorField(data.buttonOutlineColor);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Selected button outline color", GUILayout.Width(170));
        data.selectedButtonOutlineColor = EditorGUILayout.ColorField(data.selectedButtonOutlineColor);
		GUILayout.EndHorizontal();
		
		GUILayout.Space(5);
	}
}

public class PosePopup : PopupWindowContent{

    public override Vector2 GetWindowSize(){
        return new Vector2(170, 87);
    }

    public override void OnGUI(Rect rect){
		GUI.color = new Color(0.6f, 0.65f, 0.7f, 0.8f);
		
		GUILayout.BeginVertical("Box");
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("Pose options", EditorStyles.largeLabel);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUI.color = new Color(0.85f, 0.85f, 0.85f, 0.9f);
		GUILayout.BeginHorizontal("Box");
		
		GUI.color = Color.white;
        if(GUILayout.Button("Paste pose", EditorStyles.label))
			createPose(false);
	
		GUILayout.EndHorizontal();
		
		GUI.color = new Color(0.85f, 0.85f, 0.85f, 0.6f);
		GUILayout.BeginHorizontal("Box");
		
		GUI.color = Color.white;
		if(GUILayout.Button("Change saved pose", EditorStyles.label))
			createPose(true);
		
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
    }
	
	void createPose(bool saveNewPose){
		PoseWindow poseWindow = (PoseWindow)EditorWindow.GetWindow(typeof(PoseWindow), true, "TD Archery GameKit - Pose");
		
		poseWindow.minSize = new Vector2(310, 472);
		poseWindow.maxSize = new Vector2(310, 472);
			
		poseWindow.saveNewPose = saveNewPose;
		poseWindow.Show();
	}
}
