using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class NPC : MonoBehaviour
{
	[SerializeField] Collider[] floors;
	[SerializeField] float idleDuration = 10f, animSmooth = 0.1f;
	
	Vector3 targetPosition;
	
	float speedPercent, agentSpeedSqr;
	Vector3 previousPosition;
	
	NavMeshAgent agent;
	Animator anim;
	
	int blend = Animator.StringToHash("blend");
	
	IEnumerator Start(){
		agent = GetComponent<NavMeshAgent>();
		anim = GetComponentInChildren<Animator>();
		
		agentSpeedSqr = Mathf.Pow(agent.speed, 2);
		
		while(true)
			yield return GoToRandomLocation();
	}
	
	IEnumerator GoToRandomLocation(){
		var floor = floors[Random.Range(0, floors.Length)];
		var bounds = floor.bounds;
		
		var min = bounds.min;
		var max = bounds.max;
		
		targetPosition = new Vector3(
			Random.Range(min.x, max.x),
			Random.Range(min.y, max.y),
			Random.Range(min.z, max.z)
		);
		
		agent.SetDestination(targetPosition);
		while(speedPercent > 0f) yield return null;
		
		float idle = idleDuration * Random.value;
		yield return new WaitForSeconds(idle);
	}
	
	void Update(){
		float deltaTime = Time.deltaTime;
		var velocity = (transform.position - previousPosition) / deltaTime;
		previousPosition = transform.position;
		
		speedPercent = Mathf.Clamp01(velocity.sqrMagnitude / agentSpeedSqr);
		anim.SetFloat(blend, speedPercent, animSmooth, deltaTime);
	}
	
	void OnDrawGizmosSelected(){
		Gizmos.DrawWireSphere(targetPosition, 0.5f);
	}
}