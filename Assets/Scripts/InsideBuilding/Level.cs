using UnityEngine;

namespace InsideBuilding
{
	public class Level : SceneObjectSingleton<Level>
	{
		public Room[] rooms;
		
		[HideInInspector]
		public int selectedRoomIndex;
	}
}