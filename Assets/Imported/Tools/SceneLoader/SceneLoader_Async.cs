using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public partial struct SceneLoader
{
	static SceneLoaderProgress visualizer;
	
	static SceneLoaderProgress Visualizer{
		get{
			if(!visualizer){
				visualizer = SceneLoaderProgress.Instance;
				
				// Handle Duplications
					var slp = MonoBehaviour.FindObjectsOfType<SceneLoaderProgress>(true);
						
					foreach(var s in slp)
						if(s != visualizer)
							UnityEngine.Object.DestroyImmediate(s.gameObject);
			}
			
			return visualizer;
		}
	}
	
	public void LoadAsync(){
		Visualizer.Load(index, LoadSceneMode.Single, null);
	}
	
	public void LoadAsync(Action onFinish){
		Visualizer.Load(index, LoadSceneMode.Single, onFinish);
	}
	
	public void LoadAsync(LoadSceneMode mode){
		Visualizer.Load(index, mode, null);
	}
	
	public void LoadAsync(LoadSceneMode mode, Action onFinish){
		Visualizer.Load(index, mode, onFinish);
	}
	
	public void Unload(){
		Visualizer.Unload(index, UnloadSceneOptions.None);
	}
	
	public void Unload(UnloadSceneOptions options){
		Visualizer.Unload(index, options);
	}
}