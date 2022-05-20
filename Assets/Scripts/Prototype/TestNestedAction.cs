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
	
	[Space()]
	public Gradient gradient;
	public int division;
	
	public Color[] colors;
	
	[ContextMenu("Sample Gradient Colors")]
	public void SampleGradientColors(){
		colors = new Color[division];
		
		float maxIndex = (float) division - 1;
		
		for(int i = 0; i < division; i++)
			colors[i] = gradient.Evaluate((float) i / maxIndex);
	}
}