using UnityEngine;

public class SceneObjectSingleton<T> : MonoBehaviour where T : SceneObjectSingleton<T>
{
	static T instance;
	const bool includeInactive = true;
	
	public static T Instance{
		get{
			if(!instance){
				var instances = FindObjectsOfType<T>(includeInactive);
				int count = instances.Length;
				
				if(instances == null || count < 1)
					// Debug.LogWarning("Could not find T instance");
					throw new System.Exception("Could not find T instance");
				
				else if(count > 1)
					// Debug.LogWarning("Multiple instances of T are found");
					throw new System.Exception("Multiple instances of T are found");
				
				instance = instances[0];
			}
			
			return instance;
		}
		
		protected set{ instance = value; }
	}
}