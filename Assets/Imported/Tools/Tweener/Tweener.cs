using UnityEngine;

public class Tweener : MonoBehaviour
{
	[SerializeField] Transform target;
	[SerializeField] float speed = 1f;
	
	[SerializeField] bool
		position = true,
		rotation = true,
		scale = false;
	
	[Space()]
	[SerializeField] bool autoSleep = true;
	[SerializeField] float autoSleepDuration = 5f;
	
	float autoSleepTimer;
	
	void Update(){
		if(!target) return;
		
		float deltaTime = Time.deltaTime;
		
		if(autoSleep){
			enabled = autoSleepTimer > 0f;
			autoSleepTimer -= deltaTime;
		}
		
		float deltaSpeed = this.speed * deltaTime;
		
		if(position) transform.position = Vector3.Lerp(transform.position, target.position, deltaSpeed);
		if(rotation) transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, deltaSpeed);
		if(scale)	 transform.localScale = Vector3.Lerp(transform.localScale, target.localScale, deltaSpeed);
		
		
	}
	
	public void SetTarget(Transform target){
		if(this.target == target) return;
		this.target = target;
		
		autoSleepTimer = autoSleepDuration;
		enabled = true;
	}
	
	public Transform CreateDefaultPoint(){
		var newPoint = new GameObject(name).transform;
			newPoint.position = transform.position;
			newPoint.rotation = transform.rotation;
			newPoint.localScale = transform.localScale;
		
		SetTarget(newPoint);
		return newPoint;
	}
}