using UnityEngine;
using UnityEngine.UI;
using System;
using Gameplay;

namespace InsideBuilding
{
	public partial class GameManager : SceneObjectSingleton<GameManager>
	{
		#region Inspector
		
			[Foldout("UI")]
			[Header("Start Load")]
			[SerializeField, LabelOverride("Reference")] GameObject startLoadUI;
			[SerializeField, LabelOverride("Progress Img")] Image startProgressImg;
			[SerializeField, LabelOverride("Progress Txt")] Text startProgressTxt;
			
			[Header("Word Popup")]
			[SerializeField, LabelOverride("Reference")] GameObject wordPopup;
			[SerializeField, LabelOverride("Text")] Text wordPopupTxt;
			[SerializeField, LabelOverride("Icon")] Image wordPopupIcon;
			[SerializeField, LabelOverride("Check")] GameObject wordPopupCheck;
			
			[Space()]
			[SerializeField] GameObject menu;
			
			[Foldout("Audio Clips")]
			[SerializeField, LabelOverride("Where do you want to go?")] AudioClip wdywtg_Clip;
			[SerializeField, LabelOverride("Say Clip")] AudioClip say_Clip;
			[SerializeField, LabelOverride("You can now enjoy your free time!")] AudioClip ycneyft_Clip;
			
			[SerializeField] GeneralAudioSelector
				wordPopupSound,
				correctWordSound;
			
		#endregion
		
		#region Variables
		
			// References required in changing game modes
				Transform playerDefaultPoint; // remember default point where the player spawns, just in case there's no room selected we go to here
				FirstPersonController fpCtrl;
			
			Fairy fairy;
			Speech speech;
			
			UIManager uiMgr;
			const ShowType single = ShowType.Single; // UIManager
			
			// Camera cam;
			CameraManager camMgr;
			
			public GameObject player{ get; private set; }
			
		#endregion

		#region Unity Updates
			
			void LateUpdate(){
				if(!hasStarted) return;
				
				if(Input.GetButtonDown("Cancel")){
					if(fairy.isSpeaking)
						fairy.StopSpeaking();
					
					else if(fairy.isListening)
						fairy.StopListening();
					
					else ToggleMenuUI();
				}
			}
			
			void ToggleMenuUI(){
				if(menu.activeSelf) uiMgr.Hide(menu);
				else uiMgr.Show(menu, single);
			}
			
			void OnDestroy(){
				// SpeechRecognizer.OnDestroyCall();
				// AudioRecorder.OnDestroyCall();
			}
			
		#endregion
		
		#region Utilities
			
			public void SetOrientation(Transform transform, Transform target){
				transform.SetParent(target);
				transform.SetPositionAndRotation(target.position, target.rotation);
			}
			
			void WordPopup_Show(string text, Sprite sprite){
				wordPopupTxt.text = text;
				wordPopupIcon.sprite = sprite;
				
				wordPopupIcon.gameObject.SetActive(sprite);
				
				wordPopupSound.Play();
				wordPopup.SetActive(true);
			}
			
			void WordPopup_Correct(){
				correctWordSound.Play();
				
				wordPopupCheck.SetActive(true);
				Invoke(nameof(HideWordPopupUI), 2f);
			}
			
			void HideWordPopupUI(){
				wordPopupSound.Play();
				
				wordPopupCheck.SetActive(false);
				wordPopup.SetActive(false);
			}
			
		#endregion
		
		#region Utility and Events
			
			public Action
				onRoomEntered,
				
				onTaskSelected,
				onTaskFinished,
				
				onLevelFinished;
			
			public Action<GameMode> onGameModeChanged;
			
			void OnRoomEntered() => onRoomEntered?.Invoke();
			void OnTaskFinished() => onTaskFinished?.Invoke();
			void OnLevelFinished() => onLevelFinished?.Invoke();
			
		#endregion
	}
}