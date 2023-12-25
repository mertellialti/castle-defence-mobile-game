using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

[CustomEditor(typeof(Spawner))]
public class SpawnerInspector : Editor{
	
	List<string> spawnPointNames = new List<string>();
	
	int toolbarSelected = 0;
	
	GUIContent spawnpointContent;
	GUIContent settingsContent;
	GUIContent enemyContent;
	
	Texture spawnpointsTexture;
	Texture enemiesTexture;
	Texture settingsTexture;
	
	Texture spawnpointsTexture1;
	Texture enemiesTexture1;
	Texture settingsTexture1;
	
	Texture plus;
	Texture X;
	
	Spawner spawner;
	
	ReorderableList enemyList;
	
	void OnEnable(){
	spawner = (target as Spawner).gameObject.GetComponent<Spawner>();
	
	updateSpawnPointNames();
	
	enemyList = new ReorderableList(serializedObject, serializedObject.FindProperty("enemies"), true, true, false, true);
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	enemyList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
    var element = enemyList.serializedProperty.GetArrayElementAtIndex(index);
	
	int labelWidth = 60;
	
	GUI.color = new Color(1f, 0.9f, 0.5f, 1);
	spawner.enemies[index].spawnPointIndex = EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width - 17, EditorGUIUtility.singleLineHeight), spawner.enemies[index].spawnPointIndex, spawnPointNames.ToArray());
	
	if(spawner.enemies[index].newEnemy){
		GUI.color = new Color(0.5f, 1f, 0.5f, 1f);
	}
	else{
		GUI.color = new Color(0.7f, 0.7f, 0.7f, 0.8f);
	}
	
	spawner.enemies[index].newEnemy = EditorGUI.Toggle(new Rect(rect.x + rect.width - 14, rect.y, 16, EditorGUIUtility.singleLineHeight), spawner.enemies[index].newEnemy);
	
	if(spawner.enemies[index].newEnemy){
		GUI.color = new Color(0.85f, 0.85f, 0.85f, 1f);
	}
	else{
		GUI.color = Color.white;
	}
	
    EditorGUI.PropertyField(
        new Rect(rect.x + labelWidth, rect.y + EditorGUIUtility.singleLineHeight + 5, rect.width - labelWidth, EditorGUIUtility.singleLineHeight),
        element.FindPropertyRelative("enemyPrefab"), GUIContent.none);
	
	if(index != spawner.enemies.Count - 1){
	EditorGUI.PropertyField(
        new Rect(rect.x + labelWidth, rect.y + EditorGUIUtility.singleLineHeight * 2 + 10, rect.width - labelWidth, EditorGUIUtility.singleLineHeight),
        element.FindPropertyRelative("delay"), GUIContent.none);
	}
		
	GUI.color = Color.white;
    EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + 5, labelWidth - 10, EditorGUIUtility.singleLineHeight), "Prefab");
	
	if(index != spawner.enemies.Count - 1)
	EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2 + 10, labelWidth - 10, EditorGUIUtility.singleLineHeight), "Delay");
	
		if(spawner.enemies[index].newEnemy){
			EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 4 + 2, rect.width - 10, EditorGUIUtility.singleLineHeight), "NEW ENEMY POPUP:");
			
			GUI.color = new Color(0.7f, 0.7f, 0.7f, 0.6f);
			
			EditorGUI.DrawRect(new Rect(rect.x - 5, rect.y + EditorGUIUtility.singleLineHeight * 4, rect.width + 5, EditorGUIUtility.singleLineHeight * 9 + 7), new Color(0.7f, 0.7f, 0.7f, 0.6f));
			
			GUI.color = Color.white;
			spawner.enemies[index].bigImage = (Sprite)EditorGUI.ObjectField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 5 + 8, rect.width - 10, EditorGUIUtility.singleLineHeight), "Main image", spawner.enemies[index].bigImage, typeof(Sprite), false);
			spawner.enemies[index].smallImage = (Sprite)EditorGUI.ObjectField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 6 + 10, rect.width - 10, EditorGUIUtility.singleLineHeight), "Small image", spawner.enemies[index].smallImage, typeof(Sprite), false);
			spawner.enemies[index].name = EditorGUI.TextField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 7 + 12, rect.width - 10, EditorGUIUtility.singleLineHeight), "Enemy name", spawner.enemies[index].name);
			
			EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 8 + 18, rect.width - 10, EditorGUIUtility.singleLineHeight), "Description");
			spawner.enemies[index].description = EditorGUI.TextArea(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 9 + 18, rect.width - 10, EditorGUIUtility.singleLineHeight * 3), spawner.enemies[index].description);
		}
	};
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	enemyList.elementHeightCallback = (index) => { 
		
		float height = EditorGUIUtility.singleLineHeight * 4.5f;
		
		if(spawner.enemies[index].newEnemy)
			height += EditorGUIUtility.singleLineHeight * 9.5f;
	
		return height;
	};
	
	enemyList.onAddCallback = (ReorderableList l) => {  
    var index = l.serializedProperty.arraySize;
    l.serializedProperty.arraySize++;
    l.index = index;
    var element = l.serializedProperty.GetArrayElementAtIndex(index);
    element.FindPropertyRelative("spawnPointIndex").intValue = 0;
	element.FindPropertyRelative("enemyPrefab").objectReferenceValue = null;
	element.FindPropertyRelative("delay").floatValue = 0;
	};
	
	enemyList.onRemoveCallback = (ReorderableList l) => {  
    if (EditorUtility.DisplayDialog("Remove enemy", 
        "Are you sure you want to remove this enemy?", "Yes", "No")) {
        ReorderableList.defaultBehaviours.DoRemoveButton(l);
    }
	};
	
	enemyList.drawHeaderCallback = (Rect rect) => {  
    EditorGUI.LabelField(rect, "Enemies (" + spawner.enemies.Count + ")");
	};
	
	spawnpointsTexture = Resources.Load("spawnpoint") as Texture;
	enemiesTexture = Resources.Load("enemy") as Texture;
	settingsTexture = Resources.Load("settings") as Texture;
	
	spawnpointsTexture1 = Resources.Load("spawnpoint 1") as Texture;
	enemiesTexture1 = Resources.Load("enemy 1") as Texture;
	settingsTexture1 = Resources.Load("settings 1") as Texture;
	
	plus = Resources.Load("plus") as Texture;
	X = Resources.Load("x") as Texture;
	}
	
	public override void OnInspectorGUI(){	
	
	if(toolbarSelected == 0){
		spawnpointContent = new GUIContent(" Spawnpoints", spawnpointsTexture);
		settingsContent = new GUIContent(" Settings", settingsTexture1);
		enemyContent = new GUIContent(" Enemies", enemiesTexture);
	}
	else if(toolbarSelected == 1){
		spawnpointContent = new GUIContent(" Spawnpoints", spawnpointsTexture);
		settingsContent = new GUIContent(" Settings", settingsTexture);
		enemyContent = new GUIContent(" Enemies", enemiesTexture1);
	}
	else if(toolbarSelected == 2){
		spawnpointContent = new GUIContent(" Spawnpoints", spawnpointsTexture1);
		settingsContent = new GUIContent(" Settings", settingsTexture);
		enemyContent = new GUIContent(" Enemies", enemiesTexture);
	}
	
	GUILayout.Space(10);
	toolbarSelected = GUILayout.Toolbar(toolbarSelected, new GUIContent[] {settingsContent, enemyContent, spawnpointContent}, GUILayout.Height(25), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 50));
	
	if(toolbarSelected == 0){
	GUI.color = Color.white;
	GUILayout.Space(10);
	DrawDefaultInspector();
	}
	
	if(toolbarSelected == 1){
	GUILayout.Space(10);
	
	serializedObject.Update();
    enemyList.DoLayoutList();
	serializedObject.ApplyModifiedProperties();
	
	GUILayout.Space(-15);
	EditorGUILayout.BeginHorizontal();
	GUILayout.Space(2);
	GUI.color = new Color(0.7f, 1f, 0.7f, 1);
	if(GUILayout.Button(plus, GUILayout.Height(30), GUILayout.Width(30))){
		spawner.enemies.Add(new enemy{ spawnPointIndex = 0, enemyPrefab = spawner.defaultEnemy, delay = 0});
	}
	
	GUI.color = Color.white;
	spawner.defaultEnemy = EditorGUILayout.ObjectField(spawner.defaultEnemy, typeof(GameObject), false, GUILayout.Width(170)) as GameObject;
	EditorGUILayout.EndHorizontal();
	
	GUILayout.Space(10);
	if(GUILayout.Button("Clear all") && 
	EditorUtility.DisplayDialog("Clear all enemies",
	"Are you sure you want to clear all enemies?", "Yes", "No")){
	spawner.enemies.Clear();
	}
	}
	
	if(toolbarSelected == 2){
	GUILayout.Space(15);
	
	for(int i = 0; i < spawner.spawnpoints.Count; i++){
		EditorGUILayout.BeginHorizontal();
		
		GUILayout.Label(i + 1 + "", EditorStyles.boldLabel, GUILayout.Width(15));
		
		GUI.color = Color.white;
		EditorGUILayout.BeginVertical("Textfield");
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.BeginVertical();
		GUILayout.Space(3);
		spawner.spawnpoints[i].spawnPoint = EditorGUILayout.ObjectField("Spawn point", spawner.spawnpoints[i].spawnPoint, typeof(Transform), true) as Transform;
		GUILayout.Space(2);
		EditorGUI.BeginDisabledGroup(!spawner.showRangeWarning);
		spawner.spawnpoints[i].warning = EditorGUILayout.ObjectField("Range warning", spawner.spawnpoints[i].warning, typeof(GameObject), true) as GameObject;
		GUILayout.Space(2);
		EditorGUI.EndDisabledGroup ();
		EditorGUILayout.EndVertical();
		
		GUI.color = new Color(1f, 0.5f, 0.5f, 1);
		if(GUILayout.Button(X, GUILayout.Width(20), GUILayout.Height(36)) 
		&& EditorUtility.DisplayDialog("Remove spawnpoint",
		"Are you sure you want to remove this spawnpoint?", "Yes", "No")){
		spawner.spawnpoints.RemoveAt(i);
		}
		GUI.color = Color.white;
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
		
		EditorGUILayout.EndHorizontal();
		
		GUILayout.Space(3);
	}
	
	EditorGUILayout.BeginHorizontal();
	GUILayout.Space(20);
	GUI.color = new Color(0.7f, 1f, 0.7f, 1);
	if(GUILayout.Button(plus, GUILayout.Height(30), GUILayout.Width(30))){
		spawner.spawnpoints.Add(new spawnpoint{ spawnPoint = null, warning = null});
	}
	EditorGUILayout.EndHorizontal();
	
	GUI.color = Color.white;
	
	GUILayout.Space(10);
	spawner.showRangeWarning = GUILayout.Toggle(spawner.showRangeWarning, "Use range warnings", EditorStyles.toolbarButton);
	
	serializedObject.Update();
	updateSpawnPointNames();
	serializedObject.ApplyModifiedProperties();
	
	if(GUILayout.Button("Clear all") && 
	EditorUtility.DisplayDialog("Clear all spawnpoints",
	"Are you sure you want to clear all spawnpoints?", "Yes", "No")){
	spawner.spawnpoints.Clear();
	}	
	}
	
    serializedObject.ApplyModifiedProperties();
	Undo.RecordObject(spawner, "change in spawner");
	}
	
	void updateSpawnPointNames(){
		if(spawner.spawnpoints.Count > 0)
		spawnPointNames.Clear();
	
		for(int i = 0; i < spawner.spawnpoints.Count; i++){
			if(spawner.spawnpoints[i].spawnPoint != null)
			spawnPointNames.Add(spawner.spawnpoints[i].spawnPoint.gameObject.name);
		}
	}
}