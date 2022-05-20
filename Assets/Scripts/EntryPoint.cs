using UnityEngine;
using System.Collections;
using Cinemachine;

public class EntryPoint : MonoBehaviour
{
	public SceneLoader cityScene;
	
	[Space()]
	public GameObject ui;
	public CinemachineVirtualCamera camPoint;
	
	[Space()]
	public Transform onRejectionPlayerDest;
	public Vector3 playerPositionCity = Vector3.zero;
	
	FpCtrl fpCtrl;
	Transform fpCam;
	
	UIManager uiMgr;
	
	IEnumerator Start(){
		uiMgr = UIManager.Instance;
		{
			var parent = ui.transform.parent;
			
			if(parent != uiMgr.transform){
				ui.transform.SetParent(uiMgr.transform);
				{					
					ui.transform.localPosition = Vector3.zero;
					ui.transform.localRotation = Quaternion.identity;
					ui.transform.localScale = Vector3.one;
				}
				
				Destroy(parent.gameObject);
			}
			
			ui.SetActive(false);
		}
		
		var gameMgr = FindObjectOfType<GameManager>();
		
		if(gameMgr){
			while(!gameMgr.hasStarted) yield return null;
		}
		
		fpCtrl = FindObjectOfType<FpCtrl>(true);
		fpCam = fpCtrl.GetComponentInChildren<CinemachineVirtualCamera>().transform;
	}
	
	void OnTriggerEnter(Collider col){
		if(!col.CompareTag("Player")) return;
		
		uiMgr.Show(ui);
		fpCtrl.enabled = false;
		
		CameraManager.Instance.SetPriority(camPoint);
		{
			camPoint.transform.position = fpCam.position;
			camPoint.transform.rotation = fpCam.rotation;
		}
	}
	
	public void OnAccept(){
		if(playerPositionCity != Vector3.zero)
			Prototype.City.playerPosition = playerPositionCity;
		
		else Prototype.City.playerPosition = onRejectionPlayerDest.position;
		
		cityScene.LoadAsync();
	}
	
	public void OnReject(){
		StopAllCoroutines();
		StartCoroutine(Reject());
	}
	
	IEnumerator Reject(){
		uiMgr.Hide(ui);
		
		var transform = fpCtrl.transform;
		var startPos = transform.position;
		var destPos = onRejectionPlayerDest.position;
		
		float timer = 0f;
		float duration = 0.5f;
		
		while(timer < duration){
			transform.position = Vector3.Lerp(startPos, destPos, timer / duration);
			timer += Time.deltaTime;
			yield return null;
		}
		
		fpCtrl.enabled = true;
	}
}