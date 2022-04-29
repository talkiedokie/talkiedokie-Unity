using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class Fairy : SceneObjectSingleton<Fairy>
{
	[SerializeField] AudioSource source;
	[SerializeField] AudioClip noAudioClip;
	
	IEnumerator speakRoutine;
	
	public bool isSpeaking{ get; private set; }
	
	#region Calls
		
		public void Speak(
			AudioClip[] clips,
			float startDelayForeach,
			Action onFinish
		){
			if(clips.Length == 0){
				AudioClip noClip = null;
				Speak(noClip, startDelayForeach, onFinish);
				return;
			}
			
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
				new AudioClip[0];
			
			Speak(array, startDelay, onFinish);
		}
		
		public void Speak(AudioClip clip, float startDelay){ 
			Speak(clip, startDelay, null);
		}
		
	#endregion
	
	#region Routines
		
		IEnumerator SpeakRoutine(
			AudioClip[] clips,
			float delay,
			Action onFinish
		){
			isSpeaking = true;
			
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
			onFinish?.Invoke();
		}
		
		IEnumerator WaitForPlayback(){
			bool isPlaying = true;
			
			while(isPlaying){
				isPlaying = source.isPlaying;
				
				if(Input.GetKeyDown("p")){
					source.Stop();
					
					Debug.LogWarning("Fairy Speak Canceled");
					yield break;
				}
				
				yield return null;
			}
		}
		
	#endregion
}