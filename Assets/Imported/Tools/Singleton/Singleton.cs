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
				
				var type = typeof(T);
				string typeName = type.ToString();
				
				if(assets == null || count < 1)
					throw new System.Exception("Could not find '" + typeName + "' instance in Resources/" + directory);
				
				else if(count > 1)
					throw new System.Exception("Multiple instances of '" + typeName + "' are found in Resources/" + directory);
				
				instance = assets[0];
			}
			
			return instance;
		}
	}
}