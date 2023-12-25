using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

public class PoseWindow : EditorWindow {
	
	[HideInInspector]
	public bool saveNewPose;
	
	Transform[] bones = new Transform[19];
	Color green = new Color(0, 1, 0.3f, 0.6f);
	
	MainGameData data;
	
	void OnEnable(){
		data = (MainGameData)Resources.Load("Global game data", typeof(MainGameData));
	}

    void OnGUI(){
		
		if(!saveNewPose){
			EditorGUILayout.HelpBox("To paste the currently saved pose, add all bones and click 'ok'. Click 'T pose' to set a  T pose.", MessageType.Info);
			GUI.color = new Color(0.8f, 0.85f, 0.9f, 0.4f);
		}
		else{
			EditorGUILayout.HelpBox("By pressing 'ok' you'll save a new pose. Make sure your character is currently in the pose you want to save.", MessageType.Warning);
			GUI.color = new Color(0.9f, 0.9f, 0.8f, 0.6f);
		}
		
		GUILayout.BeginVertical("Box");
		
		GUILayout.Space(3);
		
		checkColor(0);
		bones[0] = EditorGUILayout.ObjectField("Hips", bones[0], typeof(Transform), true) as Transform;
		checkColor(1);
		bones[1] = EditorGUILayout.ObjectField("Spine", bones[1], typeof(Transform), true) as Transform;
		checkColor(2);
		bones[2] = EditorGUILayout.ObjectField("Chest", bones[2], typeof(Transform), true) as Transform;
		checkColor(3);
		bones[3] = EditorGUILayout.ObjectField("Neck", bones[3], typeof(Transform), true) as Transform;
		checkColor(4);
		bones[4] = EditorGUILayout.ObjectField("Head", bones[4], typeof(Transform), true) as Transform;
		
		line();
		
		checkColor(5);
		bones[5] = EditorGUILayout.ObjectField("Left Hips", bones[5], typeof(Transform), true) as Transform;
		checkColor(6);
		bones[6] = EditorGUILayout.ObjectField("Left Knee", bones[6], typeof(Transform), true) as Transform;
		checkColor(7);
		bones[7] = EditorGUILayout.ObjectField("Left Foot", bones[7], typeof(Transform), true) as Transform;
		
		line();
		
		checkColor(8);
		bones[8] = EditorGUILayout.ObjectField("Right Hips", bones[8], typeof(Transform), true) as Transform;
		checkColor(9);
		bones[9] = EditorGUILayout.ObjectField("Right Knee", bones[9], typeof(Transform), true) as Transform;
		checkColor(10);
		bones[10] = EditorGUILayout.ObjectField("Right Foot", bones[10], typeof(Transform), true) as Transform;
		
		line();
		
		checkColor(11);
		bones[11] = EditorGUILayout.ObjectField("Left Shoulder", bones[11], typeof(Transform), true) as Transform;
		checkColor(12);
		bones[12] = EditorGUILayout.ObjectField("Left Arm", bones[12], typeof(Transform), true) as Transform;
		checkColor(13);
		bones[13] = EditorGUILayout.ObjectField("Left Elbow", bones[13], typeof(Transform), true) as Transform;
		checkColor(14);
		bones[14] = EditorGUILayout.ObjectField("Left Hand", bones[14], typeof(Transform), true) as Transform;
		
		line();
		
		checkColor(15);
		bones[15] = EditorGUILayout.ObjectField("Right Shoulder", bones[15], typeof(Transform), true) as Transform;
		checkColor(16);
		bones[16] = EditorGUILayout.ObjectField("Right Arm", bones[16], typeof(Transform), true) as Transform;
		checkColor(17);
		bones[17] = EditorGUILayout.ObjectField("Right Elbow", bones[17], typeof(Transform), true) as Transform;
		checkColor(18);
		bones[18] = EditorGUILayout.ObjectField("Right Hand", bones[18], typeof(Transform), true) as Transform;
		
		GUILayout.Space(3);
		
		GUILayout.EndVertical();
		
		GUI.color = Color.white;
		
		GUILayout.BeginHorizontal();
		
		EditorGUI.BeginDisabledGroup(buttonActive() == false);
		
		if(!saveNewPose){
			if(GUILayout.Button("T pose"))
				setTPose();
		}
		
		if(GUILayout.Button("OK"))
			checkAction();
		
		EditorGUI.EndDisabledGroup();
		GUILayout.EndHorizontal();
		
		EditorUtility.SetDirty(data);
    }
	
	void checkColor(int i){
		if(bones[i] == null)
			GUI.color = Color.white;
		else
			GUI.color = green;
	}
	
	void line(){
		GUILayout.Space(2);
		GUI.color = new Color(0.7f, 0.7f, 0.7f, 0.5f);
		GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2));
		GUI.color = Color.white;
		GUILayout.Space(2);
	}
	
	void checkAction(){
		if(saveNewPose){
			savePose();
		}
		else{
			setPose();
		}
	}
	
	void setPose(){
		for(int i = 0; i < 19; i++){
			bones[i].localEulerAngles = data.rotations[i];
		}
		
		this.Close();
	}
	
	void savePose(){
		for(int i = 0; i < 19; i++){
			data.rotations[i] = bones[i].localEulerAngles;
		}
		
		Debug.LogWarning("New pose saved");
		
		this.Close();
	}
	
	void setTPose(){
		for(int i = 0; i < 19; i++){
			bones[i].localEulerAngles = data.TPose[i];
		}
		
		this.Close();
	}
	
	bool buttonActive(){
		bool possible = true;
		
		for(int i = 0; i < 19; i++){
			if(bones[i] == null)
				possible = false;
		}
		
		return possible;
	}
}
