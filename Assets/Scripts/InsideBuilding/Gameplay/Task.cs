using UnityEngine;
using Cinemachine;
using System.Collections;

namespace InsideBuilding
{
	using Gameplay;
	
	public class Task : MonoBehaviour
	{
		[SerializeField] Description[] descriptions;
		public Description description{ get; private set; }
		
		// [Foldout("General")]
		public int rewards = 10;
		public bool autoCompleteDebug;
		
		public Transform charPoint;
			Transform playerDefaultParent;
			Vector3 playerDefaultPos;
			Quaternion playerDefaultRot;
		
		[SerializeField] CinemachineVirtualCamera camView;
		
		// [Foldout("Recap")]
		[SerializeField] CinemachineVirtualCamera recapCam;
		[SerializeField] Animator recapAnim;
		[SerializeField] string recapAnimParam = "recap";
		
		public bool isCanceled{ get; private set; }
		
		protected Room room;
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
		
		public void RephraseDescription() =>
			description = Tools.Random(descriptions);
		
		public void SetRoom(Room room){ this.room = room; }
		
		public virtual void Play(bool b){
			isPlaying = b;
			
			if(b){
				if(camView){
					camMgr.SetPriority(camView);
					GeneralAudio.Instance.Play("slide");
				}
				
				RepositionPlayer();
				
				if(autoCompleteDebug)
					StartCoroutine(AutoComplete());
			}
		}
		
		protected virtual void OnEnable(){}
		
		protected void CompleteTask(){
			OnTaskCompleted();
			gameMgr.FinishTask();
			
			if(recapAnim)
				recapAnim?.SetTrigger(recapAnimParam);
			
			camMgr.SetPriority(recapCam);
			Play(false);
		}
		
		protected virtual void OnTaskCompleted(){}
		
		public void CancelTask(){
			isCanceled = true;
			
			OnTaskCompleted();
			gameMgr.FinishTask();
			
			Play(false);
		}
		
		protected virtual void Update(){
			if(Input.GetKeyDown("p")) CompleteTask();
		}
		
		protected virtual void LateUpdate(){}
		
		void RepositionPlayer(){
			if(!charPoint) return;
			
			var player = gameMgr.player.transform;
				playerDefaultParent = player.parent;
				playerDefaultPos = player.localPosition;
				playerDefaultRot = player.localRotation;
				
				player.parent = null;
			
			var tweener = player.GetComponent<Tweener>();
				if(tweener) tweener.SetTarget(charPoint);
		}
		
		public void ResetPlayerPosition(){
			if(!charPoint) return;
			
			var player = gameMgr.player.transform;
			var tweener = player.GetComponent<Tweener>();
				tweener.enabled = false;
				
				player.parent = playerDefaultParent;
				player.localPosition = playerDefaultPos;
				player.localRotation = playerDefaultRot;
		}
		
		IEnumerator AutoComplete(){
			yield return new WaitForSeconds(3f);
			CompleteTask();
		}
		
		[System.Serializable]
		public struct Description{
			[TextArea()]
			public string text;
			public AudioClip fairyVoiceClip;
			
			public Description(string txt, AudioClip clip){
				text = txt;
				fairyVoiceClip = clip;
			}
		}
		
		/* [ContextMenu("Yeah")]
		public void Yeah(){
			descriptions = new Description[]{ new Description(description, fairyVoiceClip) };
			
			UnityEditor.EditorUtility.SetDirty(this);
		} */
	}
}