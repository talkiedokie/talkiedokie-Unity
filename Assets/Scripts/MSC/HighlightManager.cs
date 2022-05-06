using UnityEngine;
using System.Linq;

public class HighlightManager : MonoBehaviour
{
	public GameObject[] highlightables;
	public Material highlightMaterial;
	
	public Info[] infos;
	
	void Start(){
		int count = highlightables.Length;
		infos = new Info[count];
		
		for(int i = 0; i < count; i++){
			infos[i] = new Info(
				highlightables[i],
				highlightMaterial
			);
		}
	}
	
/* 	void OnValidate(){
		if(Application.isPlaying)
			foreach(var info in infos){
				info.UpdateVisibility();
				info.UpdateThickness();
			}
	} */
	
	[System.Serializable]
	public class Info{
		[HideInInspector] public string name;
		
		public GameObject obj;
		public Renderer[] renderers;
		public Material material;
		
/* 		[Range(0,1)]
		public float
			visibility = 1f,
			thickness = 1f;
		
		float defaultThickness;
		
		const string
			VISIBILITY = "_alpha",
			THICKNESS = "_thickness"; */
		
		public Info(GameObject obj, Material material){
			this.obj = obj;
			
			renderers = obj.GetComponentsInChildren<Renderer>();
			this.material = Instantiate(material);
			
			foreach(var renderer in renderers){
				var originalMaterials = renderer.materials.ToList();
					originalMaterials.Add(this.material);
				
				renderer.materials = originalMaterials.ToArray();
			}
			
			// defaultThickness = material.GetFloat(THICKNESS);
			name = obj.name;
		}
		
/* 		public void UpdateVisibility(){
			material.SetFloat(VISIBILITY, visibility);
		}
		
		public void UpdateThickness(){
			material.SetFloat(THICKNESS, defaultThickness * thickness);
		} */
	}
}