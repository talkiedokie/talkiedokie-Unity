using UnityEngine;

namespace Gameplay
{
	public class Steps : MonoBehaviour
	{
		Transform player, fairy;
		GameManager gameMgr;
		
		void Awake(){
			gameMgr = GameManager.Instance;
			
			gameMgr.onRoomEntered += SelectTask;
			gameMgr.onTaskFinished += PlayWordOfTheDay;
			gameMgr.onLevelFinished += ExitScene;
		}
		
		void Start(){		
			player = gameMgr.player.transform;
			fairy = Fairy.Instance.transform;
			
			gameMgr.SelectRoom();
		}
		
		void SelectTask(){ if(enabled) gameMgr.SelectTasks(); }
		void PlayWordOfTheDay(){ if(enabled) gameMgr.WordOfTheDay(); }
		
		void ExitScene(){
			if(!enabled) return;
			Debug.Log("EXIT");
			
			if(player.TryGetComponent<FpCtrl>(out var fpController))
				fpController.enabled = true;
		}
		
		void OnDestroy(){
			gameMgr.onRoomEntered -= SelectTask;
			gameMgr.onTaskFinished -= PlayWordOfTheDay;
			gameMgr.onLevelFinished -= ExitScene;
		}
	}
}