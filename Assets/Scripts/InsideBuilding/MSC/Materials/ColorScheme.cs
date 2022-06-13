using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName ="MSC/Color Scheme")]
public class ColorScheme : ScriptableObject
{
	public Material[] materials;
	public List<Color> originalColors, editedColors;
	public List<ColorGroup> groups = new List<ColorGroup>();
	
	[Range(0,1)]
	public float hue = 0.5f;
	
	void OnValidate(){
		SetColorHue();
	}
	
	[ContextMenu("Sample Colors")]
	public void SampleColors(){
		originalColors.Clear();
		editedColors.Clear();
		groups.Clear();
		
		foreach(var material in materials){
			var color = material.color;
			
			if(originalColors.Contains(color)){
				var g = groups.Find(g => g.color == color);
					g.materials.Add(material);
			}
			else{
				originalColors.Add(color);
				editedColors.Add(color);
				
				groups.Add(new ColorGroup(color, material));
			}
		}
	}
	
	[ContextMenu("Randomize Colors")]
	public void RandomizeColors(){
		hue = Random.value;
		SetColorHue();
		
		Debug.Log("Hue Value: " + hue);
	}
	
	public void SetColorHue(){
		for(int i = 0; i < editedColors.Count; i++){
			var color = editedColors[i];
			
			Color.RGBToHSV(color, out float h, out float s, out float v);
			h += hue;
			h = h % 1f;
			
			editedColors[i] = Color.HSVToRGB(h, s, v);
		}
	}
	
	[ContextMenu("Apply Edited Colors")]
	public void ApplyEditedColors(){
		for(int i = 0; i < editedColors.Count; i++){
			groups[i].SetColor(editedColors[i]);
		}
	}
	
	[ContextMenu("Reset Colors")]
	public void ResetColors(){
		for(int i = 0; i < originalColors.Count; i++){
			groups[i].SetColor(originalColors[i]);
		}
	}
	
	[System.Serializable]
	public class ColorGroup{
		public string name;
		
		public Color color;
		public List<Material> materials;
		
		public ColorGroup(Color color, Material material){
			this.color = color;
			
			materials = new List<Material>();
			materials.Add(material);
			
			name = material.name;
		}
		
		public void SetColor(Color color){
			foreach(var material in materials)
				material.color = color;
		}
	}
}