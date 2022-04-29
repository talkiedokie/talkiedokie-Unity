using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SpriteSheetAnimationUI : MonoBehaviour
{
	public Sprite[] sprites;
	public float sampleRate = 12f;
	
	float timer;
	int i;
	
	Image image;
	
	void Start(){
		image = GetComponent<Image>();
	}
	
	#if UNITY_EDITOR
		
		void OnValidate(){
			if(!image)
				image = image = GetComponent<Image>();
			
			if(sprites.Length > 0)
				image.sprite = sprites[0];
		}
		
	#endif
	
	void LateUpdate(){
		float step = 1 / sampleRate;
		timer += Time.deltaTime;
		
		if(timer >= step){
			image.sprite = sprites[i];
			
			i ++;
			i = i % sprites.Length;
			
			timer = 0f;
		}
	}
}