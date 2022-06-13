using UnityEngine;

public class RandomizeColorScheme : MonoBehaviour
{
	public ColorScheme colorScheme;
	public bool randomize;
	
	void Start(){
		if(randomize) colorScheme.RandomizeColors();
		else colorScheme.SetColorHue();
		
		colorScheme.ApplyEditedColors();
	}
}