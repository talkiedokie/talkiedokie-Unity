using UnityEngine;

#if (UNITY_EDITOR || UNITY_WINDOWS)
using UnityEngine.Windows.Speech;
#endif

public partial class STTMultiPlatformHandler
{
	#if (UNITY_EDITOR || UNITY_WINDOWS)
		
		public bool IsWindowSpeechSupported => PhraseRecognitionSystem.isSupported;
		
		DictationRecognizer dr;
		WindowsSpeechErrorHandler errorUI => WindowsSpeechErrorHandler.Instance;
		
		void WinScript_OnAwake(){
			dr = new DictationRecognizer();
			{
				dr.DictationResult += Result;
				dr.DictationHypothesis += Hypothesis;
				dr.DictationComplete += Complete;
				dr.DictationError += Error;
				
				dr.InitialSilenceTimeoutSeconds = listenDuration;
			}
		}
		
		void WinScript_Listen(){
			dr.InitialSilenceTimeoutSeconds = listenDuration;
			dr.Start();
		}
		
		void WinScript_StopListening(){
			if(dr.Status != SpeechSystemStatus.Stopped){
				dr.Stop();
				dr.Dispose();
			}
		}
		
		#region Events
			
			void Result(string result, ConfidenceLevel confidence){
				this.result = result;
				onResult?.Invoke();
			}
			
			void Hypothesis(string text){
				hypothesis = text;
				onHypothesis?.Invoke();
			}
			
			void Complete(DictationCompletionCause cause){
				if(cause != DictationCompletionCause.Complete)
					errorUI.Show(cause);
			}
			
			void Error(string error, int hresult){
				errorUI.OnError(error, hresult);
			}
			
		#endregion
		
	#endif
}