using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class FpCtrl_Input : SceneObjectSingleton<FpCtrl_Input>,
	IPointerDownHandler,
	IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
	IPointerUpHandler
{
    #region Inspector
		[SerializeField] float dragSensitivity = 0.1f;
		
		[Space()]
		[SerializeField] Joystick moveJoystick;
		[SerializeField]private GameObject interactionPromptUI_keycodeTip;
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
				return _jumpButtonDown ? _jumpButtonDown : Input.GetButtonDown("Jump");
			}
		}
		
		public bool click{ get; private set; }
		public bool hold{ get; private set; }
		public bool drag{ get; private set; }
        public bool interactButtonClicked { get; private set; }
		public bool sprintButtonPressed { get; private set; }

        public Vector2 clickPosition => Input.mousePosition;
		public Vector2 panAxis{ get; private set; }
		
 		IEnumerator dragInfoTrackerRoutine;
		Vector2 currentPosition, prevPosition;
		
	#endregion	
	
	#region Variables
	
		bool _jumpButtonDown;
		static readonly WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
		
	#endregion
	
	#region Intialize
	
	protected override void Awake(){
		base.Awake();
			
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
		if (!Application.isMobilePlatform)
		{
			foreach (var ti in touchInputs)
				ti.SetActive(false);
		}
		else
			interactionPromptUI_keycodeTip.SetActive(false);
	}

    private void Update()
    {
        if (!Application.isMobilePlatform)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("F");
                OnTriggerInteractButton();
            }
            panAxis = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

			sprintButtonPressed = Input.GetKey(KeyCode.LeftShift);
        }

    }

    #endregion

    #region Events

    public void OnPointerDown(PointerEventData data) => hold = true;
	public void OnBeginDrag(PointerEventData data) => TrackDraggingBehaviour();
		
	public void OnDrag(PointerEventData data){
        if (!Application.isMobilePlatform)
            return;

		panAxis = data.delta * dragSensitivity * panSpeed;
		currentPosition = data.position;
	}
		
	public void OnEndDrag(PointerEventData data){
        if (!Application.isMobilePlatform)
            return;

        panAxis = Vector2.zero;
		drag = false;   
			
		if(dragInfoTrackerRoutine != null)
			StopCoroutine(dragInfoTrackerRoutine);
	}
		
	public void OnPointerUp(PointerEventData data){
        if (!Application.isMobilePlatform)
            return;

        if (!drag)
			TriggerClick();
			
		hold = false;
	}

	public void OnJumpButton()
    {
		_jumpButtonDown = true;
		StartCoroutine(reset());
			
		IEnumerator reset(){
			yield return endOfFrame;
			_jumpButtonDown = false;
		}
	}

	public void OnSprintButtonDown()
    {
		sprintButtonPressed = true;
    }

	public void OnSprintButtonUp()
    {
		sprintButtonPressed = false;
    }

    // triggered via button
    public void OnTriggerInteractButton()
    {
        interactButtonClicked = true;
        StartCoroutine(InteractButtonClickedRoutine());

        IEnumerator InteractButtonClickedRoutine()
        {
            yield return endOfFrame;
            interactButtonClicked = false;
        }
    }

    #endregion

    #region Settings

    public void OnSpeedSlider(){ PlayerPrefs.SetFloat(SPD_KEY, speed); }
		public void OnPanSlider(){ PlayerPrefs.SetFloat(PAN_KEY, panSpeed); }
		
	#endregion
	
	void TrackDraggingBehaviour(){
		Tools.StartCoroutine(ref dragInfoTrackerRoutine, routine(), this);
		
		IEnumerator routine(){
			while(true){
				drag = currentPosition != prevPosition;
				prevPosition = currentPosition;
				
				if(!drag)
					panAxis = Vector2.zero;
			
				yield return null;
			}
		}
	}
	
	void TriggerClick(){
		click = true;
		StartCoroutine(reset());
		
		IEnumerator reset(){
			yield return endOfFrame;
			click = false;
		}
	}

}