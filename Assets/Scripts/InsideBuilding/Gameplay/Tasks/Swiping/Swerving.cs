using UnityEngine;

namespace InsideBuilding.Gameplay
{
	public class Swerving : Task
	{
		// [Foldout("Swerving")]
		[SerializeField] float sensitivity = 0.25f;
		
		[SerializeField] protected Transform gpx, targetInteractable;
		[SerializeField] bool updateGpxOrientation = true;
		
		[SerializeField] float gpxMovementSmooth;
		Vector3 gpxPosSmoothVel, gpxRotSmoothVel;
		
		[SerializeField] LayerMask
			toolLayer,
			interactables,
			raycastFollow;
		
		[Space()]
		public Animator anim;
		public string param;
		
		[Range(0,1)] public float progress;
		
		Vector2 input;
		const float SENSITIVITY = 0.01f;
		
		bool isGrabbingTool, isTaskCompleted;
		
		[SerializeField] Transform stinkParticle;
		bool isStinkParticleRemoved; // single-frame gate
		
		[Space()]
		[SerializeField, Tooltip("The amount of mouse speed to trigger the sound")]
		float toolSoundTrigger = 1f;
		
		[SerializeField] GeneralAudioGroupSelect audioGroup;
		
		static GeneralAudio genAudio;
		
		protected override void Awake(){
			base.Awake();
			if(!genAudio) genAudio = GeneralAudio.Instance;
		}
		
		protected override void Update(){
			base.Update();
			if(!isPlaying) return;
			
			var ray = cam.ScreenPointToRay(Input.mousePosition);
			
			GrabTool(ray);
			if(!isGrabbingTool) return;
			
			if(Input.GetMouseButton(0)){
				bool isInteracting = false;
				
				if(Physics.Raycast(ray, out var hit, 100, interactables))
					isInteracting = hit.transform == targetInteractable;
				
				if(isInteracting){
					input.x = Input.GetAxis("Mouse X");
					input.y = Input.GetAxis("Mouse Y");
					
					progress += input.magnitude * sensitivity * SENSITIVITY;
					progress = Mathf.Clamp01(progress);
					
					float triggerValue = Mathf.Pow(toolSoundTrigger, 2);
					
					if(input.sqrMagnitude >= triggerValue)
						OnProgressTrigger();
				
					if(progress > 0.65f) RemoveStinkParticle();
					if(progress >= 1f && !isTaskCompleted){
						CompleteTask();
						isTaskCompleted = true; // one frame gate
					}
				}
				
				UpdateGpx(ray, isInteracting, hit);
			}
		}
		
		protected override void LateUpdate(){
			base.LateUpdate();
			
			if(gpx) uiMgr.SetExclamationPoint(gpx.position, !isGrabbingTool);
		}
		
		public override void Play(bool b){
			base.Play(b);
			
			if(anim) anim.SetBool(param, !b);
			gpx.gameObject.SetActive(b);
		}
		
		void GrabTool(Ray ray){
			if(Input.GetMouseButtonDown(0))
				isGrabbingTool = Physics.Raycast(ray, out var hit, 100, toolLayer);
			
			if(Input.GetMouseButtonUp(0))
				isGrabbingTool = false;
		}
		
		void UpdateGpx(
			Ray ray,
			bool isInteracting,
			RaycastHit interactionInfo
		){
			if(!gpx) return;
			
			var point = gpx.position;
			var normal = gpx.up;
			
			if(isInteracting){
				point = interactionInfo.point;
				normal = interactionInfo.normal;
			}
			
			else if(Physics.Raycast(ray, out var hit, 100, raycastFollow)){
				point = hit.point;
				normal = hit.normal;
			}
			
			gpx.position = Vector3.SmoothDamp(
				gpx.position,
				point,
				ref gpxPosSmoothVel,
				gpxMovementSmooth
			);
			
			if(updateGpxOrientation)
				gpx.up = Vector3.SmoothDamp(
					gpx.up,
					normal,
					ref gpxRotSmoothVel,
					gpxMovementSmooth
				);
		}
		
		protected virtual void OnProgressTrigger(){
			if(!genAudio.isPlaying)
				genAudio.PlayRandom(audioGroup);
		}
		
		void RemoveStinkParticle(){
			if(!stinkParticle) return;
			if(isStinkParticleRemoved) return;
		
			stinkParticle.position += Vector3.up * 200f;
			isStinkParticleRemoved = true;
		}
	}
}