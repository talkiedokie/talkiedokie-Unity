using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Minigame{
	namespace Conveyor
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
			
			[Foldout("Item Spawn")]
			[SerializeField, LabelOverride("Duration")] float itemSpawnDuration;
			[SerializeField, LabelOverride("Decrement Amount")] float itemSpawnDurationDecrement;
			
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
			
			bool isPlaying, isWin;
			
			SpeechRecognizer speechRecognizer;
			GameManager gameMgr;
			
			void Awake(){
				speechRecognizer = SpeechRecognizer.Instance;
				gameMgr = GameManager.Instance;
				
				gameMgr.onScoreUpdate += OnScoreUpdate;
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
					// Get Target Basket
						Basket targetBasket = null;
						
						while(!targetBasket){
							targetBasket = gameMgr.targetBasket;
							yield return null;
						}
						
						speechRecognizer.Listen(null);
						
						while(speechRecognizer.isListening) 
							yield return null;
						
					// Get the Destination Basket via Speech result
						string result = speechRecognizer.result;
						var destinationBasket = gameMgr.FindBasket(result);
							
						if(destinationBasket){
							gameMgr.SelectBasket(destinationBasket);
							speechRecognizer.Stop();
						}
					
					// Loop
						yield return null;
				}
			}
			
			void OnScoreUpdate(){
				itemSpawnDuration -= itemSpawnDurationDecrement;
				
				itemSpawnDuration = Mathf.Clamp(
					itemSpawnDuration,
					gameMgr.itemSpawnDelay,
					float.MaxValue
				);
			}
			
			void OnWinning(){
				isPlaying = false;
				isWin = true;
			}
			
			public void Exit(){ cityScene.LoadAsync(); }
			public void Restart(){ SceneLoader.Current(); }
		}
	}
}