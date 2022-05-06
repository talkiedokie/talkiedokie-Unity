using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using AccountsManagement;

namespace Minigame
{
	public class BunnyPancake : MonoBehaviour
	{
		#region Inspector
			
			public Character[] characters;
			
			public GameObject instructionUI;
			public GameObject header;
			
			[Foldout("Timer")]
				[LabelOverride("Amount")] public float _timer = 10f;
				[LabelOverride("Correct Answer Reward")] public float timeRecovery = 1.25f;
				[LabelOverride("Text UI")] public Text timerTxt;
					
					float timer;
					bool isHypothizing;
				
				public GeneralAudioSelector timesUpSound = 13;
				
				[Space()]
				[LabelOverride("Image Fill")] public Image timerImg;
				[LabelOverride("Gradient")] public Gradient timerGradient;
			
			[Foldout("Score")]
				[LabelOverride("Amount")] public int score;
				[LabelOverride("Max Amount")] public int maxScore = 10;
				
				[LabelOverride("Text UI")] public Text scoreTxt;
				
				public GeneralAudioSelector
					correctSound = 6,
					wrongSound = 7,
					moveSound = 8;
				
				[Space()]
				public GameObject[] correctParticles;
				[LabelOverride("Despawn Time")] public float correctParticlesDespawnTime = 2f;
			
			[Foldout("Post Gameplay")]
				public int coinRewards = 20;
				
				[LabelOverride("UI Object")]
				public GameObject postGameplayUi;
				public Text winLoseTxt;
				
				public GameObject[] particles;
				
				public GeneralAudioSelector
					winSound = 14,
					postGameplaySound = 1;
				
				[Space()]
				public SceneLoader exitScene;
			
		#endregion
		
		#region Initializations
			
			Animator headerAnim;
			int pop = Animator.StringToHash("pop");
			
			SpeechRecognizer speechRecognizer;
			UIManager uiMgr;
			
			void Awake(){
				speechRecognizer = SpeechRecognizer.Instance;
				speechRecognizer.onHypothesis = onHypothesis;
				
				uiMgr = UIManager.Instance;
				
				headerAnim = scoreTxt.GetComponentInParent<Animator>();
				
				foreach(var character in characters)
					character.OnAwake();
			}
			
			void onHypothesis(){ isHypothizing = true; }
			
			IEnumerator Start(){
				yield return null; // late start
				
				uiMgr.Show(instructionUI);
				header.SetActive(false);
			}
			
		#endregion
		
		#region Steps
			
			public void OnPlayButton(){
				StartCoroutine(Play());
				
				uiMgr.Hide(instructionUI);
				uiMgr.Transition();
			}
			
			IEnumerator Play(){
				// Step1
				yield return new WaitForSeconds(1f);
					NewChallenge();
					header.SetActive(true);
				
				// Step2
				timer = _timer;
				bool isWin = false;
				
				while(timer > 0f){
					if(!isHypothizing){
						float lerp = timer / _timer;
						var color = timerGradient.Evaluate(lerp);
						{
							timerImg.fillAmount = lerp;
							timerImg.color = color;
							
							timerTxt.text = (Mathf.Round(timer * 10) / 10).ToString();
							timerTxt.color = color;
						}
						
						timer -= Time.deltaTime;
					}
					
					yield return null;
					
					isWin = score >= maxScore;
					if(isWin) break;
				}
				
				// Step3
				yield return new WaitForSeconds(0.35f);
				
					speechRecognizer.Stop();
					
					Pop(headerAnim);
					timesUpSound.PlayAdditive();
				
				// Step4
				yield return new WaitForSeconds(1f);
				
					if(isWin){
						var particle = Tools.Random(particles);
							particle.SetActive(true);
							winSound.Play();
						
						yield return new WaitForSeconds(1f);
							
							var coinHUD = CoinHUD.Instance;
							{
								coinHUD.AddCoin(coinRewards);
								while(coinHUD.isUpdatingAmount) yield return null;
							}
					}
				
				// Step5
				yield return new WaitForSeconds(2f);
				
					winLoseTxt.text = isWin? "YOU WIN": "YOU LOSE";
					
					winLoseTxt.color = isWin?
						timerGradient.Evaluate(1f):
						timerGradient.Evaluate(0f);
					
					uiMgr.Show(postGameplayUi);
					postGameplaySound.PlayAdditive();
			}
			
		#endregion
		
		#region Challenge
			
			public void NewChallenge(){
				var character = Tools.Random(characters);
					var food = character.InstantiateFood();
				
				speechRecognizer.Listen(character.name, onListen);
				speechRecognizer.listenAgainOnWrong = false;
				
				// speechRecognizer.onResult = onListen;
				
				void onListen(){
					if(timer <= 0f) return;
					
					string result = speechRecognizer.result;
					var destChar = Array.Find(characters, character => character.name.ToLower() == result);
					
					if(destChar != null){
						var routine = MoveTheFood(food.transform, destChar, character);
							StartCoroutine(routine);
					}else{
						speechRecognizer.Listen(character.name, onListen);
						speechRecognizer.listenAgainOnWrong = false;
					}
				}
			}
			
			IEnumerator MoveTheFood(
				Transform food,
				Character destination,
				Character correct
			){			
				float timer = 0f;
				float duration = 0.35f;
				
				var start = food.position;
				var dest = destination.transform.position;
				
				moveSound.Play();
				
				while(timer < duration){
					float lerp = timer / duration;
					food.position = Vector3.Lerp(start, dest, lerp);
					
					timer += Time.deltaTime;
					yield return null;
				}
				
				bool isCorrect = destination == correct;
					UpdateScore(destination, isCorrect);
				
				Destroy(food.gameObject);
				NewChallenge();
			}
			
		#endregion
		
		#region Score
			
			public void UpdateScore(Character character, bool isCorrect){
				if(isCorrect){
					score ++;
					scoreTxt.text = score.ToString();
					
					Pop(headerAnim);
					
					var particle = Instantiate(
						Tools.Random(correctParticles),
						character.transform.position,
						Quaternion.identity
					);
					
					Destroy(particle, correctParticlesDespawnTime);
					
					timer += timeRecovery;
				}
				
				Pop(character.animator);
				GeneralAudio.Instance.Play(isCorrect? correctSound: wrongSound);
				
				isHypothizing = false;
			}
			
			void Pop(Animator anim){
				anim.SetTrigger(pop);
			}
			
		#endregion
		
		#region Post Gameplay
		
			public void OnRestartButton(){ SceneLoader.Current(); }
			public void OnExitButton(){ exitScene.Load(); }
			
		#endregion
		
		[System.Serializable]
		public class Character{
			public string name;
			
			public GameObject gameObject;
			public GameObject food;
			
			public Transform transform{ get; private set; }
			public Animator animator{ get; private set; }
			
			public void OnAwake(){
				transform = gameObject.transform;
				animator = gameObject.GetComponent<Animator>();
			}
			
			public GameObject InstantiateFood(){
				var food = Instantiate(this.food);
					food.SetActive(true);
				
				return food;
			}
		}
	}
}