using UnityEngine;
using System.Collections.Generic;

public enum ShowType{ Single, OnTop }

public class UIManager : SceneObjectSingleton<UIManager>
{
	[SerializeField] GameObject transition;
	[SerializeField] Transform exclamation;
	
	[SerializeField]
	List<GameObject> current = new List<GameObject>();
	
	Camera cam;
	
	void Awake(){
		for(int i = 0; i < transform.childCount; i++){
			var child = transform.GetChild(i).gameObject;
				Hide(child);
		}
		
		cam = Camera.main;
	}
	
	public void Show(GameObject obj, ShowType showType){
		var transform = obj.transform;
		
		if(transform.parent != this.transform)
			transform.SetParent(this.transform);
		
		switch(showType){
			case ShowType.Single:{
				foreach(var c in current)
					c.SetActive(false);
				
				current.Clear();
				current.Add(obj);
				obj.SetActive(true);
				break;
			}
			
			case ShowType.OnTop:{
				current.Add(obj);
				transform.SetAsLastSibling();
				obj.SetActive(true);
				
				break;
			}
		}
		
		// CustomCursor.Instance.Show(true);
	}
	
	public void Show(GameObject obj){
		Show(obj, ShowType.Single);
	}
	
	public void Hide(GameObject obj){
		if(current.Contains(obj))
			current.Remove(obj);
		
		obj.SetActive(false);
	}
	
	public void SetExclamationPoint(Vector3 worldPosition, bool isActive){
		exclamation.gameObject.SetActive(isActive);
		
		if(isActive)
			exclamation.position = cam.WorldToScreenPoint(worldPosition);
	}
	
	public void Transition(){ transition.SetActive(true); }
}