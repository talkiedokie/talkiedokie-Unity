using UnityEngine;
using System;
using System.Collections;

public partial class SpeechRecognizer
{
	void OnListen(
		Action<string> onResult,
		Action<Action, Action> pluginListenCall
	){
		this.onResult = onResult;
		pluginListenCall.Invoke(OnHypothesis, OnResult);
		
		gameObject.SetActive(true);
		ui.OnListen();
		
		isListening = true;
		isSkipped = false;
		
		Tools.StartCoroutine(ref timer, Timer(), this);
		GeneralAudio.Instance.SetBGMVolume(0.1f);
	}
	
	void OnHypothesis(){
		StopListenTimer();
		ui.OnHypothesis(stt.hypothesis);
	}
	
	void OnResult(){
		StartCoroutine(routine());
		
		IEnumerator routine(){
			stt.StopListening();
			ui.OnResult(result);
			
			yield return new WaitForSeconds(onFinishExitDelay);
			isListening = false;
			
			onResult?.Invoke(result);
			onResult = null;
		}
	}
}