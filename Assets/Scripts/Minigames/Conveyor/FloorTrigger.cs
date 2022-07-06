using UnityEngine;

namespace Minigame{
	namespace Conveyor
	{
		public class FloorTrigger : MonoBehaviour
		{
			public float radius = 1f;
			public LayerMask interactables;
			
			Transform _transform;
			
			void Awake(){
				_transform = transform;
			}
			
			void Update(){
				var position = _transform.position;
				
				var cols = Physics.OverlapSphere(position, radius, interactables);
				
				foreach(var col in cols){
					if(col.TryGetComponent<Item>(out var item))
						item.Break();
				}
			}
			
			void OnDrawGizmos(){
				if(!_transform)
					_transform = transform;
				
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(_transform.position, radius);
			}
			
			/* void OnTriggerEnter(Collider col){
				if(col.TryGetComponent<Item>(out var item))
					item.Break();
			} */
		}
	}
}
