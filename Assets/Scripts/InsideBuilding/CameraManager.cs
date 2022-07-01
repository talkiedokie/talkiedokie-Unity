using UnityEngine;
using Cinemachine;

public class CameraManager : SceneObjectSingleton<CameraManager>
{	
	[SerializeField] int
		priorValue = 11,
		nonPriorValue = 10;
	
	[SerializeField] CinemachineVirtualCamera priorityCamera;
	
	#if UNITY_EDITOR
	
		public CinemachineVirtualCamera setCamDebug;
		
		[ContextMenu("Set Camera")]
		public void SetCamDebug() => SetPriority(setCamDebug);
		
	#endif
	
	public CinemachineVirtualCamera defaultCamera{ get; private set; }
	public CinemachineVirtualCamera Current => priorityCamera;
	
	Camera cam;
	
	protected override void Awake(){
		base.Awake();
		
		defaultCamera = priorityCamera;
		cam = Camera.main;
	}
	
	public void SetPriority(CinemachineVirtualCamera cam){
		if(!cam) return;
		if(priorityCamera) priorityCamera.Priority = nonPriorValue;
		
		priorityCamera = cam;
		priorityCamera.Priority = priorValue;
	}
	
	public void SetDefaultPriority() => SetPriority(defaultCamera);
	
	public void AddCullingLayer(int layer){
		if(!cam) return;
		cam.cullingMask |= 1 << layer;
	}
	
	public void RemoveCullingLayer(int layer){
		if(!cam) return;
		cam.cullingMask &= ~(1 << layer);
	}
}