using UnityEngine;

namespace Prototype.TrafficSystems
{
	public partial class Car
	{
		[SerializeField] GameObject brakeLight;
		[SerializeField] AudioSource honkAudioSource;
		
		const float
			brakeTrigger = 0.8f,
			stopTrigger = 0.025f;
		
		bool isStopped;
		public float stopDuration{ get; private set; } // for traffic jam handling
		
		// on custom update
		public void HandleBraking(){
			if(brakeLight){
				bool isBreaking = speedPercent < brakeTrigger;
				brakeLight.SetActive(isBreaking);
			}
			
			if(nextWaypoint)
				isStopped = (speedPercent < stopTrigger) && nextWaypoint.isOpen;
		}
		
		void OnCarStoppedUpdate(){
			if(isStopped){
				Honk();
				stopDuration += Time.deltaTime;
			}
			else stopDuration = 0f;
		}
		
		void Honk(){
			if(!honkAudioSource) return;
			if(honkAudioSource.isPlaying) return;
			
			honkAudioSource.Play();		
			InteractWithHonkables();
		}
		
		void InteractWithHonkables(){
			if(!currentObstacle) return;
			
			var honkTrigger = currentObstacle.GetComponent<CarHonkTrigger>();
				honkTrigger?.OnCarHonk();
			
			if(honkTrigger) Debug.Log(honkTrigger, honkTrigger);
		}
	}
}