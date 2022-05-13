using UnityEngine;
using UnityEngine.Events;

public class Clickable : MonoBehaviour
{
	[SerializeField] Animator _animator;
	[SerializeField] string _param = "clicked";
	int param;
	
	[SerializeField] GameObject[] highlights;
	[SerializeField] UnityEvent<bool> onClick;
	
	public bool clicked{ get; private set; }
	
	void Awake(){
		param = Animator.StringToHash(_param);
	}
	
	void OnMouseDown(){
		Interact();
	}
	
	public void ShowHighlight(bool b){
		foreach(var highlight in highlights)
			highlight.SetActive(b);
	}
	
	public void Interact(){
		clicked = !clicked;
		
		if(_animator)
			_animator.SetBool(param, clicked);
		
		onClick?.Invoke(clicked);
	}
}