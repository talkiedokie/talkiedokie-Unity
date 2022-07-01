using UnityEngine;
using Cinemachine;

namespace InsideBuilding
{
	public class Room : MonoBehaviour
	{
		[SerializeField] AudioClip roomNameFairyVoiceClip;
		[SerializeField] CinemachineVirtualCamera camPoint;
		[SerializeField] Animator camAnimator;
		
		public Transform fairyPoint, playerPoint;
		
		[Space()] public Task[] tasks;
		
		#region Properties
			
			public AudioClip RoomNameFairyVoiceClip => roomNameFairyVoiceClip;
			public CinemachineVirtualCamera CameraPoint => camPoint;
			
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
			
			instance.SetRoom(this);
			instance.gameObject.SetActive(true);
			
			return instance;
		}
		
		public void Start(){
			foreach(var task in tasks)
				task.gameObject.SetActive(false);
		}
		
		public void OnEnter() =>
			camAnimator.SetTrigger("enter");
		
		public void ShowCharacters(bool b){
			fairyPoint.gameObject.SetActive(b);
			playerPoint.gameObject.SetActive(b);
		}
	}
}