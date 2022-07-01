using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Prototype.Cars
{
	public class CarManager : SceneObjectSingleton<CarManager>
	{
		public Car[] cars;
		public LayerMask obstacleLayers;
		public float customUpdateRate = 5f;
		
		public Vector2 speedMinMax = new Vector2(5f, 20f);
		
		public float
			obstacleCheckRate = 5f,
			
			obstacleCheckDistance = 20f,
			obstacleCheckSphereCastRadius = 1f;
		
		public float
			stoppingDistance = 5f,
			rotationSpeed = 5f,
			turningDistance = 1f,
			signalCheckAmount = 0.75f;
		
		[Range(0,1)]
		public float stopLightBreakDistPercent = 0.35f;
		
		[Space()]
		public bool obstacleCheck = true;
		
		[Range(0,1)][SerializeField] float
			averageSpeed = 1f,
			antiJammingTrigger = 0.5f;
		
		public GameObject[] explosionEffects;
		
		void Start(){
			StartCoroutine(CheckCarsObstacle());
			// StartCoroutine(CheckForTrafficJams());
			StartCoroutine(CustomUpdates());
		}
		
		public float GetRandomSpeed() =>
			Random.Range(speedMinMax.x, speedMinMax.y);
		
		IEnumerator CheckCarsObstacle(){
			var step = new WaitForSeconds(1f / obstacleCheckRate);
			
			while(true){
				foreach(var car in cars)
					car.CheckForObstacle(
						obstacleCheckDistance,
						obstacleLayers
					);
				
				CalculateCarSpeedAverage();
				
			if(averageSpeed < antiJammingTrigger)
					yield return ResetObstacleAvoidance();
				
				yield return step;
			}
		}
		
		/* IEnumerator CheckForTrafficJams(){
			var step = new WaitForSeconds(1f / customUpdateRate);
			var jammedCars = new List<Car>();
			
			while(true){
				jammedCars.Clear();
				
				foreach(var car in cars){
					if(!car.isStopped) continue;
					
					bool isJammed = car.stopDuration >= 5f;
					if(isJammed) jammedCars.Add(car);
				}
				
				foreach(var car in cars){
					
				yield return step;
			}
		} */
		
		IEnumerator CustomUpdates(){
			var step = new WaitForSeconds(1f / customUpdateRate);
			
			while(true){
				foreach(var car in cars)
					car.UpdateLightings();
				
				yield return step;
			}
		}
		
 		void CalculateCarSpeedAverage(){
			float totalSpeed = 0f;
			float count = 0f;
			
			foreach(var car in cars){
				if(car.isStoppingByStopLight) continue;
				
				totalSpeed += car.currentSpeedPercent;
				count ++;
			}
			
			averageSpeed = totalSpeed / count;
		}
		
	 	IEnumerator ResetObstacleAvoidance(){
			obstacleCheck = false;
			Explode();
			
			yield return new WaitForSeconds(2f);
			obstacleCheck = true;
		}
		
		void Explode(/* Carp[] cars */){
			Bounds bounds = new Bounds();
			
			foreach(var car in cars){
				if(car.currentSpeedPercent < 0.1f)
					bounds.Encapsulate(car._transform.position);
			}
			
			var explodePrefab = Tools.Random(explosionEffects);
			var explodeParticle = Instantiate(explodePrefab, bounds.center, explodePrefab.transform.rotation);
			
			Destroy(explodeParticle, 2f);
		}
		
		void OnDrawGizmos(){
			foreach(var car in cars){
				var position = car.obstacleCheckPoint.position;
				var direction = car.obstacleCheckPoint.forward;
				
				Gizmos.color = Color.green;
				Gizmos.DrawLine(position, position + direction * obstacleCheckDistance);
				
				Gizmos.color = Color.red;
				Gizmos.DrawLine(position, position + direction * stoppingDistance);
			}
		}
	}
}