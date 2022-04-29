using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Mirror : MonoBehaviour
{
	static Transform cam;
	Transform parent;
	
	void Awake(){
		if(!cam) cam = Camera.main.transform;
		parent = transform.parent;
	}
	
	void OnBecameVisible(){ enabled = true; }
	void OnBecameInvisible(){ enabled = false; }
	
	void LateUpdate(){
		var dirFromCam = transform.position - cam.position;
		var reflection = Vector3.Reflect(dirFromCam, parent.forward);
		transform.forward = reflection;
	}
}