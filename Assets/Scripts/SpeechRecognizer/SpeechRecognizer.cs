using UnityEngine;
using UnityEngine.Windows.Speech;
using System;
using System.Collections;

public class SpeechRecognizer : SceneObjectSingleton<SpeechRecognizer>
{
	#region Inspector
		
		[SerializeField, LabelOverride("Word/Phrase")] string word;
		
		[SerializeField] float
			onCompletionExitDuration = 1f,
			listenDuration = 5f;
		
		[SerializeField] SpeechRecognizerUI ui;
		[Space(), SerializeField, TextArea()] string hypothesis;
		
		[Space()]
		[SerializeField] AudioClip[] tryAgainClips;
		
	#endregion
	
	#region Variables/Properties
		
		public string result{ get; private set; }
		public bool listenAgainOnWrong{ private get; set; } = true;
		
		public SpeechSystemStatus status;
		public Action onHypothesis, onResult;
		
		ConfidenceLevel confidence;
		bool isSkipped;
		
		DictationRecognizer dr;
		Action onFinish;
		
		Fairy fairy;
		
	#endregion
	
	#region Unity Methods
		
		void Awake(){
			dr = new DictationRecognizer();
			{
				dr.DictationResult += Result;
				dr.DictationHypothesis += Hypothesis;
				dr.DictationComplete += Complete;
				dr.DictationError += Error;
				
				dr.InitialSilenceTimeoutSeconds = listenDuration;
			}
			
			if(tryAgainClips.Length > 0)
				fairy = Fairy.Instance;
		}
		
		void Update(){
			status = dr.Status;
			
			if(Input.GetKeyDown("p"))
				Skip();
		}
		
		void OnDestroy(){ Stop(); }
		
	#endregion
	
	#region Calls
		
		public void Listen(string word, Action onFinish){
			this.word = word;
			this.onFinish = onFinish;
			
			Listen();
		}
		
		[ContextMenu("Listen")]
		public void Listen(){
			if(!Application.isPlaying) return;
			
			enabled = true;
			isSkipped = false;
			
			dr.InitialSilenceTimeoutSeconds = listenDuration;
			
			StartSpeechDictation();
			ui.OnListen(listenDuration);
			
			Debug.Log("SpeechRecognizer is now listening to the word/phrase: " + word);
		}
		
		[ContextMenu("Skip")]
		public void Skip(){
			if(!Application.isPlaying) return;
			
			if(dr.Status != SpeechSystemStatus.Running){
				Debug.LogWarning("SpeechRecognizer is not running but is currently '" + dr.Status.ToString() + "'");
				return;
			}
			
			isSkipped = true;
			dr.Stop();
			
			ui.OnSkip();
			Debug.LogWarning("Speech recognizer skipped the word/phrase: " + word);
		}
		
		[ContextMenu("Stop")]
		public void Stop(){
			dr.Stop();
			dr.Dispose();
			
			ui.gameObject.SetActive(false);
			enabled = false;
		}
		
	#endregion
	
	#region Events
		
		void Result(string result, ConfidenceLevel confidence){
			this.result = result;
			this.confidence = confidence;
			
			Complete(DictationCompletionCause.Complete);
			dr.Stop();
			
			onResult?.Invoke();
		}
		
		void Hypothesis(string text){
			hypothesis = text;
			
			ui.OnHypothesis();
			onHypothesis?.Invoke();
		}
		
		void Complete(DictationCompletionCause cause){
			if(!Application.isPlaying) return;
			bool isCorrect = false;
			
			if(!isSkipped){
				isCorrect = cause == DictationCompletionCause.Complete && result == word.ToLower();
				float grade = CalculateGrade();
				
				ui.OnCompletion(cause, result, isCorrect, grade);
			}
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
			var exitDelay = new WaitForSeconds(onCompletionExitDuration);
			
			if(isCorrect || isSkipped){
				yield return exitDelay;
				
				ui.gameObject.SetActive(false);
				enabled = false;
				
				onFinish?.Invoke();
			}
			else{
				if(listenAgainOnWrong){
					bool fairyCanSpeak = fairy && tryAgainClips.Length > 0;
					
					if(fairyCanSpeak){
						fairy.Speak(Tools.Random(tryAgainClips), 0.15f);
						while(fairy.isSpeaking) yield return null;
					}
					
					enabled = false;
					Listen();
				}
				
				else{
					yield return exitDelay;
					
					enabled = false;
					onFinish?.Invoke();
				}
			}
		}
		
	#endregion
	
	#region MSC
	
		void StartSpeechDictation(){
			bool isDictatorRunning = false;
			int iteration = 0;
			
			while(!isDictatorRunning && iteration < 50){
				dr.Start(); // this
				
				isDictatorRunning = dr.Status != SpeechSystemStatus.Running;
				iteration ++;
			}
			
			if(iteration > 1)
				Debug.LogWarning("On Completion Looped " + iteration + " times");
		}
		
		const float diffBetweenConfidenceLevel = 0.25f;
		
		float CalculateGrade(){
			float tolerance = (diffBetweenConfidenceLevel * UnityEngine.Random.value);
			float level = (float) confidence + tolerance;
			
				return Mathf.InverseLerp(3, 0, level);
		}
	
	#endregion
}