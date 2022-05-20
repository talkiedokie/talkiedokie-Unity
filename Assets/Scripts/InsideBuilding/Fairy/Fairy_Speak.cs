using System;
using UnityEngine;

public partial class Fairy
{
	public bool isSpeaking{ get; private set; }
	bool isSpeakStopped;
	
	public void Speak(
		AudioClip[] clips,
		float startDelayForeach,
		Action onFinish
	){
		if(!gameObject.activeSelf){
			Debug.LogWarning(name + " is trying to speak while inactive", this);
			return;
		}
		
		if(clips.Length == 0)
			clips = new AudioClip[]{ noAudioClip };
		
		if(speakRoutine != null)
			StopCoroutine(speakRoutine);
		
		speakRoutine = SpeakRoutine(clips, startDelayForeach, onFinish);
		StartCoroutine(speakRoutine);
	}
	
	public void Speak(
		AudioClip clip,
		float startDelay,
		Action onFinish
	){
		var array = clip?
			new AudioClip[]{ clip }:
			new AudioClip[]{ noAudioClip };
		
		Speak(array, startDelay, onFinish);
	}
	
	public void Speak(AudioClip clip, float startDelay){ 
		Speak(clip, startDelay, null);
	}
	
	#region Encourage
		
		public void SayWow(float delay, Action onFinish){
			Speak(Tools.Random(wowClips), delay, onFinish);
		}
		
		public void SayFail(float delay, Action onFinish){
			Speak(Tools.Random(failClips), delay, onFinish);
		}
		
	#endregion
	
	public void StopSpeaking(){ isSpeakStopped = true; }
}