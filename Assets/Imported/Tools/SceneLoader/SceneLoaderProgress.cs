using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System;

public class SceneLoaderProgress : SceneObjectSingleton<SceneLoaderProgress> // This script is Used by SceneLoader because it doesn't have a support of using a coroutine
{
	#region Inspector
		
		[SerializeField] Image progressBar;
		[SerializeField] Text progressValue;
		
		[Space()]
		[SerializeField] float exitDelay = 0.25f;
		
	#endregion
	
	#region Variables
		
		float progress;
		int sceneIndex;
		
		LoadSceneMode mode;
		UnloadSceneOptions options;
		
		AsyncOperation operation;
		Action onFinish;
		
	#endregion
	
	#region Calls
		
		public void Load(
			int sceneIndex,
			LoadSceneMode mode,
			Action onFinish
		){
			this.sceneIndex = sceneIndex;
			this.mode = mode;
			this.options = UnloadSceneOptions.None;
			this.onFinish = onFinish;
			
			gameObject.SetActive(true);
			StartCoroutine(LoadRoutine());
			StartCoroutine(Smoother());
		}
		
		public void Unload(
			int sceneIndex,
			UnloadSceneOptions options
		){
			this.sceneIndex = sceneIndex;
			this.mode = LoadSceneMode.Single;
			this.options = options;
			this.onFinish = null;
			
			gameObject.SetActive(true);
			StartCoroutine(UnloadRoutine());
			StartCoroutine(Smoother());
		}
		
	#endregion
	
	#region Coroutines
		
		IEnumerator LoadRoutine(){
			yield return null;
			
			operation = SceneManager.LoadSceneAsync(sceneIndex, mode);
			yield return Operate();
		}
		
		IEnumerator UnloadRoutine(){
			yield return null;
			
			operation = SceneManager.UnloadSceneAsync(sceneIndex, options);
			yield return Operate();
		}
		
		IEnumerator Operate(){
			operation.allowSceneActivation = false;
			
			while(progress < 1f){
				progress = Mathf.Clamp01(operation.progress / 0.9f);
				Debug.Log(progress);
				yield return null;
			}
			
			yield return new WaitForSecondsRealtime(exitDelay);
			
				operation.allowSceneActivation = true;
			
			yield return null; // this will prevent skipping of start/awake/onenable calls of scripts in the newly loaded scene
			
			onFinish?.Invoke();
			gameObject.SetActive(false);
			
			progress = 0f; // reset
		}
		
	#endregion
	
	#region Design
		
		[SerializeField] float smoothTime = 0.12f;
		
		float
			progressSmooth,
			progressSmoothVel;
		
		IEnumerator Smoother(){
			// Reset values (values might remembered from last use of this script)
				progressSmooth = 0f;
				progressSmoothVel = 0f;
			
			while(progressSmooth < 1f){
				progressSmooth = Mathf.SmoothDamp(
					progressSmooth,
					progress,
					ref progressSmoothVel,
					smoothTime
				);
			
				progressBar.fillAmount = progressSmooth;
				progressValue.text = Mathf.RoundToInt(progressSmooth * 100f).ToString();
			
				yield return null;
			}
		}
		
	#endregion
}