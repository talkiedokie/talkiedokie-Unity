using UnityEngine;

public class Level : SceneObjectSingleton<Level>
{
	public Room[] rooms;
	
	[HideInInspector]
	public int selectedRoomIndex;
}