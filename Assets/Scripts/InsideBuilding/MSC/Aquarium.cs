using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Aquarium : MonoBehaviour
{
	public Bounds waterBounds{ get; private set; }
	public Transform waterReference;
	
	[Range(0,1)]
	public float
		waterLevel = 1f,
		boundsOffset = 0.85f;
	
	Vector3 min, max;
	
	void OnValidate(){
		if(!Application.isPlaying)
			UpdateWaterLevel();
	}
	
	void OnBecameVisible(){ enabled = true; }
	void OnBecameInvisible(){ enabled = false; }
	
	void Update(){
		UpdateWaterLevel();
		
		waterBounds = new Bounds(
			waterReference.position,
			waterReference.lossyScale * boundsOffset
		);
		
		min = waterBounds.min;
		max = waterBounds.max;
	}
	
	public void UpdateWaterLevel(){
		waterReference.parent.localScale = new Vector3(1, waterLevel, 1);
	}
	
	public Vector3 Clamp(Vector3 position){
		position.x = Mathf.Clamp(position.x, min.x, max.x);
		position.y = Mathf.Clamp(position.y, min.y, max.y);
		position.z = Mathf.Clamp(position.z, min.z, max.z);
		
		return position;
	}
	
	void OnDrawGizmos(){
		if(Application.isPlaying) return;
		if(!waterReference) return;
		
		waterBounds = new Bounds(
			waterReference.position,
			waterReference.lossyScale
		);
		
		var center = waterBounds.center;
		var size = waterBounds.size;
		
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(center, size);
		
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(center, size * boundsOffset);
	}
}