using UnityEngine;
using UnityEngine.Audio;

public partial class Fairy : SceneObjectSingleton<Fairy>
{
	[SerializeField] AudioSource source;
	[SerializeField] AudioClip noAudioClip;
	
	[Space()]
	[SerializeField] AudioClip[] wowClips;
	[SerializeField] AudioClip[] failClips;
	
	SpeechRecognizer speechRecognizer;
	
	void Awake(){
		speechRecognizer = SpeechRecognizer.Instance;
	}
}