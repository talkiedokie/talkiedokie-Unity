using UnityEngine;
using System.Collections;

namespace InsideBuilding.Gameplay
{
	public class ClosetHooping : Hooping
	{
		public Interactable[]
			girlClothes,
			boyClothes;
		
		public Transform[] insidePoints;
		int currentPoint;
		
		public bool rescaleInteractables;
		
		public Animator anim;
		int param = Animator.StringToHash("open");
		
		protected override void OnEnable(){
			EnableObjects(girlClothes, false);
			EnableObjects(boyClothes, false);
			
			switch(gameMgr.PlayerGender){
				case Gender.Male: interactableObjects = boyClothes; break;
				case Gender.Female: interactableObjects = girlClothes; break;
			}
			
			EnableObjects(interactableObjects, true);
			
			base.OnEnable();
			
			void EnableObjects(Interactable[] objects, bool b){
				foreach(var obj in objects)
					obj.gameObject.SetActive(b);
			}
		}
		
		protected override void Update(){
			base.Update();
			if(!anim) return;
			
			if(Input.GetMouseButton(0)){
				Ray ray = cam.ScreenPointToRay(Input.mousePosition);
				
				if(Physics.Raycast(ray, out var hit, 100, hoopLayer)){
					if(hit.transform == hoop.transform)
						anim.SetBool(param, true);
				}
				else anim.SetBool(param, false);
			}else anim.SetBool(param, false);
		}
		
		new public void Score(){
			StartCoroutine(delayOneFrame());
			
			IEnumerator delayOneFrame(){
				yield return null;
				
				selected.Enable(false);
				{
					currentPoint ++;
					currentPoint = currentPoint % insidePoints.Length;
					
					var insidePoint = insidePoints[currentPoint];
					
					var transform = selected.transform;
						transform.parent = insidePoint;
						transform.localPosition = Vector3.zero;
						transform.localRotation = Quaternion.identity;
						
						if(rescaleInteractables)
							transform.localScale = Vector3.one;
				}
				selected = null;
			}
			
			score ++;
			if(score >= maxScore) CompleteTask();
		}
	}
}