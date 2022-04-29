using UnityEngine;

public class Level : MonoBehaviour
{
	public Room[] rooms;
	
	[HideInInspector]
	public int selectedRoomIndex;
}