using UnityEngine;
using System;

// This Script Manages the SpeechToText plugins for different platforms (not focused on gameplay)

public partial class STTMultiPlatformHandler : SceneObjectSingleton<STTMultiPlatformHandler>
{
	public float listenDuration = 5f;
	
	Action onHypothesis, onResult;
	
	public string hypothesis{ get; private set; } = "";
	public string result{ get; private set; } = "";
	
	void Start(){
		#if (UNITY_EDITOR_WIN || UNITY_WINDOWS || UNITY_STANDALONE_WIN)
			WIN_Start();
			
		#elif (UNITY_ANDROID || UNITY_IOS)
			MOB_Start();
			
		#endif
	}
	
	public void StartListening(Action onHypothesis, Action onResult){ // (new) smooth but unstable and may result in lag if the game is running for a long time
		#if (UNITY_EDITOR_WIN || UNITY_WINDOWS || UNITY_STANDALONE_WIN)
			WIN_Listen();
			
		#elif (UNITY_ANDROID || UNITY_IOS)
			MOB_Listen();
			
		#endif
		
		result = "";
		hypothesis = "";
		
		this.onHypothesis = onHypothesis;
		this.onResult = onResult;
	}
	
	public void StopListening(){
		#if (UNITY_EDITOR_WIN || UNITY_WINDOWS || UNITY_STANDALONE_WIN)
			WIN_StopListening();
			
		#elif (UNITY_ANDROID || UNITY_IOS)
			MOB_StopListening();
			
		#endif
		
		onHypothesis = null;
		onResult = null;
	}
	
	public void ReinitializeListen(Action onHypothesis, Action onResult){ // (old)causes small (one frame) freeze on call but very stable and safe 
		StopListening();
		
		#if (UNITY_EDITOR_WIN || UNITY_WINDOWS || UNITY_STANDALONE_WIN)
			dr?.Dispose();
			WIN_Start();
			
		#elif (UNITY_ANDROID || UNITY_IOS)
		#endif
		
		StartListening(onHypothesis, onResult);
	}
}