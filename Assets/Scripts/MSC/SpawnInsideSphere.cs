using UnityEngine;

public class SpawnInsideSphere : MonoBehaviour
{
	[SerializeField] float radius = 1f;
	[SerializeField] Vector3 position;
	
	[SerializeField] bool
		randomRotation = true,
		randomScale = false;
	
	[SerializeField] Vector2 randomScaleMinMax = new Vector2(0.5f, 2f);
	
	[ContextMenu("Spawn")]
	public void Spawn(){
		var position = transform.TransformPoint(this.position);
		
		foreach(Transform child in transform){
			var randomPosition = Random.insideUnitSphere * radius;
			child.position = position + randomPosition;
			
			if(randomRotation){
				// child.rotation = Random.rotation;
				
				var eulerAngles = child.eulerAngles;
					eulerAngles.y = Random.Range(0, 360);
				
				child.eulerAngles = eulerAngles;
			}
			
			if(randomScale){
				float scaleValue = Random.Range(randomScaleMinMax.x, randomScaleMinMax.y);
				child.localScale *= scaleValue;
			}
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