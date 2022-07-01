using UnityEngine;

public class HideObject : MonoBehaviour
{
	public GameObject[] objects;
	
	void OnEnable(){
		foreach(var obj in objects) obj.SetActive(false);
	}

	void OnDisable(){
		foreach(var obj in objects) obj.SetActive(true);
	}
}