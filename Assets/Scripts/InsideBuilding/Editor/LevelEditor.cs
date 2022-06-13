using UnityEngine;
using UnityEditor;

namespace InsideBuilding
{
	[CustomEditor(typeof(Level))]
	public class LevelEditor : Editor
	{
		Level script;
		string[] roomNames;
		
		void OnEnable(){
			script = (Level) target;
			
			int count = script.rooms.Length;
			roomNames = new string[count];
			
			for(int i = 0 ; i < count; i++)
				roomNames[i] = script.rooms[i].name;
		}
		
		public override void OnInspectorGUI(){
			DrawDefaultInspector();
			
			script.selectedRoomIndex = EditorGUILayout.Popup(
				"Selected Room",
				script.selectedRoomIndex,
				roomNames
			);
			
			if(GUI.changed){
				EditorUtility.SetDirty(script);
				Undo.RecordObject(script, script.name);
			}
		}
	}
}