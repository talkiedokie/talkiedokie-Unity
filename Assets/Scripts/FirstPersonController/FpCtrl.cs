using UnityEngine;
using Cinemachine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class FpCtrl : MonoBehaviour
{
	#region Inspector
		
		[SerializeField] float
			speed = 1f,
			jumpForce = 2f;
		
		[Space()]
		[SerializeField] ForceMode jumpForceMode = ForceMode.Impulse;
		[SerializeField] LayerMask groundLayers;
		[SerializeField] float groundCheckRadius = 0.15f;
		
		[Space()]
		[SerializeField] float mouseSensitivity = 1f;
		[SerializeField] bool invertX, invertY;
		
		[Space()]
		[SerializeField] float moveSmoothTime = 0.12f;
		[SerializeField] float viewSmoothTime = 0.12f;
		
		[Space()]
		[SerializeField] float interactionDistance = 3f;
		[SerializeField] LayerMask clickables;
		
		[Space()]
		[SerializeField] bool enableGpx = true;
		[SerializeField] GameObject gpx;
		
		public bool autoResetTransform = true;
		
	#endregion
	
	#region Variables
		
		// Input
			Vector2 moveInput, viewInput;
			float pitch;
		
		// Output
			Vector2 movement, viewRotation;
		
		// Smoothing
			Vector2 moveSmoothVel, viewSmoothVel;
		
		bool isGrounded;
		
		Rigidbody rb;
		
		Vector3 lastPosition;
		Quaternion lastRotation;
		
		FpCtrl_Input input;
		GameObject interactionPromptUI;
		Clickable hoveredObject;
		
		CinemachineVirtualCamera camView, currentCamCache;
		Transform camT;
		Camera cam;
		CameraManager camMgr;
		
		bool hasStarted;
		
	#endregion
	
	#region Unity Methods
		
		#region Setup and Setdown
			
			void Awake(){
				input = FpCtrl_Input.Instance;
				rb = GetComponent<Rigidbody>();

				camView = GetComponentInChildren<CinemachineVirtualCamera>();
				camT = camView.transform;
				cam = Camera.main;
				camMgr = CameraManager.Instance;
				
				hasStarted = true;
			}
			
			void OnEnable(){
				if(!hasStarted) return;
				
				currentCamCache = camMgr.Current;
				camMgr?.SetPriority(camView);
				input.gameObject.SetActive(true);
				
				// fpUI
				interactionPromptUI = input.interactionPromptUI;
				
				rb.isKinematic = false;
				
				if(gpx){
					gpx.SetActive(enableGpx);
					RemoveToCamCulling();
				}
				
				if(TryGetComponent<Tweener>(out var tweener))
					tweener.enabled = false;
			}
			
			void OnDisable(){
				if(!Application.isPlaying) return;
				
				camMgr?.SetPriority(currentCamCache);
				
				rb.isKinematic = true;
				
				if(gpx){
					gpx.SetActive(true);
					camMgr?.AddCullingLayer(gpx.layer);
				}
				
				if(input)
					input.gameObject.SetActive(false);
			}
			
		#endregion
		
		#region Update Loops
			
			void Update(){
				HandleMovement();
				HandleViewRotation();
				
				HandleGroundChecking();
				
				if(input.jumpButtonDown)
					HandleJumping();
				
				if(input.click)
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
			
			void LateUpdate(){
				if(!input.gameObject.activeSelf)
					input.gameObject.SetActive(true);
				
				HandleHighlighting();
			}
			
			void OnDrawGizmosSelected(){
				Gizmos.color = Color.green;
				Gizmos.DrawWireSphere(transform.position, groundCheckRadius);
			}
			
		#endregion
		
	#endregion
	
	#region Logic
		
		void HandleMovement(){
			// Input
				moveInput = Vector2.ClampMagnitude(input.moveAxis, 1f);
				moveInput *= speed;
			
			// Smoothen
				movement = Vector2.SmoothDamp(
					movement,
					moveInput,
					ref moveSmoothVel,
					moveSmoothTime
				);
		}
		
		void HandleViewRotation(){			
			// Input
				viewInput.x += invertY? -input.panAxis.y: input.panAxis.y;
				viewInput.y = invertX? -input.panAxis.x: input.panAxis.x;
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
				camT.localEulerAngles = Vector3.left * viewRotation.x;
				transform.Rotate(Vector3.up * viewRotation.y);
		}
		
		void HandleGroundChecking(){
			isGrounded = Physics.CheckSphere(
				transform.position,
				groundCheckRadius,
				groundLayers
			);
		}
		
		void HandleJumping(){
			if(isGrounded)
				rb.AddForce(Vector3.up * jumpForce, jumpForceMode);
		}
		
	#endregion
	
	#region Interactions
		
		void HandleHighlighting(){
			// var ray = new Ray(camT.position, camT.forward);
			var ray = cam.ScreenPointToRay(input.clickPosition);
			bool raycast = Physics.Raycast(ray, out var hit, interactionDistance, clickables);
			
			if(raycast){
				if(hit.collider.TryGetComponent<Clickable>(out var current)){
					hoveredObject?.ShowHighlight(false);
					hoveredObject = current;
					
					Highlight(true);
				}
				else Highlight(false);
			} else Highlight(false);
		}
		
		void Highlight(bool b){
			hoveredObject?.ShowHighlight(b);
			
			if(interactionPromptUI)
				interactionPromptUI.SetActive(b);
		}
		
		void HandleInteraction(){
			var ray = cam.ScreenPointToRay(input.clickPosition);
			
			if(Physics.Raycast(ray, out var hit, interactionDistance, clickables)){
				var interactable = hit.collider.GetComponent<Clickable>();
					interactable?.Interact();
			}
		}
		
	#endregion
	
	IEnumerator removeCullingLayer;
	
	void RemoveToCamCulling(){
		Tools.StartCoroutine(ref removeCullingLayer, routine(), this);
		
		IEnumerator routine(){
			yield return new WaitForSeconds(2f);
			camMgr?.RemoveCullingLayer(gpx.layer);
		}
	}
}