using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Spline), true), CanEditMultipleObjects]
public class SplineHandles : Editor
{
	Spline script;
	
	void OnEnable(){
		script = (Spline) target;
	}
	
	public override void OnInspectorGUI(){
		base.OnInspectorGUI();
		
		SplineDrawer.enablePositionHandle = EditorGUILayout.ToggleLeft(
			"Position Handle",
			SplineDrawer.enablePositionHandle
		);
	}
	
	void OnSceneGUI(){
		if(script.connectedPoints == null) return;
		
		foreach(var point in script.connectedPoints){
			switch(point.interpolationType){
				case Spline.ConnectedPoint.LIN: DrawLinear(point); break;
				case Spline.ConnectedPoint.QUA: DrawQuadratic(point); break;
				case Spline.ConnectedPoint.CUB: DrawCubic(point); break;
			}
		}
	}
	
	void DrawLinear(Spline.ConnectedPoint point){
		// Handles.DrawLine(script.position, point.target.position);
	}
	
	void DrawQuadratic(Spline.ConnectedPoint point){
		var target = point.target;
		if(!target) return;
		
		DrawHandle(ref point.handleA);
		
		Handles.DrawLine(script.position, point.handleA);
		Handles.DrawLine(point.handleA, target.position);
	}
	
	void DrawCubic(Spline.ConnectedPoint point){
		var target = point.target;
		if(!target) return;
		
		DrawHandle(ref point.handleA);
		DrawHandle(ref point.handleB);
		
		Handles.DrawLine(script.position, point.handleA);
		Handles.DrawLine(point.handleB, target.position);
	}
	
	void DrawHandle(ref Vector3 position){
		var rotation = Quaternion.identity;
		var newPosition = position;
		
		if(SplineDrawer.enablePositionHandle)
			newPosition = Handles.PositionHandle(position, rotation);
		
		else
			newPosition = Handles.FreeMoveHandle(
				position, rotation,
				0.1f, Vector3.zero,
				Handles.SphereHandleCap
			);
		
		if(position != newPosition){
			EditorUtility.SetDirty(script);
			Undo.RecordObject(script, "Move Handle");
			
			position = newPosition;
		}
	}
}