using UnityEngine;

namespace Prototype.TrafficSystems
{
	public partial class Car
	{
		SpeedModifier speedModifier_TrafficLight;
		
		void InitializeSpeedModifier_TrafficLight(){
			speedModifier_TrafficLight = new SpeedModifier("Traffic Light");
			speedModifiers.Add(speedModifier_TrafficLight);
		}
		
		public void CheckForTrafficLight(float stopDstPercent){
			if(!nextWaypoint) return;
			
			if(nextWaypoint.isOpen)
				speedModifier_TrafficLight.value = 1f;
			
			else{
				float amount = Mathf.Clamp01(stopDstPercent - pathDstPercent);
				speedModifier_TrafficLight.value = amount;
			}
		}
		
		#if UNITY_EDITOR
			
			void DrawTrafficLightIndicatorGizmo(){
				if(!nextWaypoint) return;
				
				if(!nextWaypoint.isOpen)
					Gizmos.DrawLine(_transform.position, nextWaypoint.position);
			}
			
		#endif
	}
}