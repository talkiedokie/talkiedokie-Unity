using UnityEngine;
using UnityEngine.SceneManagement;

public partial struct SceneLoader
{
	#region Visualizer Reference
		
		static SceneLoaderProgress visualizer;
		
		static SceneLoaderProgress Visualizer{
			get{			
				if(!visualizer){
					// Create Instance
						var prefab = Resources.Load<SceneLoaderProgress>("Prefabs/UI/sceneLoaderProgress");
							visualizer = Object.Instantiate(prefab);
					
					// Handle Duplications
						var slp = MonoBehaviour.FindObjectsOfType<SceneLoaderProgress>(true);
						
						foreach(var s in slp)
							if(s != visualizer)
								UnityEngine.Object.DestroyImmediate(s.gameObject);
					
					Object.DontDestroyOnLoad(visualizer);
				}
				
				return visualizer;
			}
		}
		
	#endregion
	
	#region Overloads
		
		public void LoadAsync(){
			Visualizer.Load(index, LoadSceneMode.Single, null);
		}
		
		public void LoadAsync(System.Action onFinish){
			Visualizer.Load(index, LoadSceneMode.Single, onFinish);
		}
		
		public void LoadAsync(LoadSceneMode mode){
			Visualizer.Load(index, mode, null);
		}
		
		public void LoadAsync(LoadSceneMode mode, System.Action onFinish){
			Visualizer.Load(index, mode, onFinish);
		}
		
		public AsyncOperation LoadAsyncOperation(LoadSceneMode mode){
			return SceneManager.LoadSceneAsync(index, mode);
		}
		
		public void Unload(){
			Visualizer.Unload(index, UnloadSceneOptions.None);
		}
		
		public void Unload(UnloadSceneOptions options){
			Visualizer.Unload(index, options);
		}
		
	#endregion
}