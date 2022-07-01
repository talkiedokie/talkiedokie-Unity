using UnityEngine;

public class SplineDrawer : MonoBehaviour
{
	public Spline[] splines;
	
	[SerializeField] float resolution = 20f;
	
	[SerializeField] Mesh gizmoMesh;
	[SerializeField] float gizmoMeshScale = 0.25f;
	
	[SerializeField] bool drawLine = true;
	
	#if UNITY_EDITOR
		
		public static bool enablePositionHandle;
		
		public void GetSplines() =>
			splines = GetComponentsInChildren<Spline>();
		
		void OnDrawGizmos(){
			foreach(var spline in splines)
				if(spline) spline.DrawGizmo(resolution, gizmoMesh, gizmoMeshScale, drawLine);
		}
		
	#endif
}