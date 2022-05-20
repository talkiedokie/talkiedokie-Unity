using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using AccountsManagement;

public partial class GameManager
{
	public bool hasStarted{ get; private set; }
	
	IEnumerator Start(){
		hasStarted = false;
		Time.timeScale = 0f;
		
		var threads = new IEnumerator[]{
			SetupSingletonReferences(),
			CheckForSpeechSupport(),
			LoadLevel(),
			SetupUsersAccount(),
			SetupPlayerObject(),
			SetupRoomUI(),
			FinalSetup()
		};
		
		int count = threads.Length;
		float maxIndex = (float) count - 1;
		
		startLoadUI.SetActive(true);
		
		for(int i = 0; i < count; i++){
			yield return threads[i];
			float normalizedValue = (float) i / maxIndex;
			float percent = Mathf.Round(normalizedValue * 100f);
			
			startProgressImg.fillAmount = normalizedValue;
			startProgressTxt.text = "Loading " + percent + "%";
		}
		
		yield return new WaitForSecondsRealtime(1f);
			
			startLoadUI.SetActive(false);
			uiMgr.Transition();
		
		Time.timeScale = 1f;
		hasStarted = true;
	}
	
	#region Custom Threads
		
		IEnumerator SetupSingletonReferences(){
			uiMgr = UIManager.Instance;
			fairy = Fairy.Instance;
			
			cam = Camera.main;
			camMgr = CameraManager.Instance;
			
			yield return null;
		}
		
		IEnumerator CheckForSpeechSupport(){
			#if UNITY_EDITOR || UNITY_WINDOWS
			
				bool isSupported = STTMultiPlatformHandler.Instance.IsWindowSpeechSupported;
				
				if(!isSupported) winSpeechError.NotSupported();
				while(!isSupported) yield return null;
				
			#endif
			
			yield return null;
		}
		
		IEnumerator LoadLevel(){
			levelScene.LoadAdditive();
			
			int iteration = 0;
			
			while(!level){
				level = FindObjectOfType<Level>();
				iteration ++;
				yield return null;
			}
			
			if(iteration > 1) Debug.LogWarning("Level Search Iteration: " + iteration);
		}
		
		IEnumerator SetupUsersAccount(){
			accMgr = AccountManager.Instance;
			
			if(Login.hasLoggedIn) user = Login.user;
			else accMgr.LoadData();
			
			accMgr.currentUser = user;
			UserData = user.Reference;
			
			usernameTxt.text = UserData.name;
			
			yield return null;
		}
		
		IEnumerator SetupPlayerObject(){
			var playerPlaceHolder = GameObject.FindWithTag("Player").transform;
			
			player = Instantiate(
				characterPrefabs[(int) PlayerGender],
				playerPlaceHolder.position,
				playerPlaceHolder.rotation,
				playerPlaceHolder.parent
			);
			
			Destroy(playerPlaceHolder.gameObject);
			
			yield return null;
		}
		
		IEnumerator SetupRoomUI(){
			RoomButton.gameMgr = this;
			
			int count = level.rooms.Length;
			roomTasksUI = new RoomTasksUI[count];
			
			for(int i = 0; i < count; i++){
				var room = level.rooms[i];
				
				roomButtonTemplate.CreateInstance(room, i);
				roomTasksUI[i] = roomTasksUITemplate.CreateInstance(room, i);
				
				yield return null;
			}
			
			Destroy(roomButtonTemplate.gameObject);
			Destroy(roomTasksUITemplate.gameObject);
			
			yield return null;
		}
		
		IEnumerator FinalSetup(){
			ChangeTheWordOfTheDay();
			GeneralAudio.Instance.PlayMusic();
			
			yield return null;
		}
		
	#endregion
}