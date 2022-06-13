using UnityEngine;

public class RandomMaterial : MonoBehaviour
{
	[LabelOverride("Renderer")]
	public Renderer rend;
	public Material[] materials;
	
	void OnEnable(){
		if(Application.isPlaying)
			rend.material = Instantiate(Tools.Random(materials));
	}
}