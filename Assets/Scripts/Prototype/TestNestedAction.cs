using UnityEngine;

public class TestNestedAction : MonoBehaviour
{
	public Fairy fary;
	public AudioClip[] clips;
	public int i;
	
	[ContextMenu("Test")]
	public void Test(){
		i ++;
		i = i % clips.Length;
		
		fary.Speak(clips[i], 0f, Test);
		Debug.Log(clips[i].name, clips[i]);
	}
}