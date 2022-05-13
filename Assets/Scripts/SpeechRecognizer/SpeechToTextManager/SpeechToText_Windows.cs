using UnityEngine;

#if (UNITY_EDITOR || UNITY_WINDOWS)
using UnityEngine.Windows.Speech;
#endif

public partial class SpeechToText
{
	#if (UNITY_EDITOR || UNITY_WINDOWS)
		
		DictationRecognizer dr;
		
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
			StartSpeechDictation(); // Kick start
		}
		
		void WinScript_StopListening(){
			dr.Stop();
		}
		
		void StartSpeechDictation(){
			bool isDictatorRunning = false;
			int iteration = 0;
			
			while(!isDictatorRunning && iteration < 50){
				dr.Start(); // this
				
				isDictatorRunning = dr.Status != SpeechSystemStatus.Running;
				iteration ++;
			}
			
			if(iteration > 1)
				Debug.LogWarning("On Completion Looped " + iteration + " times");
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
			
			void Complete(DictationCompletionCause cause){}
			
			void Error(string error, int hresult){
				Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
			}
			
		#endregion
		
	#endif
}