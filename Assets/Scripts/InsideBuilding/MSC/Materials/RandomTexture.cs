using UnityEngine;

public class RandomTexture : MonoBehaviour
{
	public Renderer rend;
	public Texture[] textures;
	
	[ContextMenu("Test")]
	public void OnEnable(){
		if(Application.isPlaying)
			rend.material.SetTexture(
				"_BaseMap",
				Tools.Random(textures)
			);
		
		else
			rend.sharedMaterial.SetTexture(
				"_BaseMap",
				Tools.Random(textures)
			);
	}
}