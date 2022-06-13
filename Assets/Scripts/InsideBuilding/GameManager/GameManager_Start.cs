using System.Collections;
using UnityEngine;

namespace InsideBuilding
{
	public partial class GameManager
	{
		public bool hasStarted{ get; private set; }
		
		IEnumerator Start(){
			hasStarted = false;
			Time.timeScale = 0f;
			
			var threads = new IEnumerator[]{
				SetupSingletonReferences(),
				CheckForSpeechSupport(),
				LoadLevel(),
				SetupUsersAccount(),
				SetupPlayerObject(),
				SetupRoomUI(),
				FinalSetup()
			};
			
			int count = threads.Length;
			float maxIndex = (float) count - 1;
			
			startLoadUI.SetActive(true);
			
			for(int i = 0; i < count; i++){
				yield return threads[i];
				
				float normalizedValue = (float) i / maxIndex;
				float percent = Mathf.Round(normalizedValue * 100f);
				
				startProgressImg.fillAmount = normalizedValue;
				startProgressTxt.text = "Loading " + percent + "%";
			}
			
			yield return new WaitForSecondsRealtime(1f);
				
				startLoadUI.SetActive(false);
				uiMgr.Transition();
			
			Time.timeScale = 1f;
			hasStarted = true;
		}
	}
}