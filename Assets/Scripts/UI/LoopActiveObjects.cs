using UnityEngine;

public class LoopActiveObjects : MonoBehaviour
{
	public GameObject[] objects;
	public bool loop = false;
	
	int index;
	
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
	}
}