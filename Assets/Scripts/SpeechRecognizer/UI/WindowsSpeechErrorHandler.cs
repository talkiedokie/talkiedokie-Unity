using UnityEngine;
using UnityEngine.UI;
using System.Collections;

#if UNITY_EDITOR || UNITY_WINDOWS
using UnityEngine.Windows.Speech;
#endif

public class WindowsSpeechErrorHandler : SceneObjectSingleton<WindowsSpeechErrorHandler>
{
	public CompletionCause[] completionCauses;
	
	[Space()]
	public Image icon;
	public Text title, message;
	
	[Space()]
	public GameObject errorObj;
	public Text errorTxt;
	
	[Space()]
	public GameObject defaultPanel;
	public GameObject notSupportedPanel;
	
	[System.Serializable]
	public struct CompletionCause{
		public string name;
		public Sprite icon;
		
		[TextArea()]
		public string message;
		
		public bool ignore;
	}
	
	#if UNITY_EDITOR || UNITY_WINDOWS
		
		/* [ContextMenu("Show Random")]
		public void RandomShow(){
			int random = Random.Range(0, 7);
			Show((DictationCompletionCause) random);
		} */
		
		public void Show(DictationCompletionCause completionCause){
			Debug.Log(completionCause.ToString());
			
			int index = (int) completionCause;
			var cause = completionCauses[index];
			
			if(cause.ignore) return;
			
			icon.sprite = cause.icon;
			title.text = cause.name;
			message.text = cause.message;
			
			errorObj.SetActive(false);
			
			defaultPanel.SetActive(true);
			notSupportedPanel.SetActive(false);
			
			UIManager.Instance.Show(gameObject);
		}
		
		public void OnError(string error, int hresult){
			StartCoroutine(delay());
			
			IEnumerator delay(){
				yield return null;
				
				errorObj.SetActive(true);
				errorTxt.text = "Dictation error: {0}; HResult = {1}. \n" + error + "\n" + hresult;
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