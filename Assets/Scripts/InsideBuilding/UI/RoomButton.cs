using UnityEngine;
using UnityEngine.UI;

namespace InsideBuilding
{
	public class RoomButton : CustomButton
	{
		[SerializeField] Text label;
		
		int index;
		public static GameManager gameMgr;
		
		Button button;
		
		public RoomButton CreateInstance(Room room, int i){
			var instance = Instantiate(this, transform.parent, false);
				instance.index = i;
				instance.name = room.name;
				
				instance.label.text = room.name;
				instance.onClick.AddListener(delegate{ onClickAction(i); });
				
				instance.button = instance.GetComponent<Button>();
			
			return instance;
		}
		
		void onClickAction(int i){ gameMgr.OnRoomSelected(i); }
		
		public void IsInteractable(bool interactable){ button.interactable = interactable; }
	}
}