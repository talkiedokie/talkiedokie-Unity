using UnityEngine;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public partial class SpeechToText
{
	#if (UNITY_ANDROID || UNITY_IOS)
	
		TextSpeech.SpeechToText speechToText;
		const string LANG_CODE = "en-US";
		
		#region Setup
			
			void MobScript_OnAwake(){
				InstantiateManager();
				RegisterCallbacks();
				CheckPermission();
			}
			
			void MobScript_OnStart(){
				speechToText.Setting(LANG_CODE);
			}
			
			void InstantiateManager(){
				var instance = new GameObject("SpeechToText").AddComponent<TextSpeech.SpeechToText>();
					#if UNITY_ANDROID
					instance.isShowPopupAndroid = false;
					#endif
				
				speechToText = TextSpeech.SpeechToText.instance;
			}
			
			void RegisterCallbacks(){
				speechToText.onResultCallback = OnFinalSpeechResult;
				
				#if UNITY_ANDROID
					speechToText.onPartialResultsCallback = OnPartialSpeechResult;
				#endif
			}
			
			void CheckPermission(){
				#if UNITY_ANDROID
					
					var micPermission = Permission.Microphone;
					bool isMicPermitted = Permission.HasUserAuthorizedPermission(micPermission);
					
					if(!isMicPermitted)
						Permission.RequestUserPermission(micPermission);
					
				#endif
			}
			
		#endregion
		
		void MobScript_Listen(){ speechToText.StartRecording(); } // Add Timer then call StopListening
		void MobScript_StopListening(){ speechToText.StopRecording(); }
		
		void OnFinalSpeechResult(string result){
			this.result = result;
			onResult?.Invoke();
		}
		
		void OnPartialSpeechResult(string hypothesis){
			this.hypothesis = hypothesis;
			onHypothesis?.Invoke();
		}
		
	#endif
}