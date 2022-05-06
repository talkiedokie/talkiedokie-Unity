using UnityEngine;
using System;
using System.Collections;

public class CoinAnimator : SpriteSheetAnimationUI
{
	static CoinHUD hud => CoinHUD.Instance;
	public static Action onDestinationReached;
	
	void OnEnable(){
		StopAllCoroutines();
		StartCoroutine(GoToDestination());
	}
	
	IEnumerator GoToDestination(){
		yield return new WaitForSeconds(hud.coinPopupAnimTravelDelay);
		
		var startPos = transform.position;
		var destination = hud.coinPopupTravelDestination.position;
		
		float timer = 0f;
		float maxDuration = hud.coinPopupAnimTravelTime;
		
		while(timer < maxDuration){
			float lerp = hud.coinPopupTravelCurve.Evaluate(timer / maxDuration);
				
				transform.position = Vector3.Lerp(startPos, destination, lerp);
			
			timer += Time.deltaTime;
			yield return null;
		}
		
		onDestinationReached?.Invoke();
		hud.Pool(gameObject);
	}
}