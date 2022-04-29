using UnityEngine;

public class DeactivateOnExit : StateMachineBehaviour
{
	[SerializeField] bool child = true;
	[SerializeField] int childIndex;
	
	GameObject obj;
	
    override public void OnStateExit(
		Animator animator,
		AnimatorStateInfo stateInfo,
		int layerIndex
	){
		if(!obj){
			obj = child?
				animator.transform.GetChild(childIndex).gameObject:
				animator.gameObject;
		}
		
		obj.SetActive(false);
	}
}