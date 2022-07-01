using System;
using UnityEngine;

public partial class Fairy
{
	public float afterSpeakingDelay = 0.1f;
	
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
		
		Tools.StartCoroutine(
			ref speakRoutine,
			SpeakRoutine(clips, startDelayForeach, onFinish),
			this
		);
	}
	
	public void SpeakRandom(
		AudioClip[] clips,
		float delay,
		Action onFinish
	){
		var clip = Tools.Random(clips);
		Speak(clip, delay, onFinish);
	}
	
	#region Speak Overloads
		
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
		
		public void Speak(
			AudioClip clip,
			float startDelay
		) =>
			Speak(clip, startDelay, null);
		
	#endregion
	
	#region Encourage
	
		public void SayWow(float delay, Action onFinish) =>
			SpeakRandom(wowClips, delay, onFinish);
		
		public void SayFail(float delay, Action onFinish) =>
			SpeakRandom(failClips, delay, onFinish);
		
	#endregion
	
	public void StopSpeaking() => isSpeakStopped = true;
}