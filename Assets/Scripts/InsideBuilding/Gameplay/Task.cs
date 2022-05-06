using UnityEngine;
using Gameplay;
using Cinemachine;

public class Task : MonoBehaviour
{
	[TextArea()] public string description;
	
	[Foldout("General")]
	public int rewards = 10;
	
	public AudioClip fairyVoiceClip;
	[SerializeField] CinemachineVirtualCamera camView;
	
	[Foldout("Recap")]
	[SerializeField] CinemachineVirtualCamera recapCam;
	[SerializeField] Animator recapAnim;
	[SerializeField] string recapAnimParam = "recap";
	
	[HideInInspector]
	public bool isFinished;
	
	protected static GameManager gameMgr;
	protected static GameObject fairy, player;
	
	protected static CameraManager camMgr;
	protected static Camera cam;
	
	protected static UIManager uiMgr;
	
	protected bool isPlaying;
	
	protected virtual void Awake(){
		if(!camMgr) camMgr = CameraManager.Instance;
		if(!cam) cam = Camera.main;
		if(!gameMgr) gameMgr = GameManager.Instance;
		if(!uiMgr) uiMgr = UIManager.Instance;
	}
	
	protected virtual void Start(){
		if(!fairy) fairy = Fairy.Instance.gameObject;
		if(!player) player = gameMgr.player;
		
		if(!recapCam){
			var child = transform.Find("CM taskRecap");
			
			if(child)
				recapCam = child.GetComponent<CinemachineVirtualCamera>();
		}
	}
	
	public virtual void Play(bool b){
		isPlaying = b;
		
		if(b){
			if(camView){
				camMgr.SetPriority(camView);
				GeneralAudio.Instance.Play("slide");
			}
		}
	}
	
	protected virtual void OnEnable(){}
	
	protected void CompleteTask(){
		gameMgr.FinishTask();
		
		if(recapAnim)
			recapAnim?.SetTrigger(recapAnimParam);
		
		camMgr.SetPriority(recapCam);
		Play(false);
	}
	
	protected virtual void Update(){
		if(Input.GetKeyDown("p")) CompleteTask();
	}
	
	protected virtual void LateUpdate(){}
}