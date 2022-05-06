using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using System.Collections;

public class SpeechRecognizerUI : MonoBehaviour
{
	[SerializeField] Image micIcon, micIconTimer;
	IEnumerator listenTimer;
	
	[SerializeField] Text resultTxt, gradeTxt;
	[SerializeField] Gradient correctColor;
		
		Color listeningColor => correctColor.Evaluate(0.5f);
		Color wrongColor => correctColor.Evaluate(0f);
	
	[SerializeField] GameObject loading, warningIcon;
	
	[Foldout("Audio")]
	[SerializeField] bool playAudio = true;
	[SerializeField] GeneralAudioSelector
		popSound = 5,
		happySound = 4,
		sadSound = 7;
	
	Animator pop;
	int trigger = Animator.StringToHash("pop");
	
	GeneralAudio genAudio;
	
	void Awake(){
		pop = micIcon.GetComponent<Animator>();
		genAudio = GeneralAudio.Instance;
	}
	
	#region Calls
		
		public void OnHypothesis(){
			loading.SetActive(true);
		}
		
		public void OnListen(float listenDuration){
			micIcon.gameObject.SetActive(true);
			micIcon.color = listeningColor;
			
			// Reset
				gradeTxt.gameObject.SetActive(false);
				resultTxt.gameObject.SetActive(false);
				warningIcon.SetActive(false);
			
			gameObject.SetActive(true);
			
			pop.SetTrigger(trigger);
			genAudio.PlayAdditive(popSound);
			
			ListenDurationCountdown(listenDuration);
		}
		
		public void OnCompletion(
			DictationCompletionCause completionCause,
			string result,
			bool isCorrect,
			float grade
		){
			if(completionCause == DictationCompletionCause.Complete){
				if(isCorrect){
					var color = correctColor.Evaluate(grade);
					
					micIcon.color = color;
					gradeTxt.color = color;
					
					gradeTxt.text = Mathf.Round(grade * 100) + "%";
					gradeTxt.gameObject.SetActive(true);
					
					if(playAudio) genAudio.PlayAdditive(happySound);
				}
				
				else micIcon.color = wrongColor;
				
				pop.SetTrigger(trigger);
			}
			else{
				result = Tools.AddStringSpaces(completionCause.ToString());
				
				micIcon.gameObject.SetActive(false);
				warningIcon.SetActive(true);
				
				if(playAudio) genAudio.PlayAdditive(sadSound);
			}
			
			resultTxt.text = result;
			resultTxt.gameObject.SetActive(true);
			loading.gameObject.SetActive(false);
			
			StopListenDurationCountdown();
		}
		
		public void OnSkip(){
			micIcon.gameObject.SetActive(false);
			warningIcon.SetActive(true);
			
			resultTxt.text = "Skipped";
			resultTxt.gameObject.SetActive(true);
			
			loading.gameObject.SetActive(false);
			if(playAudio) genAudio.PlayAdditive(sadSound);
		}
		
	#endregion
	
	void ListenDurationCountdown(float duration){
		StopListenDurationCountdown();
		
		listenTimer = routine(duration);
		StartCoroutine(listenTimer);
		
		IEnumerator routine(float duration){
			float timer = duration;
			
			while(timer > 0){
				float percent = Mathf.Clamp01(timer / duration);
				micIconTimer.fillAmount = percent;
				
				timer -= Time.deltaTime;
				
				yield return null;
			}
		}
	}
	
	void StopListenDurationCountdown(){
		if(listenTimer != null)
			StopCoroutine(listenTimer);
	}
}