using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(Rigidbody))]
public class FpCtrl : MonoBehaviour
{
	#region Inspector
		
		public float
			speed = 1f,
			jumpForce = 2f,
			mouseSensitivity = 1f;
			
		public ForceMode jumpForceMode = ForceMode.Impulse;
		
		[Space()]
		public float moveSmoothTime = 0.12f;
		public float viewSmoothTime = 0.12f;
		
		[Space()]
		public float interactionDistance = 3f;
		public LayerMask clickables;
		
	#endregion
	
	#region Variables
		
		// Input
			Vector2 moveInput, viewInput;
			float pitch;
		
		// Output
			Vector2 movement, viewRotation;
		
		// Smoothing
			Vector2 moveSmoothVel, viewSmoothVel;
		
		GameObject ui;
		Rigidbody rb;
		
		CinemachineVirtualCamera camView, currentCamCache;
		Transform cam;
		CameraManager camMgr;
		
	#endregion
	
	#region Unity Methods
		
		void Awake(){
			rb = GetComponent<Rigidbody>();

			camView = GetComponentInChildren<CinemachineVirtualCamera>();
			cam = camView.transform;
			camMgr = CameraManager.Instance;
		}
		
		void OnEnable(){
			currentCamCache = camMgr.Current;
			camMgr.SetPriority(camView);
			
			CustomCursor.Instance.Hide();
			
			// fpUI
			if(!ui) ui = GameObject.Find("/Canvas/Overlays/FirstPerson");
			if(ui) ui.SetActive(true);
			
			rb.isKinematic = false;
		}
		
		void Update(){
			HandleMovement();
			HandleJumping();
			HandleViewRotation();
			
			if(Input.GetMouseButtonDown(0))
				HandleInteraction();
		}
		
		void FixedUpdate(){
			// Apply Movement
				var velocity = new Vector3(
					movement.x, 0f, movement.y
				);
				
				velocity = transform.TransformDirection(velocity);
				velocity.y = rb.velocity.y;
				
				rb.velocity = velocity;
		}
		
		void OnDisable(){
			camMgr.SetPriority(currentCamCache);
			if(ui) ui.SetActive(false);
			
			rb.isKinematic = true;
		}
		
	#endregion
	
	#region Logic
		
		void HandleMovement(){
			// Input
				moveInput.x = Input.GetAxisRaw("Horizontal");
				moveInput.y = Input.GetAxisRaw("Vertical");
				
				moveInput.Normalize();
				moveInput *= speed;
			
			// Smoothen
				movement = Vector2.SmoothDamp(
					movement,
					moveInput,
					ref moveSmoothVel,
					moveSmoothTime
				);
		}
		
		void HandleJumping(){
			if(Input.GetButtonDown("Jump"))
				rb.AddForce(Vector3.up * jumpForce, jumpForceMode);
		}
		void HandleViewRotation(){			
			// Input
				viewInput.x += Input.GetAxis("Mouse Y");
				viewInput.y = Input.GetAxis("Mouse X");
				viewInput *= mouseSensitivity;
			
			// Smoothen
				viewRotation.x = Mathf.SmoothDamp(
					viewRotation.x,
					viewInput.x,
					ref viewSmoothVel.x,
					viewSmoothTime
				);
				
				viewRotation.y = Mathf.SmoothDamp(
					viewRotation.y,
					viewInput.y,
					ref viewSmoothVel.y,
					viewSmoothTime
				);
			
			// Apply
				cam.localEulerAngles = Vector3.left	* viewRotation.x;
				transform.Rotate(Vector3.up * viewRotation.y);
		}
		
		void HandleInteraction(){
			var ray = new Ray(cam.position, cam.forward);
				
			if(Physics.Raycast(ray, out var hit, interactionDistance, clickables))
				if(hit.collider.TryGetComponent<Clickable>(out var clickable))
					clickable.Interact();
		}
		
	#endregion
}