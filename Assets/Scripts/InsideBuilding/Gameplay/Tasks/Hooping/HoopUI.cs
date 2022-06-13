using UnityEngine;
using System.Collections;

namespace InsideBuilding.Gameplay
{
	public class HoopUI : MonoBehaviour
	{
		[SerializeField] AnimationCurve animCurve;
		
		Animator anim;
		Camera cam;
		
		public HoopUI CreateInstance(){
			var uiMgr = UIManager.Instance;
			var instance = Instantiate(this, uiMgr.transform, false);
			
				instance.cam = Camera.main;
				instance.anim = instance.GetComponent<Animator>();
			
			uiMgr.Show(instance.gameObject);
			
			return instance;
		}
		
		public void Show( // 0.5f, 3, 1f
			float delay,
			int repeatition,
			float animationDuration,
			Component[] startObjects,
			Transform destination
		){
			gameObject.SetActive(true);
			StartCoroutine(routine());
			
			IEnumerator routine(){
				var step = new WaitForSeconds(delay);
				yield return step;
				
				for(int i = 0; i < repeatition; i++){
					float timer = 0f;
					
					var centerPoint = GetInteractablesCenterPoint(startObjects);
					var startPos = cam.WorldToScreenPoint(centerPoint);
					
					while(timer < animationDuration){
						var destPos = cam.WorldToScreenPoint(destination.position);
						float lerp = animCurve.Evaluate(timer / animationDuration);
						
						transform.position = Vector3.Lerp(startPos, destPos, lerp);
						
						timer += Time.deltaTime;
						yield return null;
					}
					
					yield return step;
				}
				
				anim?.SetTrigger("hide");
			}
		}
		
		Vector3 GetInteractablesCenterPoint(Component[] objects){
			var bounds = new Bounds(objects[0].transform.position, Vector3.zero);
			
			foreach(var obj in objects)
				bounds.Encapsulate(obj.transform.position);
			
			return bounds.center;
		}
	}
}