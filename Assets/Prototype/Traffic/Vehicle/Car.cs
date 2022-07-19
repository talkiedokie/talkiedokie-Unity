using UnityEngine;

namespace Prototype.TrafficSystems
{
	public partial class Car : MonoBehaviour
	{
		// [SerializeField] float speed = 0.25f;
		public float speed = 0.25f;
		
		[Space()]
		[SerializeField] AudioSource engineAudioSource;
		[SerializeField] Vector2 enginePitchMinMax = new Vector2(0.5f, 2f);
		
		Transform _transform;
		Rigidbody rb;

		#region Initializations
		
			void Awake(){
				_transform = transform;
				rb = GetComponent<Rigidbody>();
				
				InitializeSpeedModifier_ObstacleCheck();
				InitializeSpeedModifier_TrafficLight();
				InitializeSpeedModifier_Junction();
			}
			
			void Start(){
				SetupWaypoints();
				
				RandomizeHonkPitch();
				RandomizeColor();
			}
			
		#endregion
		
		#region Updates
			
			public void OnUpdate(){
				SmoothenSpeedPercent();
			}
			
			public void OnFixedUpdate(){
				UpdateSplinePosition();
				UpdatePosition();
				UpdateRotation();
			}
			
			public void OnLateUpdate(){
				UpdateEngineSound();
				OnCarStoppedUpdate();
			}
			
			// Editor Updates
			#if UNITY_EDITOR
				
				void OnValidate(){
					// ClampDecelerationValue();
				}
			
				void OnDrawGizmos(){
					DrawObstacleCheckGizmo();
					DrawTrafficLightIndicatorGizmo();
				}
				
			#endif
			
		#endregion
		
		#region Motor
		
			// RIGIDBODY (kinematic)
			void UpdatePosition(){
				var targetPosition = currentWaypoint.GetPositionTo(targetWaypoint_Index, pathDstPercent);
				rb.MovePosition(targetPosition);
			}
			
			void UpdateRotation(){
				var currentPosition = _transform.position;
				var direction = currentPosition - previousPosition;
				
				if(direction != Vector3.zero){
					var targetRotation = Quaternion.LookRotation(direction, Vector3.up);
					rb.MoveRotation(targetRotation);
				}
				
				previousPosition = currentPosition;
			}
			
		#endregion
		
		void UpdateEngineSound(){
			if(engineAudioSource)
				engineAudioSource.pitch = Mathf.Lerp(
					enginePitchMinMax.x,
					enginePitchMinMax.y,
					speedPercent_Smooth
				);
		}
		
		void RandomizeColor(){
			var rend = GetComponentInChildren<Renderer>();
			
			if(rend){
				var color = Random.ColorHSV();
					color.a = 1;
				
				rend.material.color = color;
			}
		}
	}
}