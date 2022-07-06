using UnityEngine;
using System.Collections;

namespace Prototype.TrafficSystems
{
	public class TrafficLightSystem : MonoBehaviour
	{
		[SerializeField] TrafficLight[] trafficLights;
		
		[SerializeField] float duration = 10f;
		WaitForSeconds step;
		
		void OnValidate() =>
			step = new WaitForSeconds(duration);
		
		IEnumerator Start(){
			step = new WaitForSeconds(duration);
			
			int count = trafficLights.Length;
			int index = 0;
			
			while(true){
				for(int i = 0; i < count; i++)
					trafficLights[i].IsGreen(i == index);
				
				yield return step;
				
				index ++;
				index = index % count;
			}
		}
		
		#if UNITY_EDITOR
			
			public float gizmoRadius = 1f;
			
			static readonly Color[] stateColors = new Color[]
			{
				Color.red,
				Color.yellow,
				Color.green
			};
			
			void OnDrawGizmos(){
				foreach(var tl in trafficLights){
					var color = stateColors[(int) tl.state];
					
					Gizmos.color = color;
					
					Gizmos.DrawSphere(
						tl.transform.position,
						gizmoRadius
					);
				}
			}
			
		#endif
		
		public void Reiterate(bool toggleUIValue){
			if(toggleUIValue)
				StartCoroutine(Start());
		}
	}
}