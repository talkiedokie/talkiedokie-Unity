using UnityEngine;
using System.Collections;

public class DoorTrigger : MonoBehaviour
{
	float direction, timer;
	bool isOpen;
	
	public float openDuration = 0.65f;
	
	public AnimationCurve curve; // UnityEditor.AnimationCurveWrapperJSON:{"curve":{"serializedVersion":"2","m_Curve":[{"serializedVersion":"3","time":0.0,"value":0.0,"inSlope":2.0,"outSlope":2.0,"tangentMode":0,"weightedMode":0,"inWeight":0.0,"outWeight":0.0},{"serializedVersion":"3","time":1.0,"value":1.0,"inSlope":0.0,"outSlope":0.0,"tangentMode":0,"weightedMode":0,"inWeight":0.0,"outWeight":0.0}],"m_PreInfinity":2,"m_PostInfinity":2,"m_RotationOrder":4}}
	
	[Space()]
	public Animator anim;
	public string _param = "amount";
	int param;
	
	IEnumerator routine;
	
	void Awake(){
		param = Animator.StringToHash(_param);
	}
	
	public void Animate(float dir){
		direction = isOpen? anim.GetFloat(param): dir;
		isOpen = !isOpen;
		
		Tools.StartCoroutine(ref routine, Routine(), this);
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