using UnityEngine;

public class SpawnInsideSphere : MonoBehaviour
{
	[SerializeField] float radius = 1f;
	[SerializeField] Vector3 position;
	[SerializeField] bool randomRotation = true;
	[SerializeField] Transform[] transforms;
	
	[ContextMenu("Spawn")]
	public void Spawn(){
		var position = transform.TransformPoint(this.position);
		
		foreach(var transform in transforms){
			var randomPosition = Random.insideUnitSphere * radius;
			transform.position = position + randomPosition;
			
			if(randomRotation)
				transform.rotation = Random.rotation;
		}
	}
	
	#if UNITY_EDITOR
		
		void OnDrawGizmosSelected(){
			var position = transform.TransformPoint(this.position);
				
				Gizmos.color = Color.green;
				Gizmos.DrawWireSphere(position, radius);
		}
		
	#endif
}