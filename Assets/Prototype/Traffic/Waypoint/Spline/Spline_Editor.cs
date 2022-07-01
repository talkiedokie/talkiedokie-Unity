using UnityEngine;

public partial class Spline
{
	#if UNITY_EDITOR
	
		public Color gizmoColor;
		static SplineDrawer splineDrawer;
		
		void OnValidate(){
			if(connectedPoints == null) return;
			
			foreach(var point in connectedPoints){
				var target = point.target;
				
				if(target){
					if(point.name == target.name)
						continue;
					
					point.name = target.name;
					point.handleA = target.position;
					point.handleB = target.position;
				}
			}
			
			if(!splineDrawer)
				splineDrawer = GetComponentInParent<SplineDrawer>();
			
			splineDrawer.GetSplines();
		}
		
		public void DrawGizmo(
			float resolution,
			Mesh mesh,
			float meshScale,
			bool drawLine
		){
			Gizmos.color = gizmoColor;
			
			foreach(var point in connectedPoints){
				if(!point.target) continue;
				
				switch(point.interpolationType){
					case ConnectedPoint.LIN: DrawLine(point); break;
					case ConnectedPoint.QUA: DrawCurve(point); break;
					case ConnectedPoint.CUB: DrawCurve(point); break;
				}
				
				DrawMesh(point);
			}
			
			void DrawLine(ConnectedPoint point) =>
				Gizmos.DrawLine(position, point.target.position);
			
			void DrawCurve(ConnectedPoint point){
				var previous = position;
				float maxResIndex = resolution - 1;
				
				for(float i = 0; i < resolution; i++){
					var current = point.GetPosition(position, i / maxResIndex);
					
					if(drawLine || !mesh)
						Gizmos.DrawLine(previous, current);
					
					previous = current;
				}
			}
			
			void DrawMesh(ConnectedPoint point){
				if(!mesh) return;
				
				var midPoint = point.GetPosition(position, 0.5f);
				var nextPoint = point.GetPosition(position, 0.6f);
				
				var direction = nextPoint - midPoint;
				
				if(direction != Vector3.zero){
					var rotation = Quaternion.LookRotation(-direction, Vector3.up);
					
					Gizmos.DrawMesh(
						mesh, midPoint, rotation,
						Vector3.one * meshScale
					);
				}
			}
		}
		
	#endif
}