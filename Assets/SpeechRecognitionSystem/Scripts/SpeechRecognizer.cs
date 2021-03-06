using System;
using System.Collections;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using SpeechRecognitionSystem;

using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;

internal class SpeechRecognizer : MonoBehaviour {
    public string LanguageModelDirPath = "SpeechRecognitionSystem/model/english_small";
	public Fairy fairy;
	
	public void OnDataProviderReady( IAudioProvider audioProvider ) {
        if ( Application.platform == RuntimePlatform.Android ) {
            if ( !Permission.HasUserAuthorizedPermission( Permission.ExternalStorageWrite ) ) {
                Permission.RequestUserPermission( Permission.ExternalStorageWrite );
            }
        }
		
		_audioProvider = audioProvider;
        _sr.Frequency = _audioProvider.Frequency;
		
		onInitResult( Application.streamingAssetsPath + "/" + LanguageModelDirPath );
		
		_running = true;
        Task.Run( processing ).ConfigureAwait( false );
		// StartCoroutine(processing());
    }

    private readonly ConcurrentQueue<float[ ]> _threadedBufferQueue = new ConcurrentQueue<float[ ]>( );
    private readonly ConcurrentQueue<string> _recognitionPartialResultsQueue = new ConcurrentQueue<string>( );
    private readonly ConcurrentQueue<string> _recognitionFinalResultsQueue = new ConcurrentQueue<string>( );
	
	[Foldout("Events")]
	public UnityEvent<string>
		LogMessageReceived,
		PartialResultReceived,
		ResultReceived;
	
    private void onInitResult( string modelDirPath ) {
		if ( modelDirPath != string.Empty ) {
			if ( Directory.Exists( modelDirPath ) )
				_init = _sr.Init( modelDirPath ); // <<<<<<<<<<<<<<<<<<<<<<< LAG
			
            else _init = false;
				
			if ( _init ){
                LogMessageReceived?.Invoke( "Say something..." );
                Debug.Log( "Say something..." );
			}
			
            else{
                LogMessageReceived?.Invoke( "Error on init SRS plugin. Check 'Language model dir path'\n" + modelDirPath );
                Debug.LogWarning( "Error on init SRS plugin. Check 'Language model dir path'\n" + modelDirPath );
			}
        }
        else{
            LogMessageReceived?.Invoke( "Error on copying streaming assets" );
            Debug.LogWarning( "Error on copying streaming assets" );
		}
    }
	
    private void onReceiveLogMess( string message ) =>
        LogMessageReceived?.Invoke( message );
	
    private void Awake( ) =>
		_sr = new SpeechRecognitionSystem.SpeechRecognizer( );
	
	private void FixedUpdate( ) {
		if( fairy ) {
			if( fairy.isSpeaking )
				return;
		}
		
        if ( Application.platform == RuntimePlatform.Android ) {
            if ( !_copyRequested && Permission.HasUserAuthorizedPermission( Permission.ExternalStorageWrite ) ) {
                copyAssets2ExternalStorage( LanguageModelDirPath );
                _copyRequested = true;
            }
        }
		
        if ( _init && ( _audioProvider != null ) ) {
            var audioData = _audioProvider.GetData( );
			
            if ( audioData != null )
                _threadedBufferQueue.Enqueue( audioData );

            if ( _recognitionPartialResultsQueue.TryDequeue( out string part ) ) {
                if ( part != string.Empty )
                    PartialResultReceived?.Invoke( part );
            }
			
            if ( _recognitionFinalResultsQueue.TryDequeue( out string result ) ) {
                if ( result != string.Empty )
					ResultReceived?.Invoke( result );
            }
        }
    }

    private async Task processing( ) {
        while ( _running ) {
            float[ ] audioData;
            var isOk = _threadedBufferQueue.TryDequeue( out audioData );
			
			if ( isOk )
				GetMessages( audioData );
			
            else
                await Task.Delay( 100 );
        }
    }
	
	/* private IEnumerator Task processing( ) {
		var delay = new WaitForSeconds(0.1f);
		
        while ( _running ) {
            float[ ] audioData;
            var isOk = _threadedBufferQueue.TryDequeue( out audioData );
			
			if ( isOk ) {
                int resultReady = _sr.AppendAudioData( audioData );
				GetMessages( resultReady );
            }
            else
                yield return delay;
        }
    } */
	
	void GetMessages( float[ ] audioData ) {
		if( fairy ) {
			if( fairy.isSpeaking )
				return;
		}
		
		int resultReady = _sr.AppendAudioData( audioData );
		 
		if ( resultReady == 0 )
			_recognitionPartialResultsQueue.Enqueue( _sr.GetPartialResult( )?.partial );
		
		else
			_recognitionFinalResultsQueue.Enqueue( _sr.GetResult( )?.text );
	}
	
	private bool _running = false;

    private void OnDestroy( ) {
		_init = false;
        _copyRequested = false;
        _running = false;

        _sr.Dispose( );
        _sr = null;
    }
	
	 private void copyAssets2ExternalStorage( string modelDirPath ) {
        if ( Application.platform == RuntimePlatform.Android ) {
            var javaUnityPlayer = new AndroidJavaClass( "com.unity3d.player.UnityPlayer" );
            var currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>( "currentActivity" );
            var recognizerActivity = new AndroidJavaObject( "com.sss.unity_asset_manager.MainActivity", currentActivity );
			
            recognizerActivity.CallStatic( "setReceiverObjectName", this.gameObject.name );
            recognizerActivity.CallStatic( "setLogReceiverMethodName", "onReceiveLogMess" );
            recognizerActivity.CallStatic( "setOnCopyingCompleteMethod", "onInitResult" );
			
            if ( Permission.HasUserAuthorizedPermission( Permission.ExternalStorageWrite ) ) {
                LogMessageReceived?.Invoke( "Please wait until the files of language model are copied..." );
                recognizerActivity.Call( "tryCopyStreamingAssets2ExternalStorage", modelDirPath );
            }
        }
    }

    private SpeechRecognitionSystem.SpeechRecognizer _sr = null;
	
	private IAudioProvider _audioProvider = null;
    private bool _init = false;
    private bool _copyRequested = false;
}