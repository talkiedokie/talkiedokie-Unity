using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public partial struct SceneLoader
{
	#region Instance
		
		public string
			label,
			currentSceneName; // for debugging (if the scenes in build settings are modified)
		
		public int index;
		
		#region Properties
			
			public Scene Reference{ get; private set; }
			public bool isLoaded => Reference.isLoaded;
			
		#endregion
		
		#region Default Methods
			
			public void Load(){
				SceneManager.LoadScene(index);
			}
			
			public void Load(LoadSceneMode mode){
				SceneManager.LoadScene(index, mode);
			}
			
		#endregion
		
	#endregion
	
	#region Constructors
		
		public SceneLoader(string _label){
			label = _label;
			index = 0;
			
			currentSceneName = "";
			
			Reference = SceneManager.GetSceneByBuildIndex(index);
		}
		
		public SceneLoader(int _index){
			label = "";
			index = _index;
			
			currentSceneName = "";
			
			Reference = SceneManager.GetSceneByBuildIndex(index);
		}
		
		public SceneLoader(string _label, int _index){
			label = _label;
			index = _index;
			
			currentSceneName = "";
			
			Reference = SceneManager.GetSceneByBuildIndex(index);
		}
		
		public SceneLoader(string _label, string _currentSceneName){
			label = _label;
			index = 0;
			
			currentSceneName = _currentSceneName;
			
			Reference = SceneManager.GetSceneByBuildIndex(index);
		}
		
	#endregion
	
	#region Static
	
		static int currentIndex{
			get{ return SceneManager.GetActiveScene().buildIndex; }
		}
		
		public static void Load(int index){
			SceneManager.LoadScene(index);
		}
		
		public static void Load(int index, LoadSceneMode mode){
			SceneManager.LoadScene(index, mode);
		}
		
		public static void Load(string name){
			SceneManager.LoadScene(name);
		}
		
		public static void Load(string name, LoadSceneMode mode){
			SceneManager.LoadScene(name, mode);
		}
		
		public static void Current(){
			Load(currentIndex);
		}
		
	#endregion
}