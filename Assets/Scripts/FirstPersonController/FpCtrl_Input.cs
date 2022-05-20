using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class FpCtrl_Input : SceneObjectSingleton<FpCtrl_Input>,
	IPointerDownHandler,
	IDragHandler,
	IEndDragHandler,
	IPointerUpHandler
{
	#region Inspector
	
		[SerializeField] Joystick moveJoystick;
		public GameObject interactionPromptUI;
		
		[Space()]
		[SerializeField] GameObject[] touchInputs;
		
		// Slider UI
			[SerializeField] Slider speedSlider, panSpeedSlider;
			
			public float speed{ get; set; } = 1f;
			public float panSpeed{ get; set; }  = 1f;
			
			const string
				SPD_KEY = "playerSpeed",
				PAN_KEY = "panSpeed";
		
	#endregion
	
	#region Properties
		
		public Vector2 moveAxis{
			get{
				var axis = moveJoystick.axis;
				
				var _axis = new Vector2(
					axis.x != 0f? axis.x: Input.GetAxis("Horizontal"),
					axis.y != 0f? axis.y: Input.GetAxis("Vertical")
				);
				
				return _axis * speed;
			}
		}
		
		public bool jumpButtonDown{
			get{
				return
					_jumpButtonDown? _jumpButtonDown:
					Input.GetButtonDown("Jump");
			}
		}
		
		public bool click{ get; private set; }
		public bool hold{ get; private set; }
		public bool drag{ get; private set; }
		
		public Vector2 clickPosition{ get; private set; }
		public Vector2 panAxis{ get; private set; }
		
	#endregion	
	
	#region Variables
		
		const float SENSITIVITY = 0.1f;
		
		bool _jumpButtonDown;
		static WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
		
	#endregion
	
	#region Intialize
	
		void Awake(){
			speed = PlayerPrefs.GetFloat(SPD_KEY);
			panSpeed = PlayerPrefs.GetFloat(PAN_KEY);
			
			if(speed == 0f){
				speed = 1f;
				PlayerPrefs.SetFloat(SPD_KEY, speed);
			}
			
			if(panSpeed == 0f){
				panSpeed = 1f;
				PlayerPrefs.SetFloat(PAN_KEY, panSpeed);
			}
			
			if(speedSlider) speedSlider.SetValueWithoutNotify(speed);
			if(panSpeedSlider) panSpeedSlider.SetValueWithoutNotify(panSpeed);
		}
		
		void OnEnable(){
			#if UNITY_WINDOWS
				
				foreach(var ti in touchInputs)
					ti.SetActive(false);
				
			#endif
		}
		
	#endregion
	
	#region Events
		
		public void OnPointerDown(PointerEventData data){
			hold = true;
		}
		
		public void OnDrag(PointerEventData data){
			panAxis = data.delta * SENSITIVITY * panSpeed;
			clickPosition = data.position;
			
			drag = true;
		}
		
		public void OnEndDrag(PointerEventData data){
			panAxis = Vector2.zero;
			drag = false;
		}
		
		public void OnPointerUp(PointerEventData data){
			clickPosition = data.position;
			
			if(!drag){
				click = true;
				StartCoroutine(reset());
				
				IEnumerator reset(){
					yield return endOfFrame;
					click = false;
				}
			}
			
			hold = false;
		}
		
		public void OnJumpButton(){
			_jumpButtonDown = true;
			StartCoroutine(reset());
			
			IEnumerator reset(){
				yield return endOfFrame;
				_jumpButtonDown = false;
			}
		}
		
	#endregion
	
	#region Settings
	
		public void OnSpeedSlider(){ PlayerPrefs.SetFloat(SPD_KEY, speed); }
		public void OnPanSlider(){ PlayerPrefs.SetFloat(PAN_KEY, panSpeed); }
		
	#endregion
}