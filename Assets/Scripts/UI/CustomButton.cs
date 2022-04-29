using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class CustomButton : MonoBehaviour
{
	[SerializeField] protected float onClickDelay = 0.65f;
	[SerializeField] protected UnityEvent onClick;
	
	public void OnButtonClick(){
		// Add delay for animations
		StopAllCoroutines();
		StartCoroutine(OnClick_Routine());
	}
	
	IEnumerator OnClick_Routine(){
		yield return new WaitForSeconds(onClickDelay);
		onClick?.Invoke();
	}
}