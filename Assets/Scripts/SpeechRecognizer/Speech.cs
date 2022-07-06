using UnityEngine;
using System;
using System.Collections;

namespace Gameplay
{
	public class Speech : SceneObjectSingleton<Speech>
	{
		[SerializeField, LabelOverride("Plugin Manager")] GameObject plugin;
		[SerializeField] SpeechUI ui;
		
		[SerializeField] GeneralAudioSelector
			appearSound,
			skipSound;
		
		public AudioRecorder audioRecorder;
		
		public bool isListening{ get; private set; }
		public bool isSkipped{ get; private set; }
		
		[TextArea()]
		public string hypothesis, result;
		Action<string> onFinish;
		
		GameObject pluginInstance;
		SpeechRecognizer speechRecognizer;
		
		GeneralAudio genAudio;
		
		#region Calls
		
		public void StartListening(Action<string> onFinish){
			if(isListening){
				Debug.LogWarning("You can't start speech listening while it is already listening"); 
				return;
			}
			
			this.onFinish = onFinish;
			
			isListening = true;
			isSkipped = false;
			
			// StartMic();
			
			ui.OnListen();
			appearSound.Play();
			
			SetBgmVolume(0.1f);
		}
		
		public void StopListening(){
			isListening = false;
			// EndMic();
			
			onFinish = null;
		}
		
		public void SkipListening(){
			isSkipped = true;
			skipSound.Play();
			
			result = " ";
			
			ui.OnResult("skipped");
			SetBgmVolume(1f);
			
			onFinish?.Invoke(result);
			StopListening();
		}
		
		public void FinishUsing(){
			ui.SetActive(false);
			SetBgmVolume(1f);
		}
		
		#endregion
		
		#region Events
		
		public void OnPluginPartialResultCallback(string message){
			if(!isListening) return;
			
			hypothesis = message;
			ui.OnHypothesis(message);
		}
		
		public void OnPluginResultCallback(string message){
			if(!isListening) return;
			
			result = message;
			ui.OnResult(result);
			
			onFinish?.Invoke(result);
		}
		
		#endregion
		
		void SetBgmVolume(float percent){
			if(!genAudio)
				genAudio = GeneralAudio.Instance;
			
			genAudio.SetBGMVolume(percent);
		}
	
		public void StartMic() => audioRecorder.StartMic();
		public void EndMic() => audioRecorder.EndMic();
	}
}