using System.Collections;
using UnityEngine;

public partial class SpeechRecognizer
{
	IEnumerator timer;
	
	IEnumerator Timer(){
		var timer = new Vector2(
			stt.listenDuration,
			stt.listenDuration
		);
		
		while(timer.x > 0f){
			ui.UpdateTimer(timer.x / timer.y);
			timer.x -= Time.deltaTime;
			yield return null;
		}
		
		OnResult();
	}
	
	void StopListenTimer(){
		if(timer != null)
			StopCoroutine(timer);
	}
}
