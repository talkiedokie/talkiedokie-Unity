using System;
using UnityEngine;

public partial class Fairy
{
	// Listen
	// Recognize the word
	// Say Wow or fail
	
	public bool isListening{ get; private set; }
	bool isListenStopped;
	
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
		speechRecognizer.Listen(speech, onSpeechResult);
		isListening = true;
		
		void onSpeechResult(){
			if(isListenStopped || speechRecognizer.isCorrect){
				onRecognize?.Invoke();
				SayWow(0.5f, onFinish);
				
				isListenStopped = false;
			}
			
			else{
				SayFail(0.5f, loop);
				
				void loop(){
					ListenToSpeech(speech, onRecognize, onFinish);
				}
			}
			
			isListening = false;
		}
	}
	
	public void StopListening(){
		speechRecognizer.Skip();
		isListenStopped = true;
	}
}