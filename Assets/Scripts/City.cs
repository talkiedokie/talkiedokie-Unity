using UnityEngine;

namespace Prototype
{
	public class City : MonoBehaviour
	{
		public Transform player;
		
		public static Vector3 playerPosition;
		
		void Awake(){
			player.position = playerPosition + Vector3.up;
		}
	}
}