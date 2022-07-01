using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SplineDrawer))]
public class SplineDrawerEditor : Editor
{
	SplineDrawer script;
	
	void OnEnable(){
		script = (SplineDrawer) target;
		script.GetSplines();
	}
	
	public override void OnInspectorGUI(){
		base.OnInspectorGUI();
		
		SplineDrawer.enablePositionHandle = EditorGUILayout.ToggleLeft(
			"Position Handle",
			SplineDrawer.enablePositionHandle
		);
	}
}