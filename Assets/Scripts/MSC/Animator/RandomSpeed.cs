using UnityEngine;
using UnityEngine.Events;

public class RandomSpeed : StateMachineBehaviour
{
	[SerializeField] float speed = 1f;
	static int param = Animator.StringToHash("speed");
	
	[Space()]
	[SerializeField] UnityEvent test;
	
    override public void OnStateEnter(
		Animator animator,
		AnimatorStateInfo stateInfo,
		int layerIndex
	){
		animator.SetFloat(param, speed * Random.value);
		test?.Invoke();
	}
}