using UnityEngine;
using Cinemachine;

public class CameraManager : SceneObjectSingleton<CameraManager>
{	
	[SerializeField] int
		priorValue = 11,
		nonPriorValue = 10;
	
	[SerializeField] CinemachineVirtualCamera priorityCamera;
	
	public CinemachineVirtualCamera defaultCamera{ get; private set; }
	public CinemachineVirtualCamera Current => priorityCamera;
	
	void Awake(){
		defaultCamera = priorityCamera;
	}
	
	public void SetPriority(CinemachineVirtualCamera cam){
		if(priorityCamera) priorityCamera.Priority = nonPriorValue;
		
		priorityCamera = cam;
		priorityCamera.Priority = priorValue;
	}
	
	public void SetDefaultPriority(){
		SetPriority(defaultCamera);
	}
}