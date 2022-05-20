using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using AccountsManagement;

public partial class GameManager : SceneObjectSingleton<GameManager>
{
	#region Inspector
		
		[Foldout("Level")]
		
			[SerializeField] SceneLoader levelScene;
			
			[Space()]
			[SerializeField] UserSelector user;
			[SerializeField] Text usernameTxt;
			
			[Space()]
			[SerializeField] GameObject[] characterPrefabs;
		
		[Foldout("UI")]
		
			[SerializeField] GameObject roomSelector;
			[SerializeField] RoomButton roomButtonTemplate;
			
			[Space()]
			[SerializeField] GameObject wordPopup;
			[SerializeField] Text wordPopupTxt;
			[SerializeField] Image wordPopupIcon;
		
		[Foldout("Tasks")]
		
			[SerializeField, LabelOverride("UI Template")] RoomTasksUI roomTasksUITemplate;
			RoomTasksUI[] roomTasksUI;
			
			[SerializeField, LabelOverride("Exit Duration")] float onTaskExitDuration = 2.25f;
			[SerializeField] GeneralAudioSelector onTaskFinishSound = 4;
			
			[Space()]
			[SerializeField] GameObject[] onTaskFinishParticles;
			[SerializeField] GameObject coinsParticles;
		
		[Foldout("Word of the Day")]
			
			[SerializeField] WordOfTheDay[] wordOfTheDays;
			[SerializeField, LabelOverride("Clip Delay")] float wotd_ClipDelay = 0.45f;
			
			[Space()]
			[SerializeField, LabelOverride("The word of the day is...")] AudioClip wotd_Clip1;
			[SerializeField, LabelOverride("I want you to say the word...")] AudioClip wotd_Clip2;
			
			WordOfTheDay wordOfTheDay;
		
		[Foldout("Audio Clips")]
			
			[Space()]
			[SerializeField, LabelOverride("Where do you want to go?")] AudioClip wdywtg_Clip;
			[SerializeField, LabelOverride("Clip Delay")] float wdywtg_ClipDelay = 1f;
			
			[Space()]
			[SerializeField, LabelOverride("Let's play the word")] AudioClip lptw_Clip;
			[SerializeField, LabelOverride("Clip Delay")] float awsm_ClipDelay = 1f;
			
			[Space()]
			[SerializeField, LabelOverride("You can now enjoy your free time!")] AudioClip ycneyft_Clip;
			[SerializeField, LabelOverride("Clip Delay")] float ycneyft_ClipDelay = 1f;
		
		[Foldout("Other")]
		
			[SerializeField] WindowsSpeechErrorHandler winSpeechError;
			
			[Space()]
			[SerializeField] GameObject startLoadUI;
			[SerializeField] Image startProgressImg;
			[SerializeField] Text startProgressTxt;
			
			[Space()]
			[SerializeField] GameObject menu;
			[SerializeField] GeneralAudioSelector transitionSound = 9;
			[SerializeField] SceneLoader loginScene;
		
	#endregion
	
	#region Variables
		
		Level level;
		AccountManager accMgr;
		
		Room selectedRoom;
		Task currentTask;
		
		Fairy fairy;
		
		UIManager uiMgr;
		const ShowType single = ShowType.Single; // UIManager
		
		Camera cam;
		CameraManager camMgr;
		
	#endregion
	
	#region Properties
		
		public User UserData{ get; private set; }
		public Gender PlayerGender => UserData.gender;
		
		public GameObject player{ get; private set; }
		
		public Room SelectedRoom{
			get{
				if(!selectedRoom)
					selectedRoom = level.rooms[selectedRoom_Index];
				
				return selectedRoom;
			}
			
			set{ selectedRoom = value; }
		}

		int selectedRoom_Index{
			get{ return level.selectedRoomIndex; }
			set{ level.selectedRoomIndex = value; }
		}
		
		GameObject onTaskFinishParticle => Tools.Random(onTaskFinishParticles);
		
	#endregion
	
	#region Unity Methods
		
		// public Transform cursor;
		
		void LateUpdate(){
			if(!hasStarted) return;
			
			if(Input.GetButtonDown("Cancel")){
				if(fairy.isSpeaking)
					fairy.StopSpeaking();
				
				else if(fairy.isListening)
					fairy.StopListening();
				
				else{
					if(menu.activeSelf) uiMgr.Hide(menu);
					else uiMgr.Show(menu, single);
				}
			}
			
			// cursor.position = Input.mousePosition;
		}
		
	#endregion
	
	#region Calls
		
		#region Main
			
			[ContextMenu("Select Rooms")]
			public void SelectRoom(){
				fairy.Speak(wdywtg_Clip, wdywtg_ClipDelay, ShowRoomSelectUI);
				
				void ShowRoomSelectUI(){
					uiMgr.Show(roomSelector, single);
				}
			}
			
			[ContextMenu("Select Tasks")]
			public void SelectTasks(){			
				if(!selectedRoom){
					Debug.LogWarning("There's no room selected");
					SelectRoom();
					return;
				}
				
				if(!SelectedRoom.HasTasks){
					Debug.LogWarning(SelectedRoom.name + " has no tasks.", SelectedRoom);
					FinishTask();
					return;
				}
				
				var uiPopup = roomTasksUI[selectedRoom_Index].gameObject;
					uiMgr.Show(uiPopup, single);			
			}
			
			[ContextMenu("Change the Word of the Day")]
			public void ChangeTheWordOfTheDay(){
				wordOfTheDay = Tools.Random(wordOfTheDays);
				Debug.Log(wordOfTheDay.name);
			}
			
			[ContextMenu("Word of the Day")]
			public void WordOfTheDay(){
				string word = wordOfTheDay.name;
				var sprite = wordOfTheDay.sprite;
				
					wordPopupTxt.text = sprite? word:
						"Word of the Day \n" + "'" + word + "'";
					
					wordPopupIcon.gameObject.SetActive(sprite);
					wordPopupIcon.sprite = sprite;
					wordPopup.SetActive(true);
				
				var fairyVoiceClip = wordOfTheDay.fairyVoiceClip;
				
				var clips = new AudioClip[]{
					wotd_Clip1,
					fairyVoiceClip,
					wotd_Clip2,
					fairyVoiceClip
				};
				
					fairy.Speak(clips, wotd_ClipDelay, SpeechListen);
				
				void SpeechListen(){
					fairy.ListenToSpeech(word, OnListenFinish);
					
					void OnListenFinish(){
						wordPopup.SetActive(false);
						
						fairy.Speak(
							ycneyft_Clip,
							ycneyft_ClipDelay,
							OnLevelFinished
						);
					}
				}
			}
			
		#endregion
		
		#region UI Input
		
			public void SelectRoom(int index){
				if(selectedRoom && index == selectedRoom_Index){
					Debug.LogWarning("You're in the " + selectedRoom.name + " already!", selectedRoom);
					return;
				}
				
				SelectedRoom = level.rooms[index];
				selectedRoom_Index = index;
				
				wordPopupTxt.text = SelectedRoom.name;
				wordPopupIcon.gameObject.SetActive(false);
				
				wordPopup.SetActive(true);
				
				fairy.Speak(SelectedRoom.RoomNameFairyVoiceClip, 1f, SpeechListen);
				uiMgr.Hide(roomSelector);
				
				void SpeechListen(){
					fairy.ListenToSpeech(SelectedRoom.name, GoToTheRoom, OnRoomEntered);
					
					void GoToTheRoom(){
						wordPopup.SetActive(false);
						
						var charPoint = SelectedRoom.CharacterPoint;
					
						SetOrientation(fairy.transform, charPoint);
						SetOrientation(player.transform, charPoint);
						camMgr.SetPriority(selectedRoom.CameraPoint);
						
						uiMgr.Transition();
						transitionSound.Play();
						SelectedRoom.OnEnter(); // play cam animation (ease-in)
					}
				}
			}
			
			public void OnTaskSelected(int index){
				var task = selectedRoom.tasks[index];
				fairy.Speak(task.fairyVoiceClip, 0.15f, PlayTask);
				
				void PlayTask(){
					currentTask = SelectedRoom.InstantiateTask(index);
					if(!currentTask) return;
				
					currentTask.gameObject.SetActive(true);
					currentTask.Play(true);
					
					// uiMgr.Transition();
					uiMgr.Hide(roomTasksUI[selectedRoom_Index].gameObject);
					
					fairy.gameObject.SetActive(false);
					player.SetActive(false);
		
					Debug.Log(currentTask.description);
				}
			}
			
			public void FinishTask(){
				StartCoroutine(routine());
				
				IEnumerator routine(){
					onTaskFinishParticle.SetActive(true);
					onTaskFinishSound.Play();
					
					yield return new WaitForSeconds(1f);
					
						var coinHUD = CoinHUD.Instance;
						{
							coinHUD.AddCoin(currentTask.rewards);
							while(coinHUD.isUpdatingAmount) yield return null;
						}
						
						coinsParticles.SetActive(true);
					
					yield return new WaitForSeconds(onTaskExitDuration);
					
					uiMgr.Transition();
					transitionSound.Play();
					
					camMgr.SetPriority(selectedRoom.CameraPoint);
					player.SetActive(true);
					
					fairy.gameObject.SetActive(true);
					{
						fairy.SayWow(0.5f, onFairyWow);
						
						void onFairyWow(){
							fairy.Speak(
								lptw_Clip,
								awsm_ClipDelay,
								OnTaskFinished
							);
						}
					}
					
					if(currentTask){
						currentTask.isFinished = true;
						Destroy(currentTask.gameObject);
					}
					
					yield return new WaitForSeconds(1);
					
					foreach(var particle in onTaskFinishParticles)
						particle.SetActive(false);
					
					coinsParticles.SetActive(false);
				}
				
				uiMgr.SetExclamationPoint(Vector3.zero, false);
			}
			
		#endregion
		
	#endregion
	
	#region Utils
		
		void SetOrientation(Transform transform, Transform target){
			var position = transform.localPosition;
			var rotation = transform.localRotation;
			
			transform.SetParent(target);
			
			transform.localPosition = position;
			transform.localRotation = rotation;
		}
		
		public void RestartGame(){
			SceneLoader.Current();
		}
		
		public void ExitGame(){
			accMgr?.Serialize();
			Application.Quit();
		}
		
		public void SwitchAccount(){
			accMgr.Serialize();
			loginScene.Load();
		}
		
		public string confirmPassword{ get; set; }
		public void DeleteAccount(){
			if(confirmPassword != UserData.password) return;
			
			accMgr.RemoveUser(user);
			accMgr.Serialize();
			
			loginScene.Load();
		}
		
	#endregion
	
	#region Events
		
		public delegate void Callback();
		public Callback onRoomEntered, onTaskFinished, onLevelFinished;
		
		void OnRoomEntered(){ onRoomEntered?.Invoke(); }
		void OnTaskFinished(){ onTaskFinished?.Invoke(); }
		void OnLevelFinished(){ onLevelFinished?.Invoke(); }
		
	#endregion
}