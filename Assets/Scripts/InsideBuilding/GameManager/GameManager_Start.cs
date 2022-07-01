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
			
			// Collect threads
				var threads = new IEnumerator[]{
					SetupSingletonReferences(),
					LoadLevel(),
					SetupUsersAccount(),
					SetupPlayerObject(),
					SetupRoomUI(),
					FinalizeSetup()
				};
			
			// Initialize Progress
				int count = threads.Length;
				float maxIndex = (float) count - 1;
				
				startLoadUI.SetActive(true);
			
			// Iterate Threads
				for(int i = 0; i < count; i++){
					yield return threads[i]; // Iterate
					
					// Update Progress
						float normalizedValue = (float) i / maxIndex;
						float percent = Mathf.Round(normalizedValue * 100f);
						
						startProgressImg.fillAmount = normalizedValue;
						startProgressTxt.text = "Loading " + percent + "%";
				}
			
			// Post Iteration
				yield return new WaitForSecondsRealtime(1f);
					
					startLoadUI.SetActive(false);
					uiMgr.Transition();
			
			// On Exit
				Time.timeScale = 1f;
				hasStarted = true;
		}
	}
}