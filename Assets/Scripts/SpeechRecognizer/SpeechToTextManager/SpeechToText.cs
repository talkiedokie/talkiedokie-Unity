using UnityEngine;
using System;

// This Script Manages the SpeechToText plugins for different platforms (not focused on gameplay)

public partial class SpeechToText : SceneObjectSingleton<SpeechToText>
{
	public float listenDuration = 5f;
	[TextArea()] public string hypothesis;
	
	Action onHypothesis, onResult;
	public string result{ get; private set; }
	
	#region Setup
		
		void Awake(){
			#if (UNITY_EDITOR || UNITY_WINDOWS)
				WinScript_OnAwake();
			
			#elif (UNITY_ANDROID || UNITY_IOS)
				MobScript_OnAwake();
				
			#endif
		}
		
		void Start(){
			#if (UNITY_EDITOR || UNITY_WINDOWS)
			#elif (UNITY_ANDROID || UNITY_IOS)
				MobScript_OnStart();
				
			#endif
		}
		
		void OnDestroy(){
			#if (UNITY_EDITOR || UNITY_WINDOWS)
				dr.Dispose();
			
			#elif (UNITY_ANDROID || UNITY_IOS)
			#endif
		}
		
	#endregion
	
	#region Main
		
		public void StartListening(Action onHypothesis, Action onResult){
			#if (UNITY_EDITOR || UNITY_WINDOWS)
				WinScript_Listen();
				
			#elif (UNITY_ANDROID || UNITY_IOS)
				MobScript_Listen();
			
			#endif
			
			this.onHypothesis = onHypothesis;
			this.onResult = onResult;
		}
		
		public void StopListening(){
			#if (UNITY_EDITOR || UNITY_WINDOWS)
				WinScript_StopListening();
				
			#elif (UNITY_ANDROID || UNITY_IOS)
				MobScript_StopListening();
			
			#endif
			
			onResult = null;
		}
		
	#endregion
}