using UnityEngine;
using UnityEditor;

namespace InsideBuilding
{
	[CustomEditor(typeof(Room))]
	public class RoomEditor : Editor
	{
		void OnEnable(){
			// var script = (Room) target;
			
			// script.tasks = script.GetComponentsInChildren<Task>(true);
			
			// EditorUtility.SetDirty(script);
			// Undo.RecordObject(script, "Room tasks got references from child objects");
		}
	}
}