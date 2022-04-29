using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class DictationScript : MonoBehaviour
{
	[SerializeField, TextArea()]
	string hypothesis, result;
	
    DictationRecognizer dr;

    void Start(){
        dr = new DictationRecognizer();
		{
			dr.DictationResult += Result;
			dr.DictationHypothesis += Hypothesis;
			dr.DictationComplete += Complete;
			dr.DictationError += Error;
		}
        dr.Start();
    }
	
	void Result(string text, ConfidenceLevel confidence){
		result += text + "\n";
		
		this.confidence = confidence;
	}
	
	void Hypothesis(string text){
		hypothesis += text;
	}
	
	void Complete(DictationCompletionCause cause){
		if(cause != DictationCompletionCause.Complete)
			Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", cause);
		
		completionCause = cause;
		dr.Stop();
		
		// UI
		micIcon.color = correctColor;
		loadingUI.SetActive(false);
	}
	
	void Error(string error, int hresult){
		Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
	}
	
	void Update(){
		status = dr.Status;
	}
	
	public string word;
	public float listenDuration = 5f;
	
	public DictationCompletionCause completionCause;
	public ConfidenceLevel confidence;
	public SpeechSystemStatus status;
	
	[Space()]
	public GameObject popupUI;
	public Image micIcon;
	
	public Color
		correctColor = Color.green,
		listeningColor = Color.yellow,
		wrongColor = Color.red;
	
	public GameObject loadingUI;
	
	[ContextMenu("Listen")]
	public void Listen(){
		dr.InitialSilenceTimeoutSeconds = listenDuration;
		
        dr.Start();
		
		// UI
		popupUI.SetActive(true);
		micIcon.color = listeningColor;
		loadingUI.SetActive(true);
	}
}