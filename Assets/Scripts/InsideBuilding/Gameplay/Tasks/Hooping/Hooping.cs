using UnityEngine;
using System.Collections;

namespace Gameplay
{
	public class Hooping : Task
	{
		[Space()]
		[SerializeField] protected Hoop hoop;
		[SerializeField] protected Interactable[] interactableObjects;
		
		protected int maxScore => interactableObjects.Length;
		protected int score;
		
		[SerializeField] int onDragSetLayerTo;
		
		[SerializeField] protected LayerMask
			interactables,
			dragFollow,
			hoopLayer;
		
		protected Interactable dragged, hovered, selected;
		
		public delegate void OnFinish();
		public OnFinish onFinish;
		
		/* protected override void Start(){
			base.Start();
			hoop._onTrigger += Score;
		} */
		
		protected override void OnEnable(){
			base.OnEnable();
			
			foreach(var i in interactableObjects)
				i.gameObject.SetActive(true);
			
			score = 0;
		}
		
		protected override void Update(){
			base.Update();
			if(!isPlaying) return;
			
			var ray = cam.ScreenPointToRay(Input.mousePosition);
			bool raycast = Physics.Raycast(ray, out var hit, 100, interactables);
			
			HandleHighlighting();
			HandleSelecting();
			HandleDragging();
			HandleDropping();
			
			void HandleHighlighting(){
				if(raycast && !dragged){
					hovered?.Highlight(false);
					hovered = hit.collider.GetComponent<Interactable>();
					hovered?.Highlight(true);
				}
				else{
					hovered?.Highlight(false);
					hovered = null;
				}
			}
			
			void HandleSelecting(){
				if(Input.GetMouseButtonDown(0)){
					// Deselect Current
						selected?.Enable(true);
						selected?.ResetLayer();
					
					// Select New
						if(raycast){
							selected = hovered? hovered: hit.collider.GetComponent<Interactable>();
							
							selected?.Enable(false);
							selected?.SetLayerTemporary(onDragSetLayerTo);
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
					if(raycast3 && dragged) hoop.Interact(dragged.gameObject);
					
					dragged?.Enable(true);
					dragged?.ResetLayer();
					
					dragged?.Highlight(false);
					dragged = null;
				}
			}
		}
		
		public override void Play(bool b){
			base.Play(b);
			// hoop.gameObject.SetActive(b);
		}
		
		public void Score(){
			selected = null;
			// StartCoroutine(DisablePhysicsDelay());
			
			score ++;
			if(score >= maxScore) CompleteTask();
		}
		
		IEnumerator DisablePhysicsDelay(){
			yield return new WaitForSeconds(1f);
			
			selected?.Enable(false);
			selected = null;
		}
	}
}