using UnityEngine;

public class Singleton<T> : ScriptableObject where T : Singleton<T>
{
	static T instance;
	const string directory = "Managers";
	
	public static T Instance{
		get{
			if(!instance){
				var assets = Resources.LoadAll<T>(directory + "/");
				int count = assets.Length;
				
				if(assets == null || count < 1)
					throw new System.Exception("Could not find T instance in the resources");
				
				else if(count > 1)
					throw new System.Exception("Multiple instances of T are found in the resources");
				
				instance = assets[0];
			}
			
			return instance;
		}
	}
}