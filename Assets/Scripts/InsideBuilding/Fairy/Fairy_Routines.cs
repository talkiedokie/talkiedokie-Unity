using UnityEngine;
using System;
using System.Collections;

public partial class Fairy
{
	IEnumerator speakRoutine;
	
	IEnumerator SpeakRoutine(
		AudioClip[] clips,
		float delay,
		Action onFinish
	){
		speech.StopListening();
		
		isSpeaking = true;
		if(anim) anim.SetBool("talk", isSpeaking);
		
		var waitForDelay = new WaitForSeconds(delay);
		
		foreach(var clip in clips){
			source.Stop();
			source.clip = clip;
			
			yield return waitForDelay;
			source.Play();
			
			yield return WaitForPlayback(clip.length);
		}
	
		isSpeaking = false;
		if(anim) anim.SetBool("talk", isSpeaking);
		
		yield return new WaitForSeconds(afterSpeakingDelay);
		onFinish?.Invoke();
	}
	
	IEnumerator WaitForPlayback(float clipLength){
		float timer = 0f;
		
		while(source.isPlaying && timer < clipLength){
			if(isSpeakStopped){
				source.Stop();
				isSpeakStopped = false;
				
				Debug.LogWarning("Fairy Speak Canceled");
				yield break;
			}
			
			yield return null;
			timer += Time.deltaTime;
		}
	}
}