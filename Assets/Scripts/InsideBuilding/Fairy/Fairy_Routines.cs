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
		isSpeaking = true;
		anim.SetBool("talk", isSpeaking);
		
		var step = new WaitForSeconds(delay);
		
		foreach(var clip in clips){
			source.Stop();
			source.clip = clip;
			
			yield return step;
			
			if(!source.clip)
				source.clip = noAudioClip;
			
			source.Play();
			
			yield return WaitForPlayback();
		}
	
		isSpeaking = false;
		anim.SetBool("talk", isSpeaking);
		
		onFinish?.Invoke();
	}
	
	IEnumerator WaitForPlayback(){
		bool isPlaying = true;
		
		while(isPlaying){
			isPlaying = source.isPlaying;
			
			if(isSpeakStopped){
				source.Stop();
				isSpeakStopped = false;
				
				Debug.LogWarning("Fairy Speak Canceled");
				yield break;
			}
			
			yield return null;
		}
	}
}