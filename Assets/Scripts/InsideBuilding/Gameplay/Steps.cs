using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
	public class Steps : MonoBehaviour
	{	
		// public GameObject button;
		public Transform defaultRoom, cam, fps;
		public Toggle fpToggle;
		
		Transform player, fairy;
		Vector3 f, p, c;
		Quaternion ff, pp, cc;
		
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
			
			f = fairy.localPosition;
			p = player.localPosition;
			c = cam.localPosition;
			ff = fairy.localRotation;
			pp = player.localRotation;
			cc = cam.localRotation;
			
			gameMgr.SelectRoom();
		}
		
		void SelectTask(){ if(enabled) gameMgr.SelectTasks(); }
		void PlayWordOfTheDay(){ if(enabled) gameMgr.WordOfTheDay(); }
		
		void ExitScene(){
			if(!enabled) return;
			/* 
			Time.timeScale = 0f;
			button.SetActive(true);
			 */
			Debug.Log("EXIT");
			
			fps.position = player.position;
			fps.rotation = player.rotation;
			
			fpToggle.isOn = true;
		}
		
		/* public void OnRestart(){
			Time.timeScale = 1f;
			
			Orient(fairy, f, ff);
			Orient(player, p, pp);
			Orient(cam, c, cc);
			
			Start();
			CameraManager.Instance.SetDefaultPriority();
		} */
		
		void Orient(Transform tr, Vector3 t, Quaternion tt){
			tr.parent = defaultRoom;
			tr.localPosition = t;
			tr.localRotation = tt;
		}
		
		void OnDestroy(){
			gameMgr.onRoomEntered -= SelectTask;
			gameMgr.onTaskFinished -= PlayWordOfTheDay;
			gameMgr.onLevelFinished -= ExitScene;
		}
	}
}