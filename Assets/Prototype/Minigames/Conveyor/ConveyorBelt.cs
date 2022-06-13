using UnityEngine;

namespace Minigame{
	namespace Conveyor
	{
		public class ConveyorBelt : SceneObjectSingleton<ConveyorBelt>
		{
			[SerializeField] float
				beltSpeed = 0.15f,
				itemSpeed = 2.4f;
			
			[SerializeField] Vector3 direction = Vector3.back;
			public Vector3 ItemVelocity => direction.normalized * itemSpeed;
			
			[Space()]
			[SerializeField] Material beltMaterial;
			[SerializeField] string materialParam = "_scrollSpeed";
			
			void OnValidate(){
				beltMaterial?.SetFloat(materialParam, beltSpeed);
			}
		}
	}
}