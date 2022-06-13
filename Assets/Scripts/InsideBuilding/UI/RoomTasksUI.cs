using UnityEngine;

namespace InsideBuilding
{
	public class RoomTasksUI : MonoBehaviour
	{
		[SerializeField] RoomTaskButton buttonTemplate;
		RoomTaskButton[] buttons;
		
		Room room;
		int index;
		
		public RoomTasksUI CreateInstance(Room room, int index){
			var instance = Instantiate(this, transform.parent, false);
				instance.room = room;
				instance.index = index;
				instance.name = room.name + " Tasks";
				
				RoomTaskButton.gameMgr = GameManager.Instance;
				
				int count = room.tasks.Length;
				instance.buttons = new RoomTaskButton[count];
				
				for(int i = 0; i < count; i++)
					instance.buttons[i] = instance.buttonTemplate.CreateInstance(room, room.tasks[i], i);
			
			Destroy(instance.buttonTemplate.gameObject);
			return instance;
		}
		
		public void OnTaskSelected(int index){
			for(int i = 0; i < buttons.Length; i++)
				buttons[i].IsSelected(i == index);
		}
	}
}