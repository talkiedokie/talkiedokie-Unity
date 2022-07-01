using UnityEngine;
using UnityEngine.Audio;
using System;
using Gameplay;

public partial class Fairy : SceneObjectSingleton<Fairy>
{
	[SerializeField] AudioSource source;
	[SerializeField] AudioClip noAudioClip;
	
	[SerializeField] AudioClip[]
		pleaseSayClips,
		wowClips, failClips;
	
	[Space()]
	[SerializeField] Animator anim;
	[SerializeField] GameObject[] appearParticles;
	
	[SerializeField] GeneralAudioSelector
		appearSound,
		disappearSound;

	Speech speech; // SpeechRecognizerOwn
	
	public event Action onInteraction;
	
	protected override void Awake(){
		base.Awake();
		
		speech = Speech.Instance;
	}
	
	[ContextMenu("Appear")]
	public void Appear(){
		if(gameObject.activeSelf) return;
		
		SetAppearState(true);
		appearSound.Play();
	}
	
	[ContextMenu("Disappear")]
	public void Disappear(){
		if(!gameObject.activeSelf) return;
		
		SetAppearState(false);
		disappearSound.Play();
	}
	
	void SetAppearState(bool appear){
		var particlePrefab = Tools.Random(appearParticles);
		
		var particle = Instantiate(
			particlePrefab,
			transform.position,
			particlePrefab.transform.rotation
		);
		
		Destroy(particle, 2f);
		gameObject.SetActive(appear);
	}
	
	public void OnInteraction() =>
		onInteraction?.Invoke();
}