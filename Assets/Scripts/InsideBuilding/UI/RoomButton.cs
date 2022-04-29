using UnityEngine;
using UnityEngine.UI;

public class RoomButton : CustomButton
{
	[SerializeField] Text label;
	
	int index;
	public static GameManager gameMgr;
	
	public RoomButton CreateInstance(Room room, int i){
		var instance = Instantiate(this, transform.parent, false);
			instance.index = i;
			instance.name = room.name;
			
			instance.label.text = room.name;
			instance.onClick.AddListener(delegate{ onClickAction(i); });
		
		return instance;
	}
	
	void onClickAction(int i){ gameMgr.SelectRoom(i); }
}