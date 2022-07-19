using UnityEngine;
using UnityEngine.SceneManagement;
using System;

#if UNITY_EDITOR

	using UnityEditor;
	using System.IO;
	
#endif

public partial struct SceneLoader
{
	#if UNITY_EDITOR
	
		[CustomPropertyDrawer(typeof(SceneLoader))]
		private class SceneLoaderPropertyDrawer : PropertyDrawer
		{
			private SerializedProperty
				serializedIndex,
				serializeLabel,
				serializeCurrentSceneName;
			
			private static string[] names;
			
			public override void OnGUI(
				Rect position,
				SerializedProperty property,
				GUIContent label
			){
				EditorGUI.BeginProperty(position, label, property);
				{
					serializedIndex = property.FindPropertyRelative(nameof(index));
					serializeLabel = property.FindPropertyRelative(nameof(label));
					serializeCurrentSceneName = property.FindPropertyRelative(nameof(currentSceneName));
					
					int indexValue = serializedIndex.intValue;
					string propertyName = serializeLabel.stringValue;
						
						if(propertyName == ""){
							propertyName = property.displayName;
							serializeLabel.stringValue = propertyName;
						}
					
					var indexRect = new Rect(
						position.x,
						position.y,
						position.width,
						EditorGUIUtility.singleLineHeight
					);
					
					GetSceneNames();
					CheckSceneIndexToName(indexValue, propertyName);
					
					serializedIndex.intValue = EditorGUI.Popup(
						indexRect,
						propertyName,
						indexValue,
						names
					);
					
					RecordSceneName(indexValue);
				}
				EditorGUI.EndProperty();
			}
			
			private void GetSceneNames(){
				int count = SceneManager.sceneCountInBuildSettings;
				
				if(names != null){
					if(names.Length == count)
						return;
				}
				
				names = new string[count];
				
				for(int i = 0; i < count; i++){
					string path = SceneUtility.GetScenePathByBuildIndex(i);
					string name = Path.GetFileNameWithoutExtension(path);
					
					names[i] = name;
				}
			}
			
			#region Debugging
				
				private void CheckSceneIndexToName(int index, string propertyName){
					string currentSceneName = serializeCurrentSceneName.stringValue;
					
						if(currentSceneName == "") return;
						
					string selectedSceneName = names[index];
					
					if(currentSceneName != selectedSceneName){
						int findIndex = Array.FindIndex(names, name => name == currentSceneName);
						
						if(findIndex < 0) // or not found
							Debug.LogWarning("'" + propertyName + "' selected scene is not " + currentSceneName + ", but " + selectedSceneName);
						
						else serializedIndex.intValue = findIndex;
					}
				}
				
				private void RecordSceneName(int index){
					serializeCurrentSceneName.stringValue = names[index];
				}
				
			#endregion
		}
		
	#endif
}