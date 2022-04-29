using UnityEngine;
using Cinemachine;

public class Room : MonoBehaviour
{
	[SerializeField] AudioClip roomNameFairyVoiceClip;
	[SerializeField] Transform charPoint;
	[SerializeField] CinemachineVirtualCamera camPoint;
	
	[Space()] public Task[] tasks;
	
	#region Properties
		
		public AudioClip RoomNameFairyVoiceClip => roomNameFairyVoiceClip;
		public CinemachineVirtualCamera CameraPoint => camPoint;
		public Transform CharacterPoint => charPoint;
		
		public bool HasTasks{
			get{
				int length = tasks.Length;
				
				return length > 0;
			}
		}
		
	#endregion
	
	public Task InstantiateTask(int index){
		if(!HasTasks) return null;
		
		var task = tasks[index];
		var transform = task.transform;
		
		var instance = Instantiate(
			task,
			transform.position,
			transform.rotation,
			transform.parent
		);
		
		instance.gameObject.SetActive(true);
		
		return instance;
	}
	
	public void Start(){
		foreach(var task in tasks)
			task.gameObject.SetActive(false);
	}
}