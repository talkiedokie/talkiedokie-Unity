using UnityEngine;
using UnityEngine.Events;

public class StateEvents : StateMachineBehaviour
{
	[Space()]
	[SerializeField] UnityEvent onEnter, onUpdate, onExit;
	
    override public void OnStateEnter(
		Animator animator,
		AnimatorStateInfo stateInfo,
		int layerIndex
	){
		onEnter?.Invoke();
	}
	
	override public void OnStateUpdate(
		Animator animator,
		AnimatorStateInfo stateInfo,
		int layerIndex
	){
		onUpdate?.Invoke();
	}
	
	override public void OnStateExit(
		Animator animator,
		AnimatorStateInfo stateInfo,
		int layerIndex
	){
		onExit?.Invoke();
	}
}