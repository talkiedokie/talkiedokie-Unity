using UnityEngine;
using System;
using System.Collections;

public class SpeechRecognizer : SceneObjectSingleton<SpeechRecognizer>
{
	[TextArea()]
	public string wordPhrase;
	
	[SerializeField] SpeechRecognizerUI ui;
	[SerializeField] float onCompletionExitDuration = 1f;
	[SerializeField] STTMultiPlatformHandler stt; // multi-platform handler
	
	public bool isListening{ get; private set; }
	
	public string result => stt.result;
	public enum ResultType{ None, Correct, Wrong, TimeOut }
	public ResultType resultType{ get; private set; } = ResultType.None;
	
	public bool isCorrect => resultType == ResultType.Correct;
	
	Action onFinish;
	
	#region Calls
	
		public void SetWordPhrase(string wordPhrase){
			this.wordPhrase = wordPhrase;
		}
		
		[ContextMenu("Listen")]
		public void Listen(){
			enabled = true;
			
			stt.StartListening(
				OnListenHypothesis, // loading UI
				OnListenResult
			);
			
			ui.gameObject.SetActive(true);
			ui.OnListen();
			
			StopListenTimer();
				listenTimer = ListenTimer();
				StartCoroutine(listenTimer);
			
			resultType = ResultType.None;
			isListening = true;
		}
		
		public void Listen(string wordPhrase, Action onFinish){
			this.wordPhrase = wordPhrase;
			this.onFinish = onFinish;
			
			Listen();
		}
		
		public void Stop(){
			stt.StopListening();
			ui.gameObject.SetActive(false);
			
			StopAllCoroutines();
			
			enabled = false;
			isListening = false;
		}
		
		[ContextMenu("Skip")]
		public void Skip(){
			Stop();
			onFinish?.Invoke();
		}
		
	#endregion
	
	#region Events
		
		void OnListenHypothesis(){
			StopListenTimer();
			ui.OnHypothesis(stt.hypothesis);
		}
		
		void OnListenResult(){
			StopListenTimer();
			
			resultType = (result == wordPhrase.ToLower())?
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
			
			resultType = ResultType.TimeOut;
			yield return OnListenResult_Routine();
		}
		
		IEnumerator OnListenResult_Routine(){
			ui.OnResult(result, resultType);
			
			yield return new WaitForSeconds(onCompletionExitDuration);
			
			Stop();
			onFinish?.Invoke();
		}
		
	#endregion
}