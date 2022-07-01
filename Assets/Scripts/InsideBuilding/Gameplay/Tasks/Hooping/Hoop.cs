using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace InsideBuilding.Gameplay
{
	public class Hoop : MonoBehaviour
	{
		[SerializeField] LayerMask layer;
		[SerializeField] GameObject[] _particles;
		[SerializeField] float particleDespawnTime = 2f;
		
		[Space()]
		[SerializeField] UnityEvent onTrigger;
		
		public List<GameObject> containedObjects{ get; private set; } = new List<GameObject>();
		
		void OnTriggerEnter(Collider col){
			var obj = col.gameObject;
			
			if(CheckLayer(obj))
				Interact(obj);
		}
		
		bool CheckLayer(GameObject obj){
			return ((1 << obj.layer) & layer) != 0;
		}
		
		public void Interact(GameObject obj){
			// Debug.Log(obj.name, obj);
			
			if(containedObjects.Contains(obj)) return;
			containedObjects.Add(obj);
			
			onTrigger?.Invoke();
			SpawnParticle();
		}
		
		void SpawnParticle(){
			var obj = Tools.Random(_particles);
			var particle = Instantiate(obj, transform.position, Quaternion.identity);
			
			Destroy(particle, particleDespawnTime);
		}
	}
}