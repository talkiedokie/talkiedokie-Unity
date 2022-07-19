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
		
		[Foldout("Other Settings")]
		[SerializeField] bool setMusicVolume = true;
		[SerializeField, Range(0,1)] float musicVolume = 0.1f;
		
		[Space()]
		[SerializeField] bool enableDisableMic = false;
		
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
			
			ui.OnListen();
			appearSound.Play();
			
			if(setMusicVolume)
				SetBgmVolume(musicVolume);
			
			if(enableDisableMic){}
				// enable plugin
		}
		
		public void StopListening(){
			isListening = false;
			
			if(enableDisableMic){}
				// disable Plugin
			
			onFinish = null;
		}
		
		public void SkipListening(){
			isSkipped = true;
			skipSound.Play();
			
			result = " ";
			
			ui.OnResult("skipped");
			SetBgmVolume(1f);
			
			OnFinish();
		}
		
		public void FinishUsing(){
			ui.SetActive(false);
			SetBgmVolume(1f);
		}

       /*  public void ToggleSpeechUI(bool isOn)
        {
            ui.SetActive(isOn);
        } */
		
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
			
			OnFinish();
		}
		
		#endregion
		
		void OnFinish(){
			onFinish?.Invoke(result);
			StopListening();
		}
		
		void SetBgmVolume(float percent){
			if(!genAudio)
				genAudio = GeneralAudio.Instance;
			
			genAudio.SetBGMVolume(percent);
		}
	}
}