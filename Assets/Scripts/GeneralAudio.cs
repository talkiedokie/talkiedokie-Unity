using UnityEngine;
using UnityEngine.Audio;
using System;

[CreateAssetMenu]
public class GeneralAudio : Singleton<GeneralAudio>
{
	[SerializeField] AudioClip[] clips;
	
	AudioSource source;
	AudioSource Source{
		get{
			if(!source){
				source = new GameObject("GeneralAudioPlayer", typeof(AudioSource)).GetComponent<AudioSource>();
				DontDestroyOnLoad(source.gameObject);
			}
			
			return source;
		}
	}
	
	public void Play(int clipIndex){
		var clip = clips[clipIndex];
		
		Play(clip);
	}
	
	public void Play(string clipName){
		var clip = Array.Find(clips, clip => clip.name == clipName);
		Play(clip);
	}
	
	public void Play(AudioClip clip){
		Source.Stop();
		Source.clip = clip;
		Source.Play();
	}
}