using UnityEngine;
using System;

public partial class SpeechRecognizer : SceneObjectSingleton<SpeechRecognizer>
{
	[SerializeField, LabelOverride("Plugin Manager")] STTMultiPlatformHandler stt;
	[SerializeField] SpeechRecognizerUI ui;
	[SerializeField] float onFinishExitDelay = 0.65f;
	[SerializeField] GeneralAudioSelector skipSound;
	
	Action<string> onResult;
	
	public bool isListening{ get; private set; }
	public bool isSkipped{ get; private set; }
	
	public string result => stt.result;
	
	#region Commands
		
		public void Listen(Action<string> onResult){ // (new) smooth but unstable and may result in lag if the game is running for a long time
			OnListen(onResult, stt.StartListening);
			Debug.Log("Speech Recognizer is Listening");
		}
		
		public void ReinitializeListen(Action<string> onResult){ // (old) causes small (one frame) freeze on call but very stable and safe
			OnListen(onResult, stt.ReinitializeListen);
			Debug.LogWarning("Speech Listen was Reinitialized");
		}
	
		public void Stop(){
			ui.SetActive(false);
			gameObject.SetActive(false);
			
			StopAllCoroutines();
			GeneralAudio.Instance.SetBGMVolume(1f);
		}
		
		public void Skip(){
			isSkipped = true;
			OnResult();
			
			skipSound.Play();
		}
		
	#endregion
}