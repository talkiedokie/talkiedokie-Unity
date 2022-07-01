using UnityEngine;
using System.Collections;

namespace InsideBuilding.Gameplay
{
	public class Hooping : Task
	{
		// [Foldout("References")]
		[SerializeField] protected Hoop hoop;
		[SerializeField] protected Interactable[] interactableObjects;
		
		protected Interactable dragged, hovered, selected;
		protected int maxScore => interactableObjects.Length;
		
		// [Foldout("Layer Masking")]
		[SerializeField] int onDragSetLayerTo;
		
		[SerializeField] protected LayerMask
			interactables,
			dragFollow,
			hoopLayer;
			
		[Space()]
		[SerializeField] protected HoopUI instructionUI;
		static HoopUI InstructionUI;
		
		public delegate void OnFinish();
		public OnFinish onFinish;
		
		protected override void OnEnable(){
			base.OnEnable();
			
			foreach(var i in interactableObjects)
				i.gameObject.SetActive(true);
		}
		
		protected override void Update(){
			base.Update();
			if(!isPlaying) return;
			
			var ray = cam.ScreenPointToRay(Input.mousePosition);
			bool raycast = Physics.Raycast(ray, out var hit, 100, interactables);
			
			// HandleHighlighting();
			HandleSelecting();
			HandleDragging();
			HandleDropping();
			
			/* void HandleHighlighting(){
				if(raycast && !dragged){
					hovered?.Highlight(false);
					hovered = hit.collider.GetComponent<Interactable>();
					hovered?.Highlight(true);
				}
				else{
					hovered?.Highlight(false);
					hovered = null;
				}
			} */
			
			void HandleSelecting(){
				if(Input.GetMouseButtonDown(0)){
					// Deselect Current
						selected?.Enable(true);
						// selected?.ResetLayer();
					
					// Select New
						if(raycast){
							selected = hovered? hovered: hit.collider.GetComponent<Interactable>();
							
							// selected?.Enable(false);
							// selected?.SetLayerTemporary(onDragSetLayerTo);
						}
						else selected = null;
				}
			}
			
			void HandleDragging(){
				dragged = selected;
				
				if(Input.GetMouseButton(0) && dragged){
					bool raycast2 = Physics.Raycast(ray, out var hit2, 10, dragFollow);
					if(raycast2) dragged.SetPosition(hit2.point);
				}
			}
			
			void HandleDropping(){
				if(Input.GetMouseButtonUp(0)){
					bool raycast3 = Physics.Raycast(ray, out var hit2, 10, hoopLayer);
					
					if(raycast3 && dragged)
						OnDrop(hit2);
					
					dragged?.Enable(true);
					// dragged?.ResetLayer();
					
					dragged?.Highlight(false);
					dragged = null;
				}
			}
		}
		
		protected virtual void OnDrop(RaycastHit rayInfo){
			hoop.Interact(dragged.gameObject);
		}
		
		public override void Play(bool b){
			base.Play(b);
			
			if(b){
				if(!InstructionUI)
					InstructionUI = instructionUI.CreateInstance();
				
				InstructionUI.Show(0.5f, 3, 1, interactableObjects, hoop.transform);
			}
		}
		
		public void Score(){
			selected = null;
			
			int score = hoop.containedObjects.Count;
			if(score >= maxScore) CompleteTask();
		}
		
		/* IEnumerator DisablePhysicsDelay(){
			yield return new WaitForSeconds(1f);
			
			selected?.Enable(false);
			selected = null;
		} */
	}
}