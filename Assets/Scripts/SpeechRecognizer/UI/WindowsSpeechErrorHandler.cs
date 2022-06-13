using UnityEngine;
using UnityEngine.UI;
using System.Collections;

#if UNITY_EDITOR_WIN || UNITY_WINDOWS || UNITY_STANDALONE_WIN
using UnityEngine.Windows.Speech;
#endif

public class WindowsSpeechErrorHandler : SceneObjectSingleton<WindowsSpeechErrorHandler>
{
	public CompletionCause[] completionCauses;
	
	[Space()]
	public Image icon;
	public Text title, message;
	
	[Space()]
	public GameObject defaultPanel;
	public GameObject notSupportedPanel;
	
	[Space()]
	public Text errorTxt;
	public Text hresultTxt;
	
	[System.Serializable]
	public struct CompletionCause{
		public string name;
		public Sprite icon;
		
		[TextArea()]
		public string message;
		
		public bool ignore;
	}
	
	#if UNITY_EDITOR_WIN || UNITY_WINDOWS || UNITY_STANDALONE_WIN
		
		public void Show(DictationCompletionCause completionCause){
			int index = (int) completionCause;
			var cause = completionCauses[index];
			
			if(cause.ignore) return;
			Debug.LogWarning(cause.ToString());
			
			icon.sprite = cause.icon;
			title.text = cause.name;
			message.text = cause.message;
			
			defaultPanel.SetActive(true);
			notSupportedPanel.SetActive(false);
			
			UIManager.Instance.Show(gameObject);
		}
		
		public void OnError(string error, int hresult){
			gameObject.SetActive(true);
			StartCoroutine(delay());
			
			IEnumerator delay(){
				defaultPanel.SetActive(false);
				notSupportedPanel.SetActive(true);
			
				yield return null;
				
				errorTxt.text = error;
				hresultTxt.text = hresult.ToString();
			}
		}
		
		public void NotSupported(){
			defaultPanel.SetActive(false);
			notSupportedPanel.SetActive(true);
			
			UIManager.Instance.Show(gameObject);
		}
		
	#endif
	
	public void ExitGame(){ Application.Quit(); }
	public void RestartScene(){ SceneLoader.Current(); }
}