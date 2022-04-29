using UnityEngine;
using UnityEditor;

namespace Gameplay
{
	[CustomEditor(typeof(GameManager))]
	public class GameManagerEditor : Editor
	{
		GameManager script;
		bool foldout;
		
		SerializedProperty wowClipsProperty;
		
		void OnEnable(){
			script = (GameManager) target;
			wowClipsProperty = serializedObject.FindProperty("wowClips");
		}
		
		override public void OnInspectorGUI(){
			DrawDefaultInspector();
			EditorGUILayout.Space();
			
			GUILayout.BeginVertical("box");			
			{
				foldout = EditorGUILayout.Foldout(foldout, "Fairy Voice Clips", true);
				
				if(foldout){
					DrawField("Where do you want to go?", ref script.wdywtg_Clip, ref script.wdywtg_ClipDelay);
					DrawField("Let's Play the Word", ref script.lptw_Clip, ref script.awsm_ClipDelay);
					DrawField("You can now enjoy your free time!", ref script.ycneyft_Clip, ref script.ycneyft_ClipDelay);
					
					DrawObjectField("The word of the day is...", ref script.wotd_Clip1);
					DrawObjectField("I want you to say the word...", ref script.wotd_Clip2);
				}
			}	
			GUILayout.EndVertical();
		}
		
		void DrawField(string label, ref AudioClip clip, ref float delay){
			GUILayout.BeginHorizontal();
			{
				delay = EditorGUILayout.FloatField(label, delay);
				DrawObjectField("", ref clip);
			}
			GUILayout.EndHorizontal();
		}
		
		void DrawObjectField<T>(string label, ref T reference) where T : Object{
			reference = (label =="")?
				(T) EditorGUILayout.ObjectField(reference, typeof(T), true):
				(T) EditorGUILayout.ObjectField(label, reference, typeof(T), true);
		}
	}
}