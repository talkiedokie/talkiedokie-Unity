using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class Fish : MonoBehaviour
{
	[SerializeField] float
		speed = 0.5f,
		updateDuration = 5f,
		gizmoRadius = 0.25f;
	
	Vector3
		targetPosition,
		rotateSmoothVel;
	
	Transform target;
	
	Aquarium aquarium;
	IEnumerator wanderingRoutine;
	
	void Awake(){
		aquarium = GetComponentInParent<Aquarium>();
		aquarium.AddSwimmers(this);
	}
	
	void OnBecameVisible(){
		enabled = true;
		
		wanderingRoutine = Wander();
		StartCoroutine(wanderingRoutine);
	}
	
	void OnBecameInvisible(){		
		if(wanderingRoutine != null)
			StopCoroutine(wanderingRoutine);
		
		wanderingRoutine = null;
		enabled = false;
	}
	
	IEnumerator Wander(){
		while(true){
			if(target){
				targetPosition = target.position;
				yield return null;
			}
			
			else{
				var min = aquarium.waterBounds.min;
				var max = aquarium.waterBounds.max;
				
				targetPosition = new Vector3(
					Random.Range(min.x, max.x),
					Random.Range(min.y, max.y),
					Random.Range(min.z, max.z)
				);
				
				yield return new WaitForSeconds(updateDuration * Random.value);
			}
		}
	}
	
	void LateUpdate(){
		var dir = targetPosition - transform.position;
		
		var velocity = dir * speed * Time.deltaTime;
		var displacement = transform.position + velocity;
			
			transform.position = aquarium.Clamp(displacement);
		
		transform.forward = Vector3.SmoothDamp(
			transform.forward,
			dir.normalized,
			ref rotateSmoothVel,
			0.5f
		);
	}
	
	void OnDrawGizmos(){
		if(Application.isPlaying)
			Gizmos.DrawSphere(targetPosition, gizmoRadius);
	}
	
	public void SetTargetPosition(Transform target){
		this.target = target;
	}
}