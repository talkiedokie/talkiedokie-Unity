using UnityEngine;

public class TestCleaning : MonoBehaviour
{
	public Renderer rend;
	public AnimationCurve lerpCurve;
	
	[Range(0,1)]
	public float amount = 1f;
	
	void Update(){
		float lerp = lerpCurve.Evaluate(amount);
		
		foreach(var mat in rend.materials)
			mat.SetFloat("_transparency", amount);
	}
}
