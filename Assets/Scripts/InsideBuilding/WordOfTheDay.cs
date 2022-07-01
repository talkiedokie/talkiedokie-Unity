using UnityEngine;

[CreateAssetMenu()]
public class WordOfTheDay : ScriptableObject
{
	new public string name;
	public Sprite[] sprites;
	
	public AudioClip fairyVoiceClip;
}