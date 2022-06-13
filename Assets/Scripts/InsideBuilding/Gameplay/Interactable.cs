using UnityEngine;

namespace InsideBuilding.Gameplay
{
	public class Interactable : MonoBehaviour
	{
		[SerializeField] Renderer[] highlightRenderers;
		
		[SerializeField] Rigidbody _rigidbody;
		[SerializeField] Collider _collider;
		
		int defaultLayer;
		
		[ContextMenu("Awake")]
		public void Awake(){
			if(!_collider) GetComponent<Collider>();
			if(!_rigidbody) GetComponent<Rigidbody>();
		}
		
		void Start(){
			defaultLayer = gameObject.layer;
		}
		
		public void Highlight(bool b){
			string param = "_alpha";
			float highlight = b? 1f: 0f;
			
			foreach(var rend in highlightRenderers){
				foreach(var material in rend.materials)
					material.SetFloat(param, highlight);
			}
		}
		
		public void Enable(bool b){
			_rigidbody.isKinematic = !b;
			_collider.enabled = b;
		}
		
		public void SetLayerTemporary(int layer){
			gameObject.layer = layer;
			
			for(int i = 0; i < transform.childCount; i++){
				var child = transform.GetChild(i).gameObject;
					child.layer = layer;
			}
		}
		
		public void ResetLayer(){
			SetLayerTemporary(defaultLayer);
		}
		
		public void SetPosition(Vector3 position){
			transform.position = position;
		}
	}
}