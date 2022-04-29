using UnityEngine;

[CreateAssetMenu()]
public class DirtyMaterials : Singleton<DirtyMaterials>
{
	[SerializeField] Material[] materials;
	
	public Material GetRandom(){
		return Tools.Random(materials);
	}
	
	public void Test(){
		Debug.Log("test");
	}
}