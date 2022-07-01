using UnityEngine;

namespace Prototype.Cars
{
	public class Car : MonoBehaviour
	{
		public Transform obstacleCheckPoint;
		
		[SerializeField] float acceleration = 2.5f;
		[SerializeField] float speed;
		
		[Space()]
		public CarTargetPoint targetPoint;
		CarTargetPoint previousPoint, nextPoint;
		
		[SerializeField] Cinemachine.CinemachineVirtualCamera camPoint;
		
		[Foldout("Lightings")]
		[SerializeField] GameObject brakeLight;
		[SerializeField] GameObject lSignalLight, rSignalLight;
		
		[Foldout("Sounds")]
		[SerializeField] AudioSource engineAudioSource;
		[SerializeField] AudioSource honkAudioSource;
		
		[SerializeField] Vector2 enginePitchMinMax = new Vector2(0.5f, 3f);
		[SerializeField] Vector2 honkPitchMinMax = new Vector2(0.5f, 1.5f);
		
		public float currentSpeedPercent{ get; private set; }
		
		public bool isStopped{ get; private set; }
		public float stopDuration{ get; private set; }
		
		// [Range(0,1f)] public float currentSpeedPercent;
		
		public Transform _transform{ get; private set; }
		
		Collider obstacle;
		float dstToObstacle;
		Vector3 sphereCastPosition;
		
		Vector3 dirToTarget;
		float dstToTarget_Sqr;
		
		CarManager mgr;
		
		#region Unity Methods
			
			// Init
			void Awake(){
				mgr = CarManager.Instance;
				_transform = transform;
				
				// FindNearestTargetPoint();
			}
			
			void Start(){
				speed = mgr.GetRandomSpeed();
				nextPoint = targetPoint.GetNextPoint();
				
				honkAudioSource.pitch = Random.Range(
					honkPitchMinMax.x,
					honkPitchMinMax.y
				);
			}
			
			// Updates
			void Update(){
				if(!targetPoint) return;
				
				dirToTarget = targetPoint.position - _transform.position;
				dstToTarget_Sqr = dirToTarget.sqrMagnitude;
				
				float deltaTime = Time.deltaTime;
				
				HandleMovement(deltaTime);
				HandleRotation(deltaTime);
				HandleTargetSelection();
			}
			
			void LateUpdate(){
				CalculateSpeedPercent_Obstacles();
				
				currentSpeedPercent = currentSpeedPercent_Obstacles * currentSpeedPercent_StopLight;
				
				isStopped = currentSpeedPercent < 0.025f && !isStoppingByStopLight;
				
				if(isStopped) stopDuration += Time.deltaTime;
				else stopDuration = 0f;
				
				SmoothenCurrentSpeedPercent();
			}
			
			void OnDrawGizmosSelected(){
				if(!mgr) mgr = CarManager.Instance;
				if(!_transform) _transform = transform;
				
				if(Application.isPlaying && obstacle){
					Gizmos.color = Color.red;
					
					Gizmos.DrawWireSphere(
						sphereCastPosition,
						mgr.obstacleCheckSphereCastRadius
					);
				}
				
				Gizmos.color = Color.white;
				Gizmos.DrawWireSphere(_transform.position, mgr.turningDistance);
			}
			
		#endregion
		
		#region Custom Ticks
			
			public void CheckForObstacle(
				float distance, // unit (meter)
				LayerMask layers
			){
				bool sphereCast = Physics.SphereCast(
					obstacleCheckPoint.position,
					mgr.obstacleCheckSphereCastRadius,
					obstacleCheckPoint.forward,
					out var hit,
					distance,
					layers
				);
				
				if(sphereCast){
					obstacle = hit.collider;
					dstToObstacle = hit.distance;
					
					sphereCastPosition = hit.point;
				}
				
				else{
					obstacle = null;
					dstToObstacle = 0f;
				}
			}
			
			public void UpdateLightings(){
				if(!gameObject.activeSelf) return;
				
				HandleSignaling();
				HandleBraking();
				
				// CalculateSpeedPercent_Obstacles();
				// CalculateSpeedPercent_Junctions();
			}
			
		#endregion
		
		#region Driving
			
			void HandleMovement(float deltaTime){
				float speed = this.speed * deltaTime;
				speed *= currentSpeedPercent_Smooth;
				// speed *= currentSpeedPercent;
				_transform.position += _transform.forward * speed;
			}
			
			void HandleRotation(float deltaTime){
				var direction = dirToTarget;
				direction.y = 0f;
				
				var lookRotation = Quaternion.LookRotation(direction.normalized);
				float rotationSpeed = mgr.rotationSpeed * currentSpeedPercent_Smooth;
				// float rotationSpeed = mgr.rotationSpeed * currentSpeedPercent;
				
				_transform.rotation = Quaternion.Slerp(
					_transform.rotation,
					lookRotation,
					deltaTime * rotationSpeed
				);
			}
			
			// [Range(0,1)]
			public float currentSpeedPercent_StopLight{ get; private set; }
			public bool isStoppingByStopLight{ get; private set; }
			
			void HandleTargetSelection(){
				if(nextPoint.stop && previousPoint){
					float roadLength_Sqr = (targetPoint.position - previousPoint.position).sqrMagnitude;
					float stopTrigger = roadLength_Sqr * mgr.stopLightBreakDistPercent;
					
						currentSpeedPercent_StopLight = (dstToTarget_Sqr / roadLength_Sqr);
						currentSpeedPercent = currentSpeedPercent_StopLight;
					
					isStoppingByStopLight = true;
					return;
				}
				
				float turnDst_Sqr = Mathf.Pow(mgr.turningDistance, 2);
				
				if(dstToTarget_Sqr < turnDst_Sqr){
					previousPoint = targetPoint;
					targetPoint = nextPoint;
					nextPoint = targetPoint.GetNextPoint();
				}
				
				currentSpeedPercent_StopLight = 1f;
				isStoppingByStopLight = false;
			}
			
		#endregion
		
		#region Lights
			
			void HandleSignaling(){
				if(!nextPoint) return;
				if(!lSignalLight || !rSignalLight) return;
				
				if(!targetPoint.isJunction){
					lSignalLight.SetActive(false);
					rSignalLight.SetActive(false);
					return;
				}
				
				var _relativePos = _transform.InverseTransformPoint(nextPoint.position);
				float relativePos = _relativePos.x;
				
				lSignalLight.SetActive(relativePos < -mgr.signalCheckAmount);
				rSignalLight.SetActive(relativePos > mgr.signalCheckAmount);
			}
			
			void HandleBraking(){
				bool isBreaking = currentSpeedPercent < 0.8f;
				
				if(brakeLight)
					brakeLight.SetActive(isBreaking);
				
				engineAudioSource.pitch = Mathf.Lerp(
					enginePitchMinMax.x,
					enginePitchMinMax.y,
					currentSpeedPercent_Smooth
					// currentSpeedPercent
				);
				
				// if(currentSpeedPercent_Smooth < 0.025f && !honkAudioSource.isPlaying)
				if(isStopped && !honkAudioSource.isPlaying){
					honkAudioSource.Play();
					
					if(obstacle){
						var honkTrigger = obstacle.GetComponent<CarHonkTrigger>();
							honkTrigger?.OnCarHonk();
						
						if(honkTrigger) Debug.Log(honkTrigger, honkTrigger);
					}
				}
			}
			
		#endregion
		
		#region Speed Percent
			
			float currentSpeedPercent_Obstacles;
			
			float
				currentSpeedPercent_Smooth,
				currentSpeedPercent_SmoothVel;
			
			void CalculateSpeedPercent_Obstacles(){
				if(mgr.obstacleCheck && obstacle){
					float currentDst = dstToObstacle - mgr.stoppingDistance;
					float divider = mgr.obstacleCheckDistance - mgr.stoppingDistance;
					
					currentSpeedPercent_Obstacles = Mathf.Clamp01(currentDst / divider);
				}
				
				else currentSpeedPercent_Obstacles = 1f;
			}
			
			/* [Range(0,1)]
			public float multiplier;
			public float prevAndTargetDistance;
			public bool isJunction;
			
			void CalculateSpeedPercent_Junctions(){
				isJunction = targetPoint.isJunction;
				
				if(!targetPoint.isJunction) return;
				if(!previousPoint) return;
				
					prevAndTargetDistance = (targetPoint.position - previousPoint.position).magnitude;
			
				// float lerp = maxDistance_Sqr / dstToTarget_Sqr;
				// multiplier = Mathf.Lerp(1f, mgr.speedPercentWhenChangingDirection, lerp);
				
				// currentSpeedPercent *= multiplier;
			} */
			
			void SmoothenCurrentSpeedPercent(){
				float brakeAmount = acceleration * currentSpeedPercent;
				
				currentSpeedPercent_Smooth = Mathf.SmoothDamp(
					currentSpeedPercent_Smooth,
					currentSpeedPercent,
					ref currentSpeedPercent_SmoothVel,
					brakeAmount // the closer the obstacle is, the more instant the speed change must be
				);
			}
			
		#endregion
		
		/* void FindNearestTargetPoint(){
			if(!targetPoint){
				var wayPointMgr = WayPointDrawer.Instance;
				
				float distance = float.MaxValue;
				CarTargetPoint nearestPoint = null;
				
				var position = _transform.position;
				
				foreach(var point in wayPointMgr.targetPoints){
					float currentDst = (point.position - position).sqrMagnitude;
					
					if(currentDst < distance){
						nearestPoint = point;
						distance = currentDst;
					}
				}
				
				targetPoint = nearestPoint;
			}
		} */
		
		[ContextMenu("Focus")]
		public void Focus() =>
			CameraManager.Instance.SetPriority(camPoint);
	}
}