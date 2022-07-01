using System;
using System.Collections;
using UnityEngine;

public class test : MonoBehaviour
{
	#region Fairy
		
		public string wordPhrase = "Apple";
		
		public AudioSource source;
		public AudioClip[] clips;
		IEnumerator speakRoutine;
		
		[ContextMenu("Test")]
		public void Test(){
			ListenToSpeech(wordPhrase, null, null);
		}
		
		void Speak(Action onFinish){
			if(source.isPlaying) return;
			
			source.clip = Tools.Random(clips);
			source.Play();
			
			speakRoutine = routine();
			StartCoroutine(speakRoutine);
			
			IEnumerator routine(){
				while(source.isPlaying) yield return null;
				
				yield return new WaitForSeconds(0.1f);
				onFinish?.Invoke();
			}
		}
		
		void ListenToSpeech(string message, Action onRecognize, Action onFinish){
			speech_StartListening(onSpeechResult);
			
			void onSpeechResult(string result){
				bool isCorrect = result == message.ToLower();
				
				if(isCorrect){
					onRecognize?.Invoke();
					// Say Wow
					
					speech_StopListening();
					onFinish?.Invoke();
					
					Debug.Log("CORRECT");
				}
				else{
					speech_StopListening();
					Speak(Loop);
					
					void Loop() =>
						ListenToSpeech(message, onRecognize, onFinish);
					
					Debug.LogWarning("WRONG");
				}
			}
		}
		
	#endregion
	
	#region Speech
		
		public GameObject plugin;
		
		public bool speech_isListening;
		
		[TextArea()]
		public string hypothesis, result;
		Action<string> onFinish;
		
		void speech_StartListening(Action<string> onFinish){
			if(speech_isListening){
				Debug.LogWarning("You can't start speech listening while it is already listening"); 
				return;
			}
			
			this.onFinish = onFinish;
			speech_isListening = true;
			
			plugin.SetActive(true);
		}
		
		void speech_StopListening(){
			plugin.SetActive(false);
			onFinish = null;
		}
		
		public void OnPartialResult(string message){
			if(!speech_isListening) return;
			hypothesis = message;
		}
		
		public void OnSpeechPluginResult(string message){
			if(!speech_isListening) return;
			
			result = message;
			speech_isListening = false;
			
			onFinish?.Invoke(message);
		}
		
	#endregion
	
/* 	public AudioSource source;
	public AudioClip[] clips;
	
	public GameObject plugin;
	
	bool isListening = false;
	
	IEnumerator Start(){
		while(true){
			
			plugin.SetActive(false);
			yield return Speak();
			
			plugin.SetActive(true);
			Listen();
			
			while(isListening) yield return null;
			
			yield return WaitForInput();
		}
	}
	
	IEnumerator Speak(){
		source.clip = Tools.Random(clips);
		source.Play();
		
		while(source.isPlaying) yield return null;
	}
	
	void Listen() => isListening = true;
	
	public void OnLogMessage(string message){
		if(!isListening) return;
		Debug.Log("LOG: " + message + ". IS LISTENING = " + isListening);
	}
	public void OnPartialResult(string message){
		if(!isListening) return;
		Debug.Log("PARTIAL: " + message + ". IS LISTENING = " + isListening);
	}
	
	public void OnSpeechPluginResult(string message){
		if(!isListening) return;
		
		Debug.Log("RESULT: " + message + ". IS LISTENING = " + isListening);
		isListening = false;
	}
	
	IEnumerator WaitForInput(){
		while(!Input.GetKeyDown("space")) yield return null;
	} */
}