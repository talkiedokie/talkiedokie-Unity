using UnityEngine;
using System.Collections;
using Cinemachine;

namespace InsideBuilding
{
	public class EntryPoint : MonoBehaviour
	{
		#region Inspector
			
			[SerializeField, TextArea()] string description = "";
			[SerializeField] SceneLoader scene;
			
			[Space()]
			[SerializeField] bool updateLevel;
			[SerializeField] SceneLoader levelScene;
			
			[Space()]
			[SerializeField] EntryPointUI ui;
			[SerializeField] CinemachineVirtualCamera camPoint;
			
			[Space()]
			[SerializeField] Transform onRejectionPlayerDest;
			[SerializeField] Vector3 playerPositionCity = Vector3.zero;
			
			[Space()]
			[SerializeField] GeneralAudioSelector onEnterSound;
			[SerializeField] GeneralAudioSelector onRejectSound;
			
		#endregion		
		
		#region Variables
		
			FirstPersonController fpCtrl;
			Transform fpCam;
			bool fpCtrlAutoResetTransform;
			
			UIManager uiMgr;
			bool hasStarted;
			
		#endregion
		
		#region Initializations
			
			IEnumerator Start(){
				SetupUI();
				
				yield return WaitForGameManager();
				
				GetPlayerDependencies();
				hasStarted = true;
			}
			
			void SetupUI(){
				uiMgr = UIManager.Instance;
				
				var ui = Instantiate(this.ui, uiMgr.transform, false);
					
					ui.onAccept = OnAccept;
					ui.onReject = OnReject;
					
					ui.SetDescription(description);
				
				this.ui = ui;
				this.ui.SetActive(false);
			}
			
			IEnumerator WaitForGameManager(){
				var gameMgr = FindObjectOfType<GameManager>();
				
				if(gameMgr)
					while(!gameMgr.hasStarted) yield return null;
			}
			
			void GetPlayerDependencies(){
				fpCtrl = FindObjectOfType<FirstPersonController>(true);
				fpCam = fpCtrl.GetComponentInChildren<CinemachineVirtualCamera>().transform;
			}
			
		#endregion
		
		void OnTriggerEnter(Collider col){
			if(!hasStarted) return;
			if(!col.CompareTag("Player")) return;
			
			uiMgr.Show(ui.gameObject);

            fpCtrl.enabled = false;
			
			CameraManager.Instance.SetPriority(camPoint);
			{
				camPoint.transform.position = fpCam.position;
				camPoint.transform.rotation = fpCam.rotation;
			}
			
			onEnterSound.Play();
		}
		
		#region UI Button Calls
			
			public void OnAccept(){
				SetPlayerOrientationInCity();
				
				if(updateLevel)
					GameManager.LevelScene = levelScene;

                scene.LoadAsync();
			}
			
			public void OnReject(){
				StopAllCoroutines();
				StartCoroutine(Reject());
				
				onRejectSound.Play();
			}
			
			void SetPlayerOrientationInCity(){
				if(playerPositionCity != Vector3.zero)
					City.playerPosition = playerPositionCity;
				
				else City.playerPosition = onRejectionPlayerDest.position;
				
				City.SetPlayerRotation(onRejectionPlayerDest.rotation);
			}
			
			IEnumerator Reject(){
				uiMgr.Hide(ui.gameObject);
                var transform = fpCtrl.transform;
				var startPos = transform.position;
				var destPos = onRejectionPlayerDest.position;
				
				float timer = 0f;
				float duration = 0.5f;

				CameraManager.Instance.SetPriority(fpCtrl.GetComponentInChildren<CinemachineVirtualCamera>());

				while (timer < duration)
				{
						transform.position = Vector3.Lerp(startPos, destPos, timer / duration);
						timer += Time.deltaTime;
						yield return null;
				}
				
					fpCtrl.enabled = true;
				
				}
			
		#endregion
	}
}