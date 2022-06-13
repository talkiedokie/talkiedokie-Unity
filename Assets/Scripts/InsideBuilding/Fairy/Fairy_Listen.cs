using System;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class Fairy
{
	// Listen
	// Recognize the word
	// Say Wow or fail
	
	public bool isListening{ get; private set; }
	
	public void ListenToSpeech(
		string speech,
		Action onFinish
	){
		ListenToSpeech(speech, null, onFinish);
	}
	
	public void ListenToSpeech(
		string speech,
		Action onRecognize,
		Action onFinish
	){
		HandleSpeechListenBug(onSpeechResult);
		isListening = true;
		
		void onSpeechResult(string result){
			bool isSkipped = speechRecognizer.isSkipped;
			
			if(result == "" && !isSkipped){
				HandleSpeechResultBug();
				var clips = reinitializeListen? cantHearClips: pleaseSayClips;
				
				SpeakRandom(clips, 0.5f, Loop);
				return;
			}
			
			bool isCorrect = result == speech.ToLower();
			
				if(isCorrect || isSkipped){
					SayWow(0.5f, OnFinish);
					onRecognize?.Invoke();
				}
				
				else SayFail(0.5f, Loop);
			
			isListening = false;
			ResetSpeechBugHandler();
		}
		
		void Loop(){
			ListenToSpeech(speech, onRecognize, onFinish);
		}
		
		void OnFinish(){
			speechRecognizer.Stop();
			onFinish?.Invoke();
		}
	}
	
	#region Plugin Bug Handling
	
		int noListenResultLoop;
		bool reinitializeListen;
		
		void HandleSpeechListenBug(Action<string> onSpeechResult){
			if(reinitializeListen)
				speechRecognizer.ReinitializeListen(onSpeechResult); // (old) causes small (one frame) freeze on call but very stable and safe
			
			else speechRecognizer.Listen(onSpeechResult); // (new) smooth but unstable and may result in lag if the game is running for a long time
		}
		
		void HandleSpeechResultBug(){
			noListenResultLoop ++;
			
			float probability = 1f / (float) noListenResultLoop;
			reinitializeListen = Random.value > probability;
		}
		
		void ResetSpeechBugHandler(){
			noListenResultLoop = 0;
			reinitializeListen = false;
		}
		
	#endregion
	
	public void StopListening(){
		speechRecognizer.Skip();
	}
}