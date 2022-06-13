using UnityEngine;
using UnityEngine.UI;

public class SpeechRecognizerUI : MonoBehaviour
{
	[SerializeField] Image timerImg;
	[SerializeField] GameObject hypothesisObj;
	[SerializeField] Text resultTxt;
	
	const FontStyle
		ITAL = FontStyle.Italic,
		BOLD = FontStyle.Bold;
	
	public void SetActive(bool b){
		gameObject.SetActive(b);
	}
	
	public void OnListen(){
		SetActive(true);
		timerImg.gameObject.SetActive(true);
		
		resultTxt.text = "";
	}
	
	public void OnHypothesis(string hypothesis){
		hypothesisObj.SetActive(true);
		timerImg.gameObject.SetActive(false);
		
		resultTxt.fontStyle = ITAL;
		resultTxt.text = hypothesis;
	}
	
	public void UpdateTimer(float percent){
		timerImg.fillAmount = percent;
	}
	
	public void OnResult(string result){
		resultTxt.fontStyle = BOLD;
		resultTxt.text = result;
		
		hypothesisObj.SetActive(false);
	}
}