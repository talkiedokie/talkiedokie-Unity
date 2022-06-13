using UnityEngine;

namespace Minigame{
	namespace Conveyor
	{
		public class FloorTrigger : MonoBehaviour
		{
			void OnTriggerEnter(Collider col){
				if(col.TryGetComponent<Item>(out var item))
					item.Break();
			}
		}
	}
}
