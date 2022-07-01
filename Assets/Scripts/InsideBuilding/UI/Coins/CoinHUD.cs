using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using AccountsManagement;

public class CoinHUD : SceneObjectSingleton<CoinHUD>
{	
	public float hideTimerDuration = 5f;
	IEnumerator hideTimerRoutine;
	
	public Text amountTxt, onValChangeTxt;
	public Animator anim;
	int pop = Animator.StringToHash("pop");
	
	int onValChangeCounter;
	IEnumerator onValChangeRoutine;
	
	GeneralAudio genAudio;
	public GeneralAudioSelector coinSound = 13;
	
	int amount{
		get => currentUser.coins;
		set => currentUser.coins = value;
	}
	
	public bool isUpdatingAmount{ get; private set; }
	
	User currentUser;
	
	[Foldout("Popup")]
	[LabelOverride("Template")] public GameObject coinPopupTemplate;
	[LabelOverride("Start Pool Size")] public int coinPopupStartPoolSize = 25;
	[LabelOverride("Spawn Offset")] public float coinPopupSpawnOffset = 25f;
	
	[Space()]
	[LabelOverride("Travel Curve")] public AnimationCurve coinPopupTravelCurve;
	[LabelOverride("Travel Destination")] public Transform coinPopupTravelDestination;
	[LabelOverride("Start Delay")] public float coinPopupAnimTravelDelay = 0.25f;
	[LabelOverride("Travel Time")] public float coinPopupAnimTravelTime = 1.5f;
	
	[LabelOverride("Multiple Add: delay for each popup")] public float coinPopupMultipleAddDelayForeach = 0.1f;
	
	Queue<GameObject> coinPopups = new Queue<GameObject>();
	
	public void Pool(GameObject coinPopup){
		if(!coinPopups.Contains(coinPopup)){
			coinPopups.Enqueue(coinPopup);
			
			coinPopup.transform.position = coinPopupTemplate.transform.position;
			coinPopup.SetActive(false);
		}
	}
	
	protected override void Awake(){
		base.Awake();
		
		var templateParent = coinPopupTemplate.transform.parent;
		bool instantiateInWorld = true;
		
		for(int i = 0; i < coinPopupStartPoolSize; i++){
			var coinPopup = Instantiate(
				coinPopupTemplate,
				templateParent,
				!instantiateInWorld
			);
			
			Pool(coinPopup);
			coinPopup.SetActive(false);
		}
		
		CoinAnimator.onDestinationReached = UpdateAmount;
		
		genAudio = GeneralAudio.Instance;
		currentUser = AccountManager.Instance.CurrentUser();
	}
	
	void Start(){
		amountTxt.text = amount.ToString();
	}
	
	#region Calls
		
		[ContextMenu("Add Coin")]
		public void AddCoin(){
			gameObject.SetActive(true);
			{
				var coinPopup = coinPopups.Dequeue();
					coinPopup.SetActive(true);
					
				var position = coinPopup.transform.position;
					position.x += RandomOffset();
					position.y += RandomOffset();
				
					coinPopup.transform.position = position;
				
				// amount ++;
				
				float RandomOffset(){
					return Random.Range(-coinPopupSpawnOffset, coinPopupSpawnOffset);
				}
			}
			
			isUpdatingAmount = true;
			ResetHideTimer();
		}
		
		public void AddCoin(int amount){
			gameObject.SetActive(true);
			
			StartCoroutine(NewMultipleCoinsRoutine());
			
			IEnumerator NewMultipleCoinsRoutine(){
				var step = new WaitForSeconds(coinPopupMultipleAddDelayForeach);
				
				for(int i = 0; i < amount; i++){
					AddCoin();
					yield return step;				
				}
			}
		}
		
	#endregion
	
	void UpdateAmount(){
		amount ++;
		{
			amountTxt.text = amount.ToString();
			anim.SetTrigger(pop);
		}
		
		onValChangeTxt.gameObject.SetActive(true);
		{
			onValChangeCounter ++;
			onValChangeTxt.text = "+" + onValChangeCounter;
			
			Tools.StartCoroutine(ref onValChangeRoutine, OnValChangeRoutine(), this);
		}
		
		genAudio.Play(coinSound);
	}
	
	IEnumerator OnValChangeRoutine(){
		yield return new WaitForSeconds(1.5f);
		
		onValChangeTxt.gameObject.SetActive(false);
		onValChangeCounter = 0;
		
		isUpdatingAmount = false;
	}
	
	void ResetHideTimer(){
		Tools.StartCoroutine(ref hideTimerRoutine, HideTimerRoutine(), this);
	}
	
	IEnumerator HideTimerRoutine(){
		yield return new WaitForSeconds(hideTimerDuration);
		gameObject.SetActive(false);
	}
}