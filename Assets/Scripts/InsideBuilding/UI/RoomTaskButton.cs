using UnityEngine;
using UnityEngine.UI;

namespace InsideBuilding
{
	public class RoomTaskButton : CustomButton
	{
		[SerializeField] Button button;
		[SerializeField] Text label;
		[SerializeField] int labelLength = 50;
		[SerializeField] GameObject indicator;
		
		Room room;
		Task task;
		int index;
		
		public static GameManager gameMgr;
		
		public RoomTaskButton CreateInstance(Room room, Task task, int i){
			var instance = Instantiate(this, transform.parent, false);
				instance.name = task.name;
				instance.room = room;
				instance.task = task;
				instance.index = i;
			
				instance.onClick.AddListener(delegate{ instance.onClickAction(); });
			
			return instance;
		}
		
		void onClickAction(){
			gameMgr.OnTaskSelected(index);
		}
		
		public void IsSelected(bool isSelected){
			indicator.SetActive(isSelected);
		}
		
		void OnEnable(){
			// Debug.Log("MUST REPHRASE");
			task.RephraseDescription();
			
			string description = task.description.text;
			label.text = Tools.Clamp(description, labelLength);
		}
	}
}