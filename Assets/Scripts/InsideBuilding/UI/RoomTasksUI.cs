using UnityEngine;

public class RoomTasksUI : MonoBehaviour
{
	[SerializeField] RoomTaskButton buttonTemplate;
	
	Room room;
	int index;
	
	public RoomTasksUI CreateInstance(Room room, int index){
		var instance = Instantiate(this, transform.parent, false);
			instance.room = room;
			instance.index = index;
			instance.name = room.name + " Tasks";
			
			RoomTaskButton.gameMgr = GameManager.Instance;
			int i = 0;
			
			foreach(var task in room.tasks){
				instance.buttonTemplate.CreateInstance(room, task, i);
				i ++;
			}
		
		Destroy(instance.buttonTemplate.gameObject);
		return instance;
	}
}