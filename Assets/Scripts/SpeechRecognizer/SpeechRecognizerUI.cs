using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class SpeechRecognizerUI : MonoBehaviour
{
	public Image micIcon;
	
	public Text resultTxt, gradeTxt;
	public Gradient correctColor;
	
	public Color
		listeningColor = Color.yellow,
		wrongColor = Color.red;
	
	public GameObject loading, warningIcon;
	
	public int
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
	
	public void OnHypothesis(){
		loading.SetActive(true);
	}
	
	public void OnListen(){
		micIcon.gameObject.SetActive(true);
		micIcon.color = listeningColor;
		
		// Reset
			gradeTxt.gameObject.SetActive(false);
			resultTxt.gameObject.SetActive(false);
			warningIcon.SetActive(false);
		
		gameObject.SetActive(true);
		
		pop.SetTrigger(trigger);
		genAudio.Play(popSound);
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
				
				genAudio.Play(happySound);
			}
			
			else micIcon.color = wrongColor;
			
			pop.SetTrigger(trigger);
		}
		else{
			result = Tools.AddStringSpaces(completionCause.ToString());
			
			micIcon.gameObject.SetActive(false);
			warningIcon.SetActive(true);
			
			genAudio.Play(sadSound);
		}
		
		resultTxt.text = result;
		resultTxt.gameObject.SetActive(true);
		
		loading.gameObject.SetActive(false);
	}
}