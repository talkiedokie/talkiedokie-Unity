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
				
				var type = typeof(T);
				string typeName = type.ToString();
				
				if(instances == null || count < 1)
					throw new System.Exception("Could not find '" + typeName + "' instance");
				
				else if(count > 1)
					throw new System.Exception("Multiple instances of '" + typeName + "' are found");
				
				instance = instances[0];
			}
			
			return instance;
		}
		
		protected set{ instance = value; }
	}
	
	protected virtual void Awake(){
		instance = instance?? (T) this;
	}
}