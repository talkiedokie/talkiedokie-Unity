using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Prototype.Cars;

[RequireComponent(typeof(NavMeshAgent))]
public class NPC : MonoBehaviour
{
	[SerializeField] Collider[] floors;
	
	[SerializeField] float
		idleDuration = 10f,
		animSmooth = 0.1f,
		
		targetPositionGizmoSize = 1f;
	
	Vector3 targetPosition;
	
	float defaultSpeedValue, speedPercent;
	bool isRunning;
	
	NavMeshAgent agent;
	Animator anim;
	
	Transform _transform;
	Vector3 position;
	
	int blend = Animator.StringToHash("blend");
	
	IEnumerator onHonkRoutine;
	
	void Awake(){
		agent = GetComponent<NavMeshAgent>();
		anim = GetComponentInChildren<Animator>();
		_transform = transform;
	}
	
	IEnumerator Start(){
		defaultSpeedValue = agent.speed;
		Walk();
		
		while(true)
			yield return GoToRandomLocation();
	}
	
	IEnumerator GoToRandomLocation(){
		GetTargetPosition();
		
		agent.SetDestination(targetPosition);
		while(speedPercent > 0f) yield return null;
		
		float idle = idleDuration * Random.value;
		yield return new WaitForSeconds(idle);
	}
	
	[ContextMenu("Get Target Position")]
	public void GetTargetPosition(){
		var floor = Tools.Random(floors);
		var bounds = floor.bounds;
		
		var min = bounds.min;
		var max = bounds.max;
		
		targetPosition = new Vector3(
			Random.Range(min.x, max.x),
			Random.Range(min.y, max.y),
			Random.Range(min.z, max.z)
		);
		
		if(!Application.isPlaying)
			Debug.Log(targetPosition);
	}
	
	void Update(){
		CalculateVelocity();
		speedPercent = velocity.magnitude / defaultSpeedValue;
		
		if(anim)
			anim.SetFloat(blend, speedPercent, animSmooth, Time.deltaTime);
	}
	
	[ContextMenu("On Car Honk")]
	public void OnCarHonk(){
		if(onHonkRoutine != null) return;
	
		onHonkRoutine = routine();
		StartCoroutine(onHonkRoutine);
		
		IEnumerator routine(){
			GetTargetPosition();
			agent.SetDestination(targetPosition);
			
			Run();
			yield return new WaitForSeconds(1.5f);
			Walk();
			
			onHonkRoutine = null;
		}
	}
	
	[ContextMenu("Run")]
	public void Run() => agent.speed = defaultSpeedValue;
	
	[ContextMenu("Walk")]
	public void Walk() => agent.speed = defaultSpeedValue * 0.5f;
	
	void OnDrawGizmosSelected(){
		if(!_transform) _transform = transform;
		if(!Application.isPlaying) position = _transform.position;
		
		Gizmos.color = Color.red;
		Gizmos.DrawLine(position, targetPosition);
		Gizmos.DrawWireSphere(targetPosition, targetPositionGizmoSize);
	}
	
	Vector3 velocity, currentPos, prevPos;
	
	void CalculateVelocity(){
		currentPos = _transform.position;
		velocity = (currentPos - prevPos) / Time.deltaTime;
		prevPos = currentPos;
	}
}