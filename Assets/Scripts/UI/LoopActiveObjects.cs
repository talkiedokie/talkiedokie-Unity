using UnityEngine;
using UnityEngine.Events;

public class LoopActiveObjects : MonoBehaviour
{
	public GameObject[] objects;
	public bool loop = false;
	
	public UnityEvent<float> onUpdate;
	
	int index;
	
	void OnEnable(){
		OnUpdateCallback();
	}
	
	public void OnClick(int dir){
		int count = objects.Length;
		index += dir;
		
		if(loop){
			index = index % count;
			if(index < 0) index = count - 1;
		}
		
		else index = Mathf.Clamp(index, 0, count - 1);
		
		for(int i = 0; i < count; i++)
			objects[i].SetActive(index == i);
		
		OnUpdateCallback();
	}
	
	void OnUpdateCallback(){
		float index = (float) this.index;
		float maxIndex = (float) objects.Length - 1;
		
		float percent = index / maxIndex;
		onUpdate?.Invoke(percent);
	}
}