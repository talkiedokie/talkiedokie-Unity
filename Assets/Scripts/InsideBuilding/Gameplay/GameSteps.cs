using UnityEngine;
using System.Collections;

namespace InsideBuilding.Gameplay
{
	public class GameSteps : MonoBehaviour
	{
		[SerializeField] Transform player;
		[SerializeField] Fairy fairy;
		[SerializeField] GameManager gameMgr;
		
		[Space()]
		[SerializeField, LabelOverride("Let's play the word")] AudioClip lptw_Clip;
		[SerializeField, LabelOverride("Clip Delay")] float lptw_ClipDelay = 1f;
		
		public bool isComplete{ get; private set; }
		
		void Awake(){
			if(!gameMgr)
				gameMgr = GameManager.Instance;
			
			gameMgr.onRoomEntered += ShowTasks;
			gameMgr.onTaskFinished += OnTaskFinished;
			gameMgr.onLevelFinished += FreeTime;
		}
		
		IEnumerator Start(){
			while(!gameMgr.hasStarted) yield return null;
			
			player = gameMgr.player.transform;
			
			if(!fairy)
				fairy = Fairy.Instance;
			
			gameMgr.SelectRoom();
		}
		
		void ShowTasks(){
			if(enabled) gameMgr.SelectTasks();
		}
		
		void OnTaskFinished(){
			if(!enabled) return;
			
			if(isComplete){
				gameMgr.SetGameMode(GameMode.Free);
				return;
			}
			
			fairy.Speak(
				lptw_Clip,
				lptw_ClipDelay,
				PlayTheWord
			);
			
			void PlayTheWord(){ gameMgr.WordOfTheDay(); }
		}
		
		void FreeTime(){
			if(!enabled) return;
			
			gameMgr.SetGameMode(GameMode.Free);
			isComplete = true;
		}
		
		void OnDestroy(){
			gameMgr.onRoomEntered -= ShowTasks;
			gameMgr.onTaskFinished -= OnTaskFinished;
			gameMgr.onLevelFinished -= FreeTime;
		}
	}
}