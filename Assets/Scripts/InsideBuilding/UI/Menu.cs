using UnityEngine;
using UnityEngine.UI;
using AccountsManagement;

namespace InsideBuilding
{
	public class Menu : MonoBehaviour
	{
		[SerializeField] Button locationBtn, wordBtn;
		[SerializeField] Text taskTxt;
		
		UIManager uiMgr => UIManager.Instance;
		GameManager gameMgr => GameManager.Instance;
		
		void OnEnable(){
			bool hasTask = gameMgr.CurrentTask != null;
			
			taskTxt.text = hasTask? "Cancel Task": "Select Task";
			
			locationBtn.interactable = !hasTask;
			wordBtn.interactable = !hasTask;
		}
		
		public void OnRestartButton(){
			SceneLoader.Current();
			uiMgr.Hide(gameObject);
		}
		
		public void OnLocationButton(){
			gameMgr.SelectRoom();
			gameMgr.SetGameMode(GameMode.StepByStep);
			
			uiMgr.Hide(gameObject);
		}
		
		public void OnTaskButton(){
			if(gameMgr.CurrentTask)
				gameMgr.CancelTask();
			
			else gameMgr.SelectTasks();
			
			gameMgr.SetGameMode(GameMode.StepByStep);
			uiMgr.Hide(gameObject);
		}
		
		public void OnWordButton(){
			gameMgr.WordOfTheDay();
			uiMgr.Hide(gameObject);
		}
		
		public void OnExitButton(){
			uiMgr.Hide(gameObject);
			
			AccountManager.Instance.Serialize();
			Application.Quit();
		}
	}
}