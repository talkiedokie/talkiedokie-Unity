using System;
using UnityEngine;

public partial class Fairy
{
	// Listen
	// Recognize the word
	// Say Wow or fail

	public bool isListening{ get; private set; }
	
	public void ListenToSpeech(
		string message,
		Action onRecognize,
		Action onFinish
	){
		speech.StartListening(onSpeechResult);
		isListening = true;
		
		void onSpeechResult(string result){
			if(result == ""){
				SpeakRandom(pleaseSayClips, 0.5f, Loop);
				return;
			}
			
			message = message.ToLower();
			
			bool isCorrect = result.Contains(message);
			bool isSkipped = speech.isSkipped;
			
			if(isCorrect || isSkipped){
				onRecognize?.Invoke();
				SayWow(0.5f, OnFinish);
				
				isListening = false;
			}
			else SayFail(0.5f, Loop);
			
			void Loop(){
				speech.FinishUsing();
				ListenToSpeech(message, onRecognize, onFinish);
			}
			
			void OnFinish(){
				speech.FinishUsing();
				onFinish?.Invoke();
			}
		}
	}
	public void ListenToSpeech(string wordPhrase, Action onFinish) =>
		ListenToSpeech(wordPhrase, null, onFinish);
		
	public void StopListening() => speech.SkipListening();
}