using System.Collections.Generic;
using UnityEngine;

namespace Prototype.TrafficSystems
{
	public partial class Car
	{
		[SerializeField] float accelerationTime = 0.15f;
		[SerializeField] List<SpeedModifier> speedModifiers = new List<SpeedModifier>();
		
		// public float speedPercent{ get; private set; }
		[Range(0,1)] public float speedPercent;
		
		public float speedPercent_Smooth{ get; private set; }
		float speedPercent_SmoothVel;
		
		public void CalculateSpeedPercent(){
			speedPercent = 1f;
			
			speedModifiers.ForEach(
				modifier => {
					speedPercent *= modifier.value;
			});
			
			speedPercent = Mathf.Clamp01(speedPercent);
		}
		
		void SmoothenSpeedPercent(){
			float smoothTime = accelerationTime * speedPercent;
			
			speedPercent_Smooth = Mathf.SmoothDamp(
				speedPercent_Smooth,
				speedPercent,
				ref speedPercent_SmoothVel,
				smoothTime
			);
		}
		
		[System.Serializable]
		public class SpeedModifier{ // must be reference type
			[HideInInspector] public string name;
			[Range(0,1)] public float value;
			
			public SpeedModifier(string name){
				this.name = name;
				value = 1f;
			}
		}
	}
}