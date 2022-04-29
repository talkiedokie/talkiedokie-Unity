using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum Gender{ Male, Female }

public class GameManager : SceneObjectSingleton<GameManager>
{
	#region Inspector
	
		public Level level;
		
		[SerializeField] Gender playerGender;
		[SerializeField] GameObject[] characterPrefabs;
		
		[Space()]
		[SerializeField] GameObject roomSelector;
		[SerializeField] RoomButton roomButtonTemplate;
		
		[Space()]
		[SerializeField] GameObject wordPopup;
		[SerializeField] Text wordPopupTxt;
		[SerializeField] Image wordPopupIcon;
		
		[Space()]
		[SerializeField] RoomTasksUI roomTasksUITemplate;
		RoomTasksUI[] roomTasksUI;
		
		[Space()]
		[SerializeField] WordOfTheDay[] wordOfTheDays;
		[SerializeField] float wotd_ClipDelay = 0.45f;
		
		WordOfTheDay wordOfTheDay;
		
		[Space()]
		[SerializeField] GameObject menu;
		public GameObject particle;
		
		[HideInInspector] public AudioClip
			wdywtg_Clip,
			wgj_Clip,
			lptw_Clip,
			ycneyft_Clip,
			wotd_Clip1,
			wotd_Clip2;
		
		[HideInInspector] public float
			wdywtg_ClipDelay = 1f,
			awsm_ClipDelay = 1f,
			ycneyft_ClipDelay = 1f;
		
		[Space()]
		[SerializeField] AudioClip[] wowClips;
		[SerializeField] float wowClipsDelay = 1f;
		
	#endregion
	
	#region Variables
		
		Room selectedRoom;
		Task currentTask;
		
		Fairy fairy;
		Camera cam;
		
		const ShowType single = ShowType.Single;
		
		UIManager uiMgr;
		SpeechRecognizer speechRecognizer;
		
		CameraManager camMgr;
		
	#endregion
	
	#region Properties
		
		public Gender PlayerGender => playerGender;
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
		
	#endregion
	
	#region Unity Methods
		
		void Awake(){
			uiMgr = UIManager.Instance;
			speechRecognizer = SpeechRecognizer.Instance;
			
			fairy = Fairy.Instance;
			
			playerGender = (Gender) Random.Range(0,2); // temporary
			
			var playerPlaceHolder = GameObject.FindWithTag("Player").transform;
			
			player = Instantiate(
				characterPrefabs[(int) playerGender],
				playerPlaceHolder.position,
				playerPlaceHolder.rotation,
				playerPlaceHolder.parent
			);
			
			Destroy(playerPlaceHolder.gameObject);
			
			cam = Camera.main;
			camMgr = CameraManager.Instance;
		}
		
		void Start(){
			level = Instantiate(level);
			
			// Setup UI
			RoomButton.gameMgr = this;
			
			int count = level.rooms.Length;
			roomTasksUI = new RoomTasksUI[count];
			
			for(int i = 0; i < count; i++){
				var room = level.rooms[i];
				
				roomButtonTemplate.CreateInstance(room, i);
				roomTasksUI[i] = roomTasksUITemplate.CreateInstance(room, i);
			}
			
			Destroy(roomButtonTemplate.gameObject);
			Destroy(roomTasksUITemplate.gameObject);
			
			ChangeTheWordOfTheDay();
		}
		
		void LateUpdate(){
			if(Input.GetButtonDown("Cancel")){
				if(menu.activeSelf) uiMgr.Hide(menu);
				else uiMgr.Show(menu, single);
			}
		}
		
	#endregion
	
	#region Calls
		
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
			
			particle.SetActive(false);
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
			var clips = new AudioClip[]{ wotd_Clip1, fairyVoiceClip, wotd_Clip2, fairyVoiceClip };
			
				fairy.Speak(clips, wotd_ClipDelay, SpeechListen);
			
			void SpeechListen(){
				speechRecognizer.Listen(word, OnListenFinish);
				
				void OnListenFinish(){
					wordPopup.SetActive(false);
					
					var clips = new AudioClip[]{
						Tools.Random(wowClips),
						ycneyft_Clip
					};
					
					fairy.Speak(clips, ycneyft_ClipDelay, OnLevelFinished);
				}
			}
		}
		
		[ContextMenu("Change the Word of the Day")]
		public void ChangeTheWordOfTheDay(){
			wordOfTheDay = Tools.Random(wordOfTheDays);
			Debug.Log(wordOfTheDay.name);
		}
		
		#region UI Input
		
			public void SelectRoom(int index){
				SelectedRoom = level.rooms[index];
				selectedRoom_Index = index;
				
				wordPopupTxt.text = SelectedRoom.name;
				wordPopupIcon.gameObject.SetActive(false);
				
				wordPopup.SetActive(true);
				
				fairy.Speak(SelectedRoom.RoomNameFairyVoiceClip, 1f, SpeechListen);
				uiMgr.Hide(roomSelector);
				
				void SpeechListen(){
					speechRecognizer.Listen(SelectedRoom.name, GoToTheRoom);
					
					void GoToTheRoom(){
						wordPopup.SetActive(false);
						
						var charPoint = SelectedRoom.CharacterPoint;
					
						SetOrientation(fairy.transform, charPoint);
						SetOrientation(player.transform, charPoint);
						camMgr.SetPriority(selectedRoom.CameraPoint);
						
						uiMgr.Transition();
						GeneralAudio.Instance.Play("splashIntro");
						
						var wowClip = Tools.Random(wowClips);
						fairy.Speak(wowClip, wowClipsDelay, OnRoomEntered);
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
					
					uiMgr.Transition();
					uiMgr.Hide(roomTasksUI[selectedRoom_Index].gameObject);
					
					fairy.gameObject.SetActive(false);
					player.SetActive(false);
		
					Debug.Log(currentTask.description);
				}
			}
			
			public void FinishTask(){
				StartCoroutine(routine());
				
				IEnumerator routine(){
					particle.SetActive(true);
					
					yield return new WaitForSeconds(1.5f);
					
					uiMgr.Transition();
					
					camMgr.SetPriority(selectedRoom.CameraPoint);
					GeneralAudio.Instance.Play("splashIntro");
					
					player.SetActive(true);
					fairy.gameObject.SetActive(true);
					
					var clips = new AudioClip[]{
						Tools.Random(wowClips),
						lptw_Clip
					};
					
					fairy.Speak(clips, awsm_ClipDelay, OnTaskFinished);
					
					if(currentTask){
						currentTask.isFinished = true;
						Destroy(currentTask.gameObject);
					}
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
			Application.Quit();
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