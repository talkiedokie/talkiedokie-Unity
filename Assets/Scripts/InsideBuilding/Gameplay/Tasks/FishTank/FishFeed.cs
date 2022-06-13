using UnityEngine;

namespace InsideBuilding.Gameplay
{
	public class FishFeed : MonoBehaviour
	{
		public Aquarium aquarium;
		
		public Rigidbody rb;
		public LayerMask water;
		public float waterDrag = 10f;
		
		void OnTriggerEnter(Collider col){
			Debug.Log(col.name, col);
			
			bool isWater = Tools.CompareLayer(col.gameObject.layer, water);
			
			if(isWater){
				rb.drag = waterDrag;
				rb.velocity = Vector3.zero;
				
				aquarium.SetFishTarget(transform);
			}
		}
	}
}