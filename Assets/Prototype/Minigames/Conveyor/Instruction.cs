using UnityEngine;

namespace Minigame{
	namespace Conveyor
	{
		public class Instruction : MonoBehaviour
		{
			[SerializeField] GameObject prevButton, nextButton, startButton;
			
			public void OnPageUpdate(float progress){
				if(progress == 0f){
					prevButton.SetActive(false);
					nextButton.SetActive(true);
					startButton.SetActive(false);
				}
				
				else if(progress > 0f && progress < 1f){
					prevButton.SetActive(true);
					nextButton.SetActive(true);
					startButton.SetActive(false);
				}
				
				else if(progress == 1f){
					prevButton.SetActive(true);
					nextButton.SetActive(false);
					startButton.SetActive(true);
				}
			}
			
			public void OnStartButton(){
				Time.timeScale = 1f;
				UIManager.Instance.Hide(gameObject);
			}
		}
	}
}