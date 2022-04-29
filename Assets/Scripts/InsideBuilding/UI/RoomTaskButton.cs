using UnityEngine;
using UnityEngine.UI;

public class RoomTaskButton : CustomButton
{
	[SerializeField] Button button;
	[SerializeField] Text label;
	[SerializeField] int labelLength = 50;
	
	Room room;
	Task task;
	int index;
	
	// bool isActiveTask => room.ActiveTask == task;
	public static GameManager gameMgr;
	
	// void OnEnable(){
		// if(task.isFinished) gameObject.SetActive(false);
	// }
	
	public RoomTaskButton CreateInstance(Room room, Task task, int i){
		var instance = Instantiate(this, transform.parent, false);
			instance.room = room;
			instance.task = task;
			instance.index = i;
		
			string description = task.description;
			
			instance.name = Tools.Clamp(description, 15);
			instance.label.text = Tools.Clamp(description, labelLength);
		
			// instance.gameObject.SetActive(!task.exclude);
			instance.onClick.AddListener(delegate{ instance.onClickAction(); });
		
		return instance;
	}
	
	void onClickAction(){
		// if(isActiveTask) gameMgr.OnTaskSelected(index);
		// else Debug.LogWarning("Task is not active", this);
		
		gameMgr.OnTaskSelected(index);
	}
}

