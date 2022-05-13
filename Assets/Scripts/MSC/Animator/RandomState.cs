using UnityEngine;

public class RandomState : StateMachineBehaviour
{
	public int stateCount = 7;	
	int param = Animator.StringToHash("idleState");
	
    override public void OnStateEnter(
		Animator animator,
		AnimatorStateInfo stateInfo,
		int layerIndex
	){
		animator.SetInteger(param, Random.Range(0, stateCount));
	}
}