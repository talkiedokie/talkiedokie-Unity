using UnityEngine;

#if (UNITY_EDITOR_WIN || UNITY_WINDOWS || UNITY_STANDALONE_WIN)
using UnityEngine.Windows.Speech;
#endif

public partial class STTMultiPlatformHandler
{
	#if (UNITY_EDITOR_WIN || UNITY_WINDOWS || UNITY_STANDALONE_WIN)
		
		public bool IsWindowSpeechSupported => PhraseRecognitionSystem.isSupported;
		WindowsSpeechErrorHandler errorUI => WindowsSpeechErrorHandler.Instance;
		
		DictationRecognizer dr;
		
		void WIN_Start(){
			dr = new DictationRecognizer();
			{
				dr.DictationHypothesis += Hypothesis;
				dr.DictationResult += Result;
				dr.DictationComplete += Complete;
				dr.DictationError += Error;
				
				dr.InitialSilenceTimeoutSeconds = listenDuration;
			}
		}
		
		void OnDisable(){ dr?.Dispose(); }
		
		void WIN_Listen(){ dr?.Start(); }
		void WIN_StopListening(){ dr?.Stop(); }
		
		#region Events
			
			void Hypothesis(string hypothesis){
				this.hypothesis = hypothesis;
				onHypothesis?.Invoke();
			}
			
			void Result(string result, ConfidenceLevel confidence){
				this.result = result;
				onResult?.Invoke();
			}
			
			void Complete(DictationCompletionCause cause){
				errorUI.Show(cause);
			}
			
			void Error(string error, int hresult){
				errorUI.OnError(error, hresult);
			}
			
		#endregion
		
	#endif
}