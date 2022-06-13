using UnityEngine;

public class City : MonoBehaviour
{
	public Transform player;
	
	public static Vector3 playerPosition;
	static Quaternion playerRotation;
	
	public static bool isLoaded{ get; private set; }
	static City instance;
	
	void Awake(){
		instance = this;
		
		player.position = playerPosition + Vector3.up;
		player.rotation = playerRotation;
		
		isLoaded = true;
	}
	
	public static void SetPlayerRotation(Quaternion rotation){
		if(instance) playerRotation = rotation;
	}
	
	#if UNITY_EDITOR
		
		public int frameRate = -1;
		
		[ContextMenu("Set Frame Rate")]
		public void SetFramerate(){
			if(Application.isPlaying)
				Application.targetFrameRate = frameRate;
		}
		
	#endif
}