using UnityEngine;
using TextSpeech;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public partial class STTMultiPlatformHandler
{
	#if (UNITY_ANDROID || UNITY_IOS)
		
		SpeechToText plugin;
		
		#region Setup
			
			void MobScript_OnAwake(){
				var instance = new GameObject("SpeechToText").AddComponent<SpeechToText>();
					
					#if UNITY_ANDROID
					instance.isShowPopupAndroid = false; // UNITY_ANDROID
					#endif
				
				plugin = SpeechToText.instance;
			}
			
			void MobScript_OnStart(){
				plugin.Setting("en-US");
				plugin.onResultCallback = OnFinalSpeechResult;
				
				#if UNITY_ANDROID
				
					plugin.onPartialResultsCallback = OnPartialSpeechResult; // UNITY_ANDROID
					
					if(!Permission.HasUserAuthorizedPermission(Permission.Microphone)) // UNITY_ANDROID
						Permission.RequestUserPermission(Permission.Microphone);
					
				#endif
			}
			
		#endregion
		
		#region Events
			
			void MobScript_Listen(){
				plugin.StartRecording();
			}
			
			void MobScript_StopListening(){
				plugin.StopRecording();
			}
			
			void OnFinalSpeechResult(string result){
				this.result = result;
				onResult?.Invoke();
			}
			
			void OnPartialSpeechResult(string result){
				this.result = result;
				onHypothesis?.Invoke();
			}
			
		#endregion
		
	#endif
}