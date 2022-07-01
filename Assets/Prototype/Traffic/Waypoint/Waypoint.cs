using UnityEngine;

namespace Prototype.TrafficSystems
{
	public class Waypoint : Spline
	{
		public bool slowDown; // for merging roads (not recognized by "IsJunction")
		[HideInInspector] public bool isOpen = true; // actively modified by Traffic light
		
		void Awake(){
			foreach(var connectedPoint in connectedPoints)
				connectedPoint.RecalculateLength(position, 100);
		}
		
		public Vector3 GetPositionTo(int connectedPointIndex, float t){
			var point = connectedPoints[connectedPointIndex];
			return point.GetPosition(position, t);
		}
		
		public Waypoint GetConnectedPoint(int index){
			var waypoint = connectedPoints[index].target as Waypoint;
			return waypoint;
		}
		
		public bool IsJunction() => connectedPoints.Length > 1;
	}
}