using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelScenesManager))]
public class LevelScenesManagerEditor : Editor
{
	LevelScenesManager script;
	
	public int
		testHouseSceneSelector,
		testMinigameSceneSelector;
	
	void OnEnable() =>
		script = (LevelScenesManager) target;
	
	public override void OnInspectorGUI(){
		DrawDefaultInspector();
		// EditorGUILayout.Space();
		
		DrawField(
			"House Level Scenes Only",
			ref testHouseSceneSelector,
			script.insideBuildingLevels
		);
		
		DrawField(
			"Minigame Scenes Only",
			ref testMinigameSceneSelector,
			script.minigameLevels
		);
	}
	
	void DrawField(string label, ref int sceneSelector, SceneLoader[] list){
		int count = list.Length;
		var names = new string[count];
		
		for(int i = 0; i < count; i ++)
			names[i] = list[i].currentSceneName;
		
		GUILayout.BeginHorizontal();
		{
			sceneSelector = EditorGUILayout.Popup(label, sceneSelector, names);
			
			if(GUILayout.Button("Load")){
				if(Application.isPlaying)
					list[sceneSelector].Load();
				
				else
					Debug.LogWarning("Please Enter Play mode");
			}
		}
		GUILayout.EndHorizontal();
	}
}