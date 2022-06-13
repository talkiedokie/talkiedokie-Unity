using UnityEngine;
using UnityEngine.Audio;
using System;

public partial class Fairy : SceneObjectSingleton<Fairy>
{
	[SerializeField] AudioSource source;
	[SerializeField] AudioClip noAudioClip;
	
	[SerializeField] AudioClip[]
		pleaseSayClips, cantHearClips,
		wowClips, failClips;
	
	[Space()]
	[SerializeField] Animator anim;
	[SerializeField] GameObject[] appearParticles;
	[SerializeField] GeneralAudioSelector appearSound, disappearSound;
	
	SpeechRecognizer speechRecognizer;
	
	public Action onInteraction;
	
	void Awake(){
		speechRecognizer = SpeechRecognizer.Instance;
	}
	
	[ContextMenu("Appear")]
	public void Appear(){
		if(gameObject.activeSelf) return;
		
		OnAppearDisappear(true);
		appearSound.Play();
	}
	
	[ContextMenu("Disappear")]
	public void Disappear(){
		if(!gameObject.activeSelf) return;
		
		OnAppearDisappear(false);
		disappearSound.Play();
	}
	
	void OnAppearDisappear(bool appear){
		var particlePrefab = Tools.Random(appearParticles);
		
		var particle = Instantiate(
			particlePrefab,
			transform.position,
			particlePrefab.transform.rotation
		);
		
		Destroy(particle, 2f);
		gameObject.SetActive(appear);
	}
	
	public void OnInteraction(){
		onInteraction?.Invoke();
	}
}