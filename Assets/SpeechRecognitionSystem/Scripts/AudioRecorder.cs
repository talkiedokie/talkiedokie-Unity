using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;
using SpeechRecognitionSystem;

public class AudioRecorder : MonoBehaviour, IAudioProvider {
	public int MicrophoneIndex = 0;
	
    public int GetRecordPosition( ) => Microphone.GetPosition( _deviceName );
    public AudioClip GetAudioClip( ) => _audioClip;
    public bool IsRecording( ) => Microphone.IsRecording( _deviceName );
    public int MicrophoneSampleRate = 16000;
    public float Frequency => ( float ) _audioClip.frequency;

    public float[ ] GetData( ) {
        int pos = Microphone.GetPosition( _deviceName );
        int diff = pos - _lastSample;
		
        if ( diff > 0 ) {
            var samples = new float[ diff ];
			
            _audioClip.GetData( samples, _lastSample );
            _lastSample = pos;
			
            return samples;
        }
		
        _lastSample = pos;
        return null;
    }
	
    public AudioReadyEvent MicReady = new AudioReadyEvent( );
	
    private void Awake( ) {
		if ( Application.platform == RuntimePlatform.Android ) {
            if ( !Permission.HasUserAuthorizedPermission( Permission.Microphone ) )
                Permission.RequestUserPermission( Permission.Microphone );
        }
    }
	
    private void Update( ) {
        bool micAutorized = true;
		
		if ( Application.platform == RuntimePlatform.Android )
            micAutorized = Permission.HasUserAuthorizedPermission( Permission.Microphone );
		
        if ( micAutorized) {
            if ( _firstLoad ) {
                _deviceName = Microphone.devices [ MicrophoneIndex ];
				
                // _audioClip = Microphone.Start( _deviceName, true, LENGTH_SEC, MicrophoneSampleRate );
				StartMic();
				
                this.MicReady?.Invoke( this );
                _firstLoad = false;
            }
			
        }
    }
	
	public void StartMic( ) =>
		_audioClip = Microphone.Start( _deviceName, true, LENGTH_SEC, MicrophoneSampleRate );
	
	public void EndMic( ) => Microphone.End( _deviceName );
	
	private void OnDestroy( ) {
        // Microphone.End( _deviceName );
		EndMic();
		
        _firstLoad = true;
    }
	
	public static bool isMicActive{ get; private set; }
	
    private bool _firstLoad = true;
    private AudioClip _audioClip = null;
    private const int LENGTH_SEC = 2;
    private string _deviceName;
    private const int FRAME_LENGTH = 512;
    private int _lastSample = 0;
}