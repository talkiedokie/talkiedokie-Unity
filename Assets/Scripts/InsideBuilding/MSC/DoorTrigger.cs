using UnityEngine;
using System.Collections;

public class DoorTrigger : MonoBehaviour
{
	float direction, timer;
	bool isOpen;
	
	public float openDuration = 0.65f;
	
	public AnimationCurve curve;
	
	[Space()]
	public Animator anim;
	public string _param = "amount";
	int param;
	
	IEnumerator routine;
	
	void Awake(){
		param = Animator.StringToHash(_param);
	}
	
	public void Animate(float direction){
		this.direction = direction;
		isOpen = !isOpen;
		
		if(routine != null)
			StopCoroutine(routine);
		
		routine = Routine();
		StartCoroutine(routine);
	}
	
	IEnumerator Routine(){
		if(isOpen){
			while(timer < openDuration){
				float lerp = curve.Evaluate(timer / openDuration);
				anim.SetFloat(param, lerp * direction);
				
				timer += Time.deltaTime;
				yield return null;
			}
		}
		
		else{
			while(timer > 0f){
				float lerp = curve.Evaluate(timer / openDuration);
				anim.SetFloat(param, lerp * direction);
				
				timer -= Time.deltaTime;
				yield return null;
			}
		}
	}
}