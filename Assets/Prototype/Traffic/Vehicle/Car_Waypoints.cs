using UnityEngine;

namespace Prototype.TrafficSystems
{
	public partial class Car
	{
		[SerializeField] Waypoint currentWaypoint;
		
		Waypoint
			targetWaypoint, // connected from currentWaypoint
			nextWaypoint; // connected from targetWaypoint, useful for turn-signaling and traffic light checking
		
		int
			targetWaypoint_Index,
			nextWaypoint_Index;
		
		float
			currentPathLength,
			currentPathDistance,
			pathDstPercent;
		
		Vector3 previousPosition;
		
		void SetupWaypoints(){ // start
			GetRandomPoint(currentWaypoint, ref targetWaypoint, ref targetWaypoint_Index);
			GetRandomPoint(targetWaypoint, ref nextWaypoint, ref nextWaypoint_Index);
			
			currentPathLength = currentWaypoint.connectedPoints[targetWaypoint_Index].length;
		}
		
		void UpdateSplinePosition(){ // called every frame
			float linearVelocity = speed * speedPercent_Smooth * Time.deltaTime; // see "speedPercent_Smooth" from "Car_SpeedHandler" partial script
			currentPathDistance += linearVelocity;
			
			if(currentPathDistance > currentPathLength){
				GetNewWaypoints();
				
				currentPathLength = currentWaypoint.connectedPoints[targetWaypoint_Index].length;
				currentPathDistance = 0f;
			}
			
			pathDstPercent = currentPathDistance / currentPathLength;
		}
		
		void GetNewWaypoints(){ // called once
			currentWaypoint = targetWaypoint;
			
			targetWaypoint = nextWaypoint;
			targetWaypoint_Index = nextWaypoint_Index;
			
			GetRandomPoint(
				targetWaypoint,
				ref nextWaypoint,
				ref nextWaypoint_Index
			);
		}
		
		void GetRandomPoint( // called once
			Waypoint fromWaypoint, // what waypoint array reference we will randomly select on
			ref Waypoint toWaypoint, // where do you store the selected point
			ref int toWaypoint_Index // where do you store the index of selected point
		){
			var connectedPoints = fromWaypoint.connectedPoints;
				
				toWaypoint_Index = Random.Range(0, connectedPoints.Length);
				toWaypoint = connectedPoints[toWaypoint_Index].target as Waypoint;
		}
	}
}