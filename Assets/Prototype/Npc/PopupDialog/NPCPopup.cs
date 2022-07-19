using UnityEngine;
using UnityEngine.Animations.Rigging;

public class NPCPopup : MonoBehaviour
{
	[SerializeField] NPCPopupUI uiPrefab;
	[SerializeField, TextArea()] string[] messages;
	
	[Space()]
	[SerializeField] bool updateRig;
	[SerializeField] Rig rig;
	[SerializeField] Transform rigTarget;
	
	bool isInteracting;
	static Transform cameraT;
	float weightSmoothVel;
	
	const string PLY = "Player";
	
	static NPCPopupUI ui;
	
	NPCPopupUI UI{
		get{
			if(!ui)
				ui = Instantiate(uiPrefab);
			
			return ui;
		}
	}
	
	void OnTriggerEnter(Collider col)
	{
		if(!col.CompareTag(PLY)) return;
		if(messages == null) return;
		if(messages.Length == 0) return;
		
		float angle = Vector3.Angle(transform.forward, -col.transform.forward);
		if(angle > 90) return;
		
		string message = Tools.Random(messages);
		
		UI.Show(message, this);
		isInteracting = true;
	}
	
	void OnTriggerExit(Collider col)
	{
		if(!isInteracting) return;
		if(!col.CompareTag(PLY)) return;
		
		if(UI.currentUser == this)
			UI.Hide();
	
		isInteracting = false;
	}
	
	void Update()
	{
		if(!updateRig) return;
		
		if(isInteracting)
		{
			if(!cameraT)
				cameraT = Camera.main.transform;
			
			rigTarget.position = cameraT.position;
		}
		
		float targetWeight = isInteracting? 1f: 0f;
		float smoothTime = 0.15f;
		
			rig.weight = Mathf.SmoothDamp(rig.weight, targetWeight, ref weightSmoothVel, smoothTime);
	}
}