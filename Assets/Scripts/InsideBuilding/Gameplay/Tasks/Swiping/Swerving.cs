using UnityEngine;

namespace Gameplay
{
	public class Swerving : Task
	{
		[SerializeField] float sensitivity = 0.25f;
		
		[SerializeField] Transform gpx;
		[SerializeField] bool updateGpxOrientation = true;
		
		[SerializeField] float gpxMovementSmooth;
		Vector3 gpxPosSmoothVel, gpxRotSmoothVel;
		
		[SerializeField] LayerMask toolLayer, raycastFollow;
		
		[Space()]
		public Animator anim;
		public string param;
		
		[Range(0,1)]
		public float progress;
		
		Vector2 input;
		const float SENSITIVITY = 0.01f;
		
		bool isGrabbingTool;
		
		public override void Play(bool b){
			base.Play(b);
			
			if(anim) anim.SetBool(param, !b);
			gpx.gameObject.SetActive(b);
		}
		
		protected override void Update(){
			base.Update();
			if(!isPlaying) return;
			
			var ray = cam.ScreenPointToRay(Input.mousePosition);
			
			GrabTool(ray);
			if(!isGrabbingTool) return;
			
			if(Input.GetMouseButton(0)){
				input.x = Input.GetAxis("Mouse X");
				input.y = Input.GetAxis("Mouse Y");
				
				progress += input.magnitude * sensitivity * SENSITIVITY;
				progress = Mathf.Clamp01(progress);
				
				if(progress >= 1) CompleteTask();
				
				UpdateGpx();
			}
		}
		
		void GrabTool(Ray ray){
			if(Input.GetMouseButtonDown(0))
				isGrabbingTool = Physics.Raycast(ray, out var hit, 100, toolLayer);
			
			if(Input.GetMouseButtonUp(0))
				isGrabbingTool = false;
		}
		
		void UpdateGpx(){
			if(!gpx) return;
			
			var ray = cam.ScreenPointToRay(Input.mousePosition);
			
			if(Physics.Raycast(ray, out var hit, 100, raycastFollow)){
				gpx.position = Vector3.SmoothDamp(
					gpx.position,
					hit.point,
					ref gpxPosSmoothVel,
					gpxMovementSmooth
				);
				
				if(updateGpxOrientation)
					gpx.up = Vector3.SmoothDamp(
						gpx.up,
						hit.normal,
						ref gpxRotSmoothVel,
						gpxMovementSmooth
					);
			}
		}
		
		protected override void LateUpdate(){
			base.LateUpdate();
			if(gpx) uiMgr.SetExclamationPoint(gpx.position, !isGrabbingTool);
		}
	}
}