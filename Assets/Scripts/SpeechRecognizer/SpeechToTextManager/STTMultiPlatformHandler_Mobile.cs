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
			
			void Awake(){
				var instance = new GameObject("SpeechToText").AddComponent<SpeechToText>();
					
					#if UNITY_ANDROID
					instance.isShowPopupAndroid = false;
					#endif
				
				plugin = SpeechToText.instance;
			}
			
			void MOB_Start(){
				plugin.Setting("en-US");
				plugin.onResultCallback = OnFinalSpeechResult;
				
				#if UNITY_ANDROID
				plugin.onPartialResultsCallback = OnPartialSpeechResult;
				#endif
			}
			
		#endregion
		
		#region Events
			
			void MOB_Listen(){
				#if UNITY_ANDROID
					
					if(!Permission.HasUserAuthorizedPermission(Permission.Microphone))
						Permission.RequestUserPermission(Permission.Microphone);
					
				#endif
				
				plugin.StartRecording();
			}
			
			void MOB_StopListening(){ plugin.StopRecording(); }
			
			void OnFinalSpeechResult(string result){
				this.result = result;
				onResult?.Invoke();
			}
			
			void OnPartialSpeechResult(string hypothesis){
				this.hypothesis = hypothesis;
				onHypothesis?.Invoke();
			}
			
		#endregion
		
	#endif
}