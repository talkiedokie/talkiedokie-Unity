using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCleaning : MonoBehaviour
{
	public Renderer rend;
	
	[Range(0,1)]
	public float amount = 1f;
	
	void Update(){
		foreach(var mat in rend.materials)
			mat.SetFloat("_transparency", amount);
	}
}
