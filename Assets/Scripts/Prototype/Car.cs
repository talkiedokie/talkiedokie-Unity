using UnityEngine;
using System.Collections;

namespace LevelSelect
{
	public class Car : MonoBehaviour
	{
		public float
			speed = 30f,
			turnSmoothTime = 0.25f,
			stoppingDistance = 1.25f;
		
		public Transform[] lanes;
		
		Building targetBldg;
		float rotationSmoothVel = 0f;
		
		[ContextMenu("Go to Random Building")]
		public void GoToBuilding(){
			var bldgs = FindObjectsOfType<Building>();
			var newTargetBldg = Tools.Random(bldgs);
			
			int iteration = 1;
			while(newTargetBldg == targetBldg){
				newTargetBldg = Tools.Random(bldgs);
				
				iteration ++;
				if(iteration > 9) break;
			}
			
			if(iteration > 1)
				Debug.LogWarning("Iteration: " + iteration, this);
		
			targetBldg = newTargetBldg;
			
			StopAllCoroutines();
			StartCoroutine(Drive());
		}
		
		IEnumerator Drive(){
			var bldgPos = targetBldg.transform.position;
			var targetPosition = GetLane(bldgPos);
			
			while(true){
				var direction = targetPosition - transform.position;
				float rotationAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
					
					transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(
						transform.eulerAngles.y,
						rotationAngle,
						ref rotationSmoothVel,
						turnSmoothTime
					);
				
				var moveDir = transform.forward;
				float angleToDir = Vector3.Angle(direction, moveDir);
				float throttle = Mathf.InverseLerp(180f, 0f, angleToDir);
				
				var velocity = moveDir * throttle * speed;
					transform.position += velocity * Time.deltaTime;
				
				yield return null;
			}
		}
		
		Vector3 GetLane(Vector3 position){
			if(position.x < 0) position.x = lanes[0].position.x;
			if(position.x > 0) position.x = lanes[1].position.x;
			
			return position;
		}
	}
}