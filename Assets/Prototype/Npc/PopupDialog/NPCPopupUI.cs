using UnityEngine;
using UnityEngine.UI;

public class NPCPopupUI : MonoBehaviour
{
	[SerializeField] Text txt;
	[SerializeField] Animator anim;
	
	[SerializeField] float position = 2.1f;
	
	[SerializeField] GeneralAudioSelector
		showSound,
		hideSound;
	
	public NPCPopup currentUser
	{
		get;
		private set;
	}
	
	Transform cam, _transform;
	
	void Awake()
	{
		_transform = transform;
		cam = Camera.main.transform;
	}
	
	public void Show(string message, NPCPopup current)
	{
		currentUser = current;
			_transform.SetParent(current.transform);
			_transform.localPosition = Vector3.up * position;
		
		txt.text = message;
		gameObject.SetActive(true);
		
		showSound.Play();
	}
	
	[ContextMenu("Hide")]
	public void Hide()
	{
		anim.SetTrigger("hide");
		hideSound.Play();
		
		currentUser = null;
	}
	
	void LateUpdate()
	{
		var	direction = cam.position - _transform.position;
			transform.forward = -direction;
	}
}