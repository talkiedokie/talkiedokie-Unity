using System.Collections;
using UnityEngine;

namespace Minigame{
	namespace Conveyor
	{
		public partial class GameSteps : MonoBehaviour
		{
			IEnumerator Start(){
				var threads = new IEnumerator[]{
					Instruction(),
					KickStart(),
					Timer(),
					Delay(),
					DecideForWinLose(),
					Result()
				};
				
				foreach(var thread in threads)
					yield return thread;
			}
			
			IEnumerator Instruction(){
				UIManager.Instance.Show(instructionUI);
				Time.timeScale = 0f;
				
				while(instructionUI.activeSelf) yield return null;
			}
			
			IEnumerator KickStart(){
				yield return new WaitForSeconds(0.5f);
					headerUI.SetActive(true);
					GeneralAudio.Instance.Play("pop");
				
				yield return new WaitForSeconds(0.5f);
					isPlaying = true;
					
					gameMgr.NewItem();
					StartCoroutine(SelectBasketWithSpeech_Loop());
			}
			
			IEnumerator Timer(){
				float timer = maxTimer;
				float itemSpawnTimer = 0f;
				
				while(timer > 0f && isPlaying){
					timerImg.fillAmount = timer / maxTimer;
					
					float deltaTime = Time.deltaTime;
					timer -= deltaTime;
					itemSpawnTimer += deltaTime;
					
					if(itemSpawnTimer >= itemSpawnDuration){
						gameMgr.NewItem();
						itemSpawnTimer = 0f;
					}
					
					yield return null;
				}
			}
			
			IEnumerator Delay(){
				speech.StopListening();
				speech.FinishUsing();
				
				yield return new WaitForSeconds(1f);
			}
			
			IEnumerator DecideForWinLose(){
				if(isWin){
					Instantiate(
						Tools.Random(winParticles),
						winParticleSpawnPoint.position,
						winParticleSpawnPoint.rotation
					);
					
					rewardSound.PlayAdditive();
					
					yield return new WaitForSeconds(1f);
					
					var coinHUD = CoinHUD.Instance;
						coinHUD.AddCoin(gameMgr.coinRewards);
						while(coinHUD.isUpdatingAmount) yield return null;
					
					postGameplayTxt.text = "YOU WIN";
					postGameplayTxt.color = Color.green;
					
					uiWinSound.Play();
				}
				
				else{
					postGameplayTxt.text = "YOU LOSE";
					postGameplayTxt.color = Color.red;
					
					uiLoseSound.Play();
				}
			}
			
			IEnumerator Result(){
				UIManager.Instance.Show(postGameplayUI);
				yield return null;
			}
		}
	}
}