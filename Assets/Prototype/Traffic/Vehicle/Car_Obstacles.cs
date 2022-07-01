using UnityEngine;
using System.Collections;

namespace Prototype.TrafficSystems
{
	public partial class Car
	{
		public Transform obstacleCheckPoint;
		
		public Collider currentObstacle{ get; private set; }
		SpeedModifier speedModifier_ObstacleCheck;
		
		bool ignoreObstacles; // anti traffic jam system
		IEnumerator ignoreObstacles_Routine = null;
		
		void InitializeSpeedModifier_ObstacleCheck(){
			speedModifier_ObstacleCheck = new SpeedModifier("Obstacle Check");
			speedModifiers.Add(speedModifier_ObstacleCheck);
		}
		
		public void CheckForObstacles(
			LayerMask layers,
			float sphereCastRadius,
			float maxDistance,
			float stoppingDistance
		){
			if(ignoreObstacles){
				currentObstacle = null;
				speedModifier_ObstacleCheck.value = 1f;
				return;
			}
			
			bool cast = Physics.SphereCast(
				obstacleCheckPoint.position,
				sphereCastRadius,
				obstacleCheckPoint.forward,
				out var hit,
				maxDistance,
				layers
			);
			
			if(cast){
				currentObstacle = hit.collider;
				
				float currentDst = hit.distance - stoppingDistance;
				float divider = maxDistance - stoppingDistance;
				
				speedModifier_ObstacleCheck.value = Mathf.Clamp01(currentDst / divider);
				
				#if UNITY_EDITOR
				
					obstacleCheckHitPoint = hit.point;
					obstacleCheckRadius = sphereCastRadius;
					
				#endif
			}
			else{
				currentObstacle = null;
				speedModifier_ObstacleCheck.value = 1f;
			}
		}
		
		public void IgnoreObstacles(float duration){
			if(ignoreObstacles_Routine != null) return;
			
			ignoreObstacles_Routine = routine();
			StartCoroutine(ignoreObstacles_Routine);
			
			IEnumerator routine(){
				ignoreObstacles = true;
				yield return new WaitForSeconds(duration);
				ignoreObstacles = false;
			}
			
			Debug.Log(name + " freed from traffic jam", this);
		}
		
		#if UNITY_EDITOR
			
			Vector3 obstacleCheckHitPoint;
			float obstacleCheckRadius;
			
			void DrawObstacleCheckGizmo(){
				if(!currentObstacle) return;
				
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(obstacleCheckHitPoint, obstacleCheckRadius);
			}
			
		#endif
	}
}