using UnityEngine;
using System;
using System.Collections;

namespace Prototype.TrafficSystems
{
	public enum CarObstacleCheck{ Raycast, Shperecast }

	public class CarManager : MonoBehaviour
	{
		[SerializeField] Car[] cars;
		
		[Space()]
		[SerializeField] LayerMask obstacleLayers;
		
		[SerializeField] float
			sphereCastRadius = 1f,
			obstacleCheckDst = 10f,
			turnSignalCheckAmount = 0.15f,
			
			antiJammingDurationTrigger = 5f,
			antiJammingDuration = 2f;
		
		[Range(0,1)]
		[SerializeField] float
			obstacleCheckStoppingDistancePercent =  0.25f,
			trafficLightStopDstPercent = 0.9f,
			junctionMinSpdPercent = 0.25f;
		
		[Space()]
		[SerializeField] float refreshRate = 5f;
		
		[SerializeField] bool
			checkForObstacles = true,
			checkForTrafficLight = true,
			calculateSpeedPercent = true,
			checkForJunctions = true,
			checkForDirectionChange = true,
			handleBraking = true;
		
		WaitForSeconds step;
		
		void OnValidate() => step = new WaitForSeconds(1f / refreshRate);
		
		IEnumerator Start(){
			step = new WaitForSeconds(1f / refreshRate);
			
			while(true){ // create custom update to avoid heavy calculations every frame
				float stoppingDistance = obstacleCheckDst * obstacleCheckStoppingDistancePercent;
				
				foreach(var car in cars){
					if(checkForObstacles)
						car.CheckForObstacles(
							obstacleLayers,
							sphereCastRadius,
							obstacleCheckDst,
							stoppingDistance
						);
				
					if(checkForTrafficLight)
						car.CheckForTrafficLight(trafficLightStopDstPercent);
				
					if(calculateSpeedPercent)
						car.CalculateSpeedPercent();
					
					if(checkForJunctions)
						car.CheckForJunctions(junctionMinSpdPercent);
					
					if(checkForDirectionChange)
						car.CheckForDirectionChange(turnSignalCheckAmount);
					
					if(handleBraking)
						car.HandleBraking();
					
					if(car.stopDuration > antiJammingDurationTrigger)
						car.IgnoreObstacles(antiJammingDuration);
				}
				
				yield return step;
			}
		}
		
		void OnDrawGizmos(){
			foreach(var car in cars){
				var position = car.obstacleCheckPoint.position;
				var direction = car.obstacleCheckPoint.forward * obstacleCheckDst;
					
					Gizmos.color = Color.green;
					Gizmos.DrawLine(position, position + direction);
					
					Gizmos.color = Color.red;
					Gizmos.DrawLine(position, position + (direction * obstacleCheckStoppingDistancePercent));
			}
		}
	}
}