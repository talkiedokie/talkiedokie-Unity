using UnityEngine;
using UnityEngine.UI;

public class SpeechRecognizerUI : MonoBehaviour
{
	[SerializeField] Image icon;
	[SerializeField] Gradient gradient;
	
		Color listenColor => gradient.Evaluate(0.5f);
		Color correctColor => gradient.Evaluate(1f);
		Color wrongColor => gradient.Evaluate(0f);
	
	[Space()]
	[SerializeField] Image timerImg;
	[SerializeField] Text txt;
	
		const FontStyle
			BOLD = FontStyle.Bold,
			ITAL = FontStyle.Italic;
	
	[Space()]
	[SerializeField] GameObject progressObj;
	[SerializeField] GameObject warningObj;
	
	const SpeechRecognizer.ResultType
		CORRECT = SpeechRecognizer.ResultType.Correct,
		WRONG = SpeechRecognizer.ResultType.Wrong,
		TIMEOUT = SpeechRecognizer.ResultType.TimeOut;
	
	public void OnListen(){
		warningObj.SetActive(false);
		
		icon.gameObject.SetActive(true);
		icon.color = listenColor;
		
		txt.text = "";
		progressObj.SetActive(false);
	}
	
	public void OnListenTimer(float percent){
		percent = Mathf.Clamp01(percent);
		
		timerImg.fillAmount = percent;
		timerImg.color = gradient.Evaluate(percent);
	}
	
	public void OnHypothesis(string hypothesis){
		txt.text = hypothesis;
		txt.fontStyle = ITAL;
		
		progressObj.SetActive(true);
	}
	
	public void OnResult(string result, SpeechRecognizer.ResultType type){
		switch(type){
			case CORRECT:
				txt.text = result;
				icon.color = correctColor;
				
			break;
			
			case WRONG:
				txt.text = result;
				icon.color = wrongColor;
				
			break;
			
			case TIMEOUT:
				txt.text = "Time Out";
				icon.gameObject.SetActive(false);
				warningObj.SetActive(true);
				
			break;
		}
		
		txt.fontStyle = BOLD;
		progressObj.SetActive(false);
	}
	
	// debug
	public void OnResult(string result, SpeechRecognizer.ResultType type, MonoBehaviour caller){
		OnResult(result, type);
		Debug.Log("Called from " + caller.name, caller);
	}
}