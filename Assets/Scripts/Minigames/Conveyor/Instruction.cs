using UnityEngine;
using UnityEngine.UI;

public class Instruction : MonoBehaviour
{
	[SerializeField] GameObject
		prevButton,
		nextButton,
		startButton;
	
	[SerializeField] UIManager uiMgr;
	
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
		uiMgr?.Hide(gameObject);
	}
}