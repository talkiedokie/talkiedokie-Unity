using UnityEngine;
using InsideBuilding;
using System.Collections;
 
namespace Prototype
{
	public class RandomTask : MonoBehaviour
	{
		public float duration = 30f;
		float timer;
		int targetTask;
		
		GameManager gameMgr;
		Fairy fairy;
		
		FpCtrl _fpCtrl;
		FpCtrl fpCtrl{
			get{
				if(!_fpCtrl)
					_fpCtrl = FindObjectOfType<FpCtrl>(true);
				return _fpCtrl;
			}
		}
		
		void Start(){
			gameMgr = GameManager.Instance;
			gameMgr.onGameModeChanged += StartRandomTask;
			gameMgr.onTaskSelected += DisableFpCtrl;
			
			fairy = Fairy.Instance;
		}
		
		void OnDisable(){
			gameMgr.onGameModeChanged -= StartRandomTask;
			gameMgr.onTaskSelected -= DisableFpCtrl;
		}
		
		void StartRandomTask(GameMode mode){
			if(mode != GameMode.Free) return;
			
			StartCoroutine(routine());
			
			IEnumerator routine(){
				timer = duration * Random.value;
				yield return new WaitForSeconds(timer);
				
				var level = FindObjectOfType<Level>();
				var room = Tools.Random(level.rooms);
					
					gameMgr.SelectedRoom = room;
					
					gameMgr.SetOrientation(fairy.transform, room.fairyPoint);
					fairy.Appear();
				
				var task = Tools.Random(room.tasks, out int targetTask);
					task.RephraseDescription();
				
				fairy.onInteraction += EnterTaskThroughFairyInteraction;
			}
		}
		
		void DisableFpCtrl(){
			fpCtrl.enabled = false;
		}
		
		void EnterTaskThroughFairyInteraction(){
			gameMgr.OnTaskSelected(targetTask);
			fairy.onInteraction -= EnterTaskThroughFairyInteraction;
		}
	}
}