using UnityEngine;
using System.Collections;
using AccountsManagement;

namespace InsideBuilding
{
	public partial class GameManager
	{
		IEnumerator SetupSingletonReferences(){
			uiMgr = UIManager.Instance;
			fairy = Fairy.Instance;
			
			cam = Camera.main;
			camMgr = CameraManager.Instance;
			
			yield return null;
		}
		
		IEnumerator CheckForSpeechSupport(){
			#if UNITY_EDITOR_WIN || UNITY_WINDOWS || UNITY_STANDALONE_WIN
			
				bool isSupported = STTMultiPlatformHandler.Instance.IsWindowSpeechSupported;
				
				if(!isSupported) winSpeechError.NotSupported();
				while(!isSupported) yield return null;
				
			#endif
			
			yield return null;
		}
		
		IEnumerator LoadLevel(){
			if(City.isLoaded)
				levelScene = LevelScene;
			
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
			
			playerDefaultPoint = new GameObject("playerDefaultPoint").transform;
			playerDefaultPoint.position = playerPlaceHolder.position;
			playerDefaultPoint.rotation = playerPlaceHolder.rotation;
			playerDefaultPoint.parent = playerPlaceHolder.parent;
			
			Destroy(playerPlaceHolder.gameObject);
			
			yield return null;
		}
		
		IEnumerator SetupRoomUI(){
			RoomButton.gameMgr = this;
			
			int count = level.rooms.Length;
			roomButtons = new RoomButton[count];
			roomTasksUI = new RoomTasksUI[count];
			
			for(int i = 0; i < count; i++){
				var room = level.rooms[i];
				
				roomButtons[i] = roomButtonTemplate.CreateInstance(room, i);
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
	}
}