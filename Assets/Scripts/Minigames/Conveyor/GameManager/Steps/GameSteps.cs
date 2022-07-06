using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Gameplay;

namespace Minigame.Conveyor
{
	public partial class GameSteps : MonoBehaviour
	{
		public bool useSpeech = true;
		
		[Foldout("UI")]
		[SerializeField, LabelOverride("Instruction")] GameObject instructionUI;
		[SerializeField, LabelOverride("Header")] GameObject headerUI;
		
		[Foldout("Timer")]
		[SerializeField, LabelOverride("Duration")] float maxTimer = 60f;
		[SerializeField, LabelOverride("Image")] Image timerImg;
		
			public float MaxTimer => maxTimer;
		
		[Foldout("Item Spawn")]
		[SerializeField] float itemSpeedIncrement = 0.5f;
		
		// [SerializeField, LabelOverride("Duration")] float itemSpawnDuration;
		// [SerializeField, LabelOverride("Decrement Amount")] float itemSpawnDurationDecrement;
		
		[Foldout("Post Gameplay")]
		[SerializeField, LabelOverride("UI")] GameObject postGameplayUI;
		[SerializeField, LabelOverride("Text")] Text postGameplayTxt;
		[SerializeField] GeneralAudioSelector uiWinSound, uiLoseSound;
		
		[Space()]
		[SerializeField] GameObject[] winParticles;
		[SerializeField, LabelOverride("Spawn Point")] Transform winParticleSpawnPoint;
		
		[Space()]
		[SerializeField] GeneralAudioSelector rewardSound;
		[SerializeField] SceneLoader cityScene;
		
		float currentItemSpeed = 1f;
		IEnumerator spawnItemRoutine;
		
		public bool isPlaying, isWin;

		Speech speech;
		GameManager gameMgr;
		
		void Awake(){
			speech = Speech.Instance;
			gameMgr = GameManager.Instance;
			
			gameMgr.onScoreUpdate += SpawnNewItem;
			gameMgr.onItemBroken += SpawnNewItem;
			gameMgr.onWinning += OnWinning;
		}
		
		void Update(){
			if(useSpeech) return;
			
			if(Input.GetMouseButtonDown(0)){
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				
				if(Physics.Raycast(ray, out var hit)){
					if(hit.collider.TryGetComponent<Basket>(out var basket))
						gameMgr.SelectBasket(basket);
				}
			}
		}
		
		IEnumerator SelectBasketWithSpeech_Loop(){
			while(useSpeech && isPlaying){
				yield return GetTargetBasket();
				yield return WaitForSpeechListen();
				
				// Get the Destination Basket via Speech result
					string result = speech.result;
					var destinationBasket = gameMgr.FindBasket(result);

					if(destinationBasket)
						gameMgr.SelectBasket(destinationBasket);
				
				// Loop
					yield return null;
			}
		}
		
		IEnumerator GetTargetBasket(){
			Basket targetBasket = null;
			
			while(!targetBasket){
				targetBasket = gameMgr.targetBasket;
				yield return null;
			}
		}
		
		IEnumerator WaitForSpeechListen(){
			speech.StartListening((res)=>{ speech.StopListening(); });
			
			while(speech.isListening) 
				yield return null;
		}
		
		void SpawnNewItem(){
			var item = gameMgr.NewItem();
			
			if(item){			
				item.speed = currentItemSpeed;
				currentItemSpeed += itemSpeedIncrement;
			}
		}
		
		void OnWinning(){
			gameMgr.donePlaying = true;
			isPlaying = false;
			isWin = true;
		}
		
		public void Exit(){ cityScene.LoadAsync(); }
		public void Restart(){ SceneLoader.Current(); }
	}
}