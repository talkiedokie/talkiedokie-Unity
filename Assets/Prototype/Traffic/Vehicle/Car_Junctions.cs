using UnityEngine;

namespace Prototype.TrafficSystems
{
	public partial class Car
	{
		[SerializeField] GameObject
			lSignalLight,
			rSignalLight;
		
		bool
			isHeadingTowardsJunction,
			isChangingDirection;
		
		SpeedModifier speedModifier_Junction;
		
		// on awake
		void InitializeSpeedModifier_Junction(){
			speedModifier_Junction = new SpeedModifier("Junction");
			speedModifiers.Add(speedModifier_Junction);
		}
		
		// on custom update
		public void CheckForJunctions(float minSpeedPercent){
			if(!targetWaypoint) return;
			
			isHeadingTowardsJunction =
				targetWaypoint.IsJunction() ||
				targetWaypoint.slowDown;
			
			if(isHeadingTowardsJunction && isChangingDirection)
				speedModifier_Junction.value = Mathf.Lerp(1f, minSpeedPercent, pathDstPercent);
			
			else
				speedModifier_Junction.value = 1f;
		}
		
		// on custom update
		public void CheckForDirectionChange(float signalCheckAmount){
			if(!isHeadingTowardsJunction){
				if(lSignalLight) lSignalLight.SetActive(false);
				if(rSignalLight) rSignalLight.SetActive(false);
				return;
			}
			
			var _relativePos = _transform.InverseTransformPoint(nextWaypoint.position);
			float relativePos = _relativePos.x;
				
				if(lSignalLight) lSignalLight.SetActive(relativePos < -signalCheckAmount);
				if(rSignalLight) rSignalLight.SetActive(relativePos > signalCheckAmount);
			
			isChangingDirection = Mathf.Abs(relativePos) > signalCheckAmount;
		}
	}
}