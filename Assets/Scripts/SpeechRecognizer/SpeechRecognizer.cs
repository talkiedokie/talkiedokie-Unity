using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;

public class SpeechRecognizer : SceneObjectSingleton<SpeechRecognizer>
{
	[TextArea()] public string word;
	
	public string result => stt.result;
	
	public enum ResultType{ None, Correct, Wrong, TimeOut }
	public ResultType resultType{ get; private set; } = ResultType.None;
	
	public bool isCorrect => resultType == ResultType.Correct;
	
	[SerializeField] SpeechRecognizerUI ui;
	[SerializeField] float onCompletionExitDuration = 1f;
	
	[Foldout("Events")]
	[SerializeField] UnityEvent
		onListen, onHypothesis,
		onCorrect, onWrong, onTimeOut;
	
	[SerializeField] UnityEvent<ResultType> onResult;
	
	Action onFinish;
	SpeechToText stt; // multi-platform handler
	
	void Awake(){
		stt = SpeechToText.Instance;
	}
	
	#region Calls
		
		[ContextMenu("Listen")]
		public void Listen(){
			enabled = true;
			
			stt.StartListening(
				OnListenHypothesis, // loading UI
				OnListenResult
			);
			
			StopListenTimer();
				listenTimer = ListenTimer();
				StartCoroutine(listenTimer);
			
			resultType = ResultType.None;
			
			// Events
				ui.gameObject.SetActive(true);
				ui.OnListen();
				onListen?.Invoke();
		}
		
		public void Listen(string word, Action onFinish){
			this.word = word;
			this.onFinish = onFinish;
			
			Listen();
		}
		
		[ContextMenu("Stop Listening")]
		public void Stop(){
			stt.StopListening();
			ui.gameObject.SetActive(false);
		}
		
	#endregion
	
	#region Events
		
		void OnListenHypothesis(){
			if(resultType == ResultType.TimeOut)
				return;
			
			StopListenTimer();
			
			// Events
				ui.OnHypothesis(stt.hypothesis);
				onHypothesis?.Invoke();
		}
		
		void OnListenResult(){
			if(resultType == ResultType.TimeOut)
				return;
			
			StopListenTimer();
			
			resultType = (result == word.ToLower())?
				ResultType.Correct:
				ResultType.Wrong;
			
			StartCoroutine(OnListenResult_Routine());
		}
		
	#endregion
	
	#region Coroutines
		
		IEnumerator listenTimer;
		
		void StopListenTimer(){
			if(listenTimer != null)
				StopCoroutine(listenTimer);
		}
		
		IEnumerator ListenTimer(){
			float duration = stt.listenDuration;
			float timer = stt.listenDuration;
			
			while(timer > 0f){
				ui.OnListenTimer(timer / duration);
				
				timer -= Time.deltaTime;
				yield return null;
			}
			
			onTimeOut?.Invoke(); // Event
			
			resultType = ResultType.TimeOut;
			yield return OnListenResult_Routine();
		}
		
		IEnumerator OnListenResult_Routine(){
			enabled = false;
			
			// Events
				ui.OnResult(result, resultType);
				onResult?.Invoke(resultType);
				
				switch(resultType){
					case ResultType.None: 							break;
					case ResultType.Correct: onCorrect?.Invoke();	break;
					case ResultType.Wrong:	 onWrong?.Invoke();		break;
					case ResultType.TimeOut: onTimeOut?.Invoke();	break;
				}
			
			yield return new WaitForSeconds(onCompletionExitDuration);
			
			Stop();
			
			onFinish?.Invoke();
			ui.gameObject.SetActive(false);	
		}
		
	#endregion
}