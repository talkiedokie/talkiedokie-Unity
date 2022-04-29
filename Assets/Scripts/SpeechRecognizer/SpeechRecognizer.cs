using UnityEngine;
using UnityEngine.Windows.Speech;
using System;
using System.Collections;

public class SpeechRecognizer : SceneObjectSingleton<SpeechRecognizer>
{
	[SerializeField] float onCompletionExitDuration = 1f;
	[SerializeField] string word;
	[SerializeField] float listenDuration = 5f;
	
	[SerializeField] SpeechRecognizerUI ui;
	[Space(), SerializeField, TextArea()] string hypothesis;
	
	public SpeechSystemStatus status;
	
	string result;
	ConfidenceLevel confidence;
	
	DictationRecognizer dr;
	Action onFinish;
	
	[Space()]
	[SerializeField] AudioClip[] tryAgainClips;
	
	Fairy fairy => Fairy.Instance;
	
	void Awake(){
		dr = new DictationRecognizer();
		{
			dr.DictationResult += Result;
			dr.DictationHypothesis += Hypothesis;
			dr.DictationComplete += Complete;
			dr.DictationError += Error;
			
			dr.InitialSilenceTimeoutSeconds = listenDuration;
		}
	}
	
	void Update(){
		status = dr.Status;
	}
	
	public void Listen(string word, Action onFinish){
		this.word = word.ToLower();
		this.onFinish = onFinish;
		
		Listen();
	}
	
	[ContextMenu("Listen")]
	public void Listen(){
		dr.InitialSilenceTimeoutSeconds = listenDuration;
		
		StartSpeechDictation();
		ui.OnListen();
	}
	
	void StartSpeechDictation(){
		bool isDictatorRunning = false;
		int iteration = 0;
		
		while(!isDictatorRunning && iteration < 100){
			dr.Start();
			
			isDictatorRunning = dr.Status != SpeechSystemStatus.Running;
			iteration ++;
		}
		
		if(iteration > 1)
			Debug.LogWarning("On Completion Looped " + iteration + " times");
	}
	
	#region Events
		
		void Result(string result, ConfidenceLevel confidence){
			this.result = result;
			this.confidence = confidence;
			
			Complete(DictationCompletionCause.Complete);
			dr.Stop();
		}
		
		void Hypothesis(string text){
			hypothesis = text;
			ui.OnHypothesis();
		}
		
		void Complete(DictationCompletionCause cause){
			bool isCorrect = cause == DictationCompletionCause.Complete && result == word;
			float grade = CalculateGrade();
			
			ui.OnCompletion(cause, result, isCorrect, grade);
			OnCompletion(isCorrect);
		}
		
		void Error(string error, int hresult){
			Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
		}
		
	#endregion
	
	#region Coroutines
	
		IEnumerator onCompletion;
		
		void OnCompletion(bool isCorrect){
			if(onCompletion != null)
				StopCoroutine(onCompletion);
			
			onCompletion = OnCompletion_Routine(isCorrect);
			StartCoroutine(onCompletion);
		}
		
		IEnumerator OnCompletion_Routine(bool isCorrect){
			if(isCorrect){
				yield return new WaitForSeconds(onCompletionExitDuration);
				
				ui.gameObject.SetActive(false);
				onFinish?.Invoke();
			}
			else{
				fairy.Speak(Tools.Random(tryAgainClips), 0.15f);
				while(fairy.isSpeaking) yield return null;
				
				Listen();
			}
		}
		
	#endregion
	
	#region MSC
		
		const float diffBetweenConfidenceLevel = 0.25f;
		
		float CalculateGrade(){
			float tolerance = (diffBetweenConfidenceLevel * UnityEngine.Random.value);
			float level = (float) confidence + tolerance;
			
				return Mathf.InverseLerp(3, 0, level);
		}
	
	#endregion
}