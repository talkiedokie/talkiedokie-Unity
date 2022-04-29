using UnityEngine;

public class CustomCursor : SceneObjectSingleton<CustomCursor>
{
	void Start(){ Show(true); }
	
	void LateUpdate(){
		transform.position = Input.mousePosition;
	}
	
	public void Show(bool b){
		gameObject.SetActive(b);
		Lock(!b);
	}
	
	public void Hide(bool b){
		gameObject.SetActive(!b);
		Lock(b);
	}
	
	void Lock(bool b){
		if(b) Cursor.lockState = CursorLockMode.Locked;
		else Cursor.lockState = CursorLockMode.None;
		
		Cursor.visible = false;
	}
}