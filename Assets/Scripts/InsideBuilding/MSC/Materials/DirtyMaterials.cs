using UnityEngine;

[CreateAssetMenu(menuName = "MSC/Dirty Materials")]
public class DirtyMaterials : Singleton<DirtyMaterials>
{
	[SerializeField] Material[] materials;
	
	public Material GetRandom(){
		return Instantiate(Tools.Random(materials));
	}
}