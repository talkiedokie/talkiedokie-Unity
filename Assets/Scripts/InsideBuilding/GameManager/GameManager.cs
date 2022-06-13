using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using AccountsManagement;

// Initialization
// Start
// Rooms
// Tasks
// GameModes
// Word of the day
// utils
// events

namespace InsideBuilding
{
	public enum GameMode{ StepByStep, Free }
	
	public partial class GameManager : SceneObjectSingleton<GameManager>
	{
		#region Inspector
			
			[SerializeField] GameMode gameMode;
			
			[Foldout("Level")]
			
				[SerializeField] SceneLoader levelScene;
				public static SceneLoader LevelScene;
				
				[SerializeField] UserSelector user;
				[SerializeField] Text usernameTxt;
				
				[Space()]
				[SerializeField] GameObject[] characterPrefabs;
			
			[Foldout("UI")]
			
				[SerializeField] GameObject roomSelector;
				[SerializeField] RoomButton roomButtonTemplate;
				RoomButton[] roomButtons;
				
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
				[SerializeField, LabelOverride("Delay")] float wotd_ClipDelay = 0.45f;
				
				[Space()]
				[SerializeField, LabelOverride("The word of the day is...")] AudioClip wotd_Clip1;
				[SerializeField, LabelOverride("I want you to say the word...")] AudioClip wotd_Clip2;
				
				WordOfTheDay wordOfTheDay;
			
			[Foldout("Audio Clips")]
				
				[Space()]
				[SerializeField, LabelOverride("Where do you want to go?")] AudioClip wdywtg_Clip;
				[SerializeField, LabelOverride("Delay")] float wdywtg_ClipDelay = 1f;
				
				[Space()]
				[SerializeField, LabelOverride("Say Clip")] AudioClip say_Clip;
				[SerializeField, LabelOverride("Delay")] float say_ClipDelay = 0.2f;
				
				[Space()]
				[SerializeField, LabelOverride("You can now enjoy your free time!")] AudioClip ycneyft_Clip;
				[SerializeField, LabelOverride("Delay")] float ycneyft_ClipDelay = 1f;
			
			[Foldout("Other")]
			
				[SerializeField] WindowsSpeechErrorHandler winSpeechError;
				
				[Space()]
				[SerializeField] GameObject startLoadUI;
				[SerializeField] Image startProgressImg;
				[SerializeField] Text startProgressTxt;
				
				[Space()]
				[SerializeField] GameObject menu;
				[SerializeField] Dropdown gameModeDrp;
				
				[Space()]
				[SerializeField] GeneralAudioSelector transitionSound = 9;
				[SerializeField] SceneLoader loginScene;
			
		#endregion
		
		#region Variables
			
			Level level;
			AccountManager accMgr;
			
			[Foldout("Debugging")]
			[SerializeField] Room selectedRoom;
			[SerializeField] Task currentTask;
			
			Transform playerDefaultPoint;
			FpCtrl fpCtrl;
			
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
			public bool useSpeechOnRoomSelect{ get; set; } = true;
			
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
			
			public Task CurrentTask => currentTask;
			
			GameObject onTaskFinishParticle => Tools.Random(onTaskFinishParticles);
			
		#endregion
		
		#region Unity Methods
			
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
			}
			
		#endregion
	
		#region General Calls
			
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
		
		#region Specific Calls
		
			public void SetGameMode(GameMode mode){
				gameMode = mode;
				
				if(!fpCtrl)
					fpCtrl = player.GetComponent<FpCtrl>();
				
				switch(gameMode){
					case GameMode.StepByStep:
						fpCtrl.enabled = false;
						var playerT = player.transform;
						
						var charPoint = selectedRoom? selectedRoom.playerPoint: playerDefaultPoint;
							SetOrientation(playerT, charPoint);
						
						fairy.Appear();
					
					break;
					
					case GameMode.Free:
						fairy.Disappear();
						fpCtrl.enabled = true;
				
					break;
				}
				
				gameModeDrp.SetValueWithoutNotify((int) mode);
				onGameModeChanged?.Invoke(mode);
			}
			
			public void SetGameMode(int index){
				SetGameMode((GameMode) index);
			}
		
			public void OnRoomSelected(int index){
				if(selectedRoom && index == selectedRoom_Index){
					Debug.LogWarning("You're in the " + selectedRoom.name + " already!", selectedRoom);
					return;
				}
				
				// Refs
					SelectedRoom = level.rooms[index];
					selectedRoom_Index = index;
				
				// Events
					if(useSpeechOnRoomSelect){
						// UI
							wordPopupTxt.text = SelectedRoom.name;
							wordPopupIcon.gameObject.SetActive(false);
							
							wordPopup.SetActive(true);
						
						var clips = new AudioClip[]{ say_Clip, SelectedRoom.RoomNameFairyVoiceClip };
						fairy.Speak(clips, say_ClipDelay, SpeechListen);
					}
					
					else{
						GoToTheRoom();
						OnRoomEntered();
					}
					
				
				void SpeechListen(){
					fairy.ListenToSpeech(SelectedRoom.name, GoToTheRoom, OnRoomEntered);
				}
				
				void GoToTheRoom(){
					wordPopup.SetActive(false);
					
					SetOrientation(fairy.transform, SelectedRoom.fairyPoint);
					SetOrientation(player.transform, SelectedRoom.playerPoint);
					
					camMgr.SetPriority(selectedRoom.CameraPoint);
					
					uiMgr.Transition();
					transitionSound.Play();
					SelectedRoom.OnEnter(); // play cam animation (ease-in)
				}
				
				// UI
					for(int i = 0; i < roomButtons.Length; i++)
						roomButtons[i].IsInteractable(i != selectedRoom_Index);
						
					uiMgr.Hide(roomSelector);
			}
			
			public void OnTaskSelected(int index){
				var task = selectedRoom.tasks[index];
				var taskDescription = task.description;
				
					roomTasksUI[selectedRoom_Index].OnTaskSelected(index);
					fairy.Speak(taskDescription.fairyVoiceClip, 0.15f, PlayTask);
				
				void PlayTask(){
					currentTask = SelectedRoom.InstantiateTask(index);
					if(!currentTask) return;
				
					selectedRoom.ShowCharacters(false);
					
					currentTask.gameObject.SetActive(true);
					currentTask.Play(true);
					
					uiMgr.Hide(roomTasksUI[selectedRoom_Index].gameObject);
					
					Debug.Log(taskDescription.text);
					onTaskSelected?.Invoke();
				}
			}
			
			public void CancelTask(){
				currentTask?.CancelTask();
			}
		
			public void FinishTask(){
				StartCoroutine(routine());
				
				IEnumerator routine(){
					bool isCompleted = !currentTask.isCanceled;
					
					// Rewards
						if(isCompleted){
							onTaskFinishParticle.SetActive(true);
							onTaskFinishSound.Play();
							
							yield return new WaitForSeconds(1f);
							
								var coinHUD = CoinHUD.Instance;
								{
									coinHUD.AddCoin(currentTask.rewards);
									while(coinHUD.isUpdatingAmount) yield return null;
								}
								
								coinsParticles.SetActive(true);
						}
					
					yield return new WaitForSeconds(onTaskExitDuration);
					
					// Reset
						uiMgr.Transition();
						transitionSound.Play();
						
						camMgr.SetPriority(selectedRoom.CameraPoint);
						
						currentTask.ResetPlayerPosition();
						selectedRoom.ShowCharacters(true);
					
					// Proceed
						if(isCompleted) fairy.SayWow(0.5f, OnTaskFinished);
						else fairy.SayFail(0.5f, OnTaskFinished);
					
					// Cleanup
						if(currentTask)
							Destroy(currentTask.gameObject);
						
						yield return new WaitForSeconds(1);
						
						foreach(var particle in onTaskFinishParticles)
								particle.SetActive(false);
						
						coinsParticles.SetActive(false);
				}
				
				uiMgr.SetExclamationPoint(Vector3.zero, false);
			}
			
		#endregion
		
		#region Utils
			
			public void SetOrientation(Transform transform, Transform target){
				transform.SetParent(target);
				
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
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
		
		#region Callbacks
			
			public Action
				onRoomEntered,
				
				onTaskSelected,
				onTaskFinished,
				
				onLevelFinished;
			
			public Action<GameMode> onGameModeChanged;
			
			void OnRoomEntered(){ onRoomEntered?.Invoke(); }
			void OnTaskFinished(){ onTaskFinished?.Invoke(); }
			void OnLevelFinished(){ onLevelFinished?.Invoke(); }
			
		#endregion
	}
}