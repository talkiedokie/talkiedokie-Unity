using UnityEngine;

namespace Prototype.Cars
{
	public class WayPointDrawer : SceneObjectSingleton<WayPointDrawer>
	{
		public CarTargetPoint[] targetPoints;
		
		[Space()]
		public Mesh gizmoMesh;
		
		public float
			gizmoScale = 0.5f,
			spacing = 1f;
		
		public int maxIteration = 100;
		
		void OnDrawGizmos(){
			var scale = Vector3.one * gizmoScale;
			
			foreach(var targetPoint in targetPoints){
				if(!targetPoint) continue;
				
				Gizmos.color = targetPoint.color;
				var origin = targetPoint.position;
				
				foreach(var point in targetPoint.connectedPoints){
					if(!point) continue;
					
					var direction = point.position - origin;
					
					if(direction == Vector3.zero){
						Debug.LogError(point, point);
						Debug.LogError(targetPoint, targetPoint);
						continue;
					}
					
					var rotation = Quaternion.LookRotation(-direction);
					
					float currentSpace = spacing;
					float maxDistanceSqr = direction.sqrMagnitude;
					
					for(int i = 0; i < maxIteration; i++){
						float currentSpaceSqr = currentSpace * currentSpace;
						if(currentSpaceSqr > maxDistanceSqr) break;
						
						var position = origin + (direction.normalized * currentSpace); 
						Gizmos.DrawMesh(gizmoMesh, position, rotation, scale);
						
						currentSpace += spacing;
					}
				}
			}
		}
		
		[ContextMenu("Get Way Points")]
		public void GetWayPoints() =>
			targetPoints = FindObjectsOfType<CarTargetPoint>();
	}
}