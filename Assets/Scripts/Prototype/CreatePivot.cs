using UnityEngine;

public class CreatePivot : MonoBehaviour
{
	[ContextMenu("Pivot")]
	public void Pivot(){
		var filter = GetComponent<MeshFilter>();
		var bounds = filter.sharedMesh.bounds;
		
		var parent = new GameObject(name).transform;
			parent.position = bounds.center + transform.position;
			parent.rotation = transform.rotation;
			transform.parent = parent;
		
		DestroyImmediate(this);
	}
}