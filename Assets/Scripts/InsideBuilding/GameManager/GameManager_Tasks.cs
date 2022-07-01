using UnityEngine;
using System.Collections;

namespace InsideBuilding
{
	public partial class GameManager
	{
		[Foldout("Tasks")]
		[SerializeField, LabelOverride("UI Template")] RoomTasksUI roomTasksUITemplate;
		RoomTasksUI[] roomTasksUI;
		
		[Space()]
		[SerializeField, LabelOverride("Exit Duration")] float onTaskExitDuration = 2.25f;
		[SerializeField] GeneralAudioSelector onTaskFinishSound = 4;
		
		[Space()]
		[SerializeField, LabelOverride("Particle Point")] Transform taskParticlePoint;
		
		[SerializeField] GameObject[]
			onTaskEnterParticles,
			onTaskFinishParticles;
		
		[SerializeField] GameObject coinsParticles;
		
		[Space()]
		[SerializeField] GeneralAudioSelector onTaskEnterTransitionSound;
		[SerializeField] GeneralAudioSelector onTaskExitTransitionSound;
		
		Task currentTask;
		public Task CurrentTask => currentTask;
		
		[ContextMenu("Select Tasks")]
		public void SelectTasks(){			
			if(!selectedRoom){
				Debug.LogWarning("There's no room selected");
				SelectRoom();
				return;
			}
			
			if(!SelectedRoom.HasTasks){
				Debug.LogWarning(SelectedRoom.name + " has no tasks.", SelectedRoom);
				FinishTask();
				return;
			}
			
			var uiPopup = roomTasksUI[selectedRoom_Index].gameObject;
				uiMgr.Show(uiPopup, single);			
		}
		
		public void OnTaskSelected(int index){
			var task = selectedRoom.tasks[index];
			var taskDescription = task.description;
			
				roomTasksUI[selectedRoom_Index].OnTaskSelected(index);
				fairy.Speak(taskDescription.fairyVoiceClip, 0.15f, PlayTask);
			
			void PlayTask(){
				currentTask = SelectedRoom.InstantiateTask(index);
				if(!currentTask) return;
			
				selectedRoom.ShowCharacters(false);
				
				currentTask.gameObject.SetActive(true);
				currentTask.Play(true);
				
				SpawnParticle(onTaskEnterParticles, taskParticlePoint);	
				onTaskEnterTransitionSound.PlayAdditive();
				uiMgr.Hide(roomTasksUI[selectedRoom_Index].gameObject);
				
				Debug.Log(taskDescription.text);
				onTaskSelected?.Invoke();
			}
		}
		
		public void CancelTask() =>
			currentTask?.CancelTask();
	
		public void FinishTask(){
			StartCoroutine(routine());
			
			IEnumerator routine(){
				bool isCompleted = !currentTask.isCanceled;
				
				// Rewards
					if(isCompleted){
						SpawnParticle(onTaskFinishParticles, taskParticlePoint);
						onTaskFinishSound.PlayAdditive();
						
						yield return new WaitForSeconds(1f);
						
							var coinHUD = CoinHUD.Instance;
							{
								coinHUD.AddCoin(currentTask.rewards);
								while(coinHUD.isUpdatingAmount) yield return null;
							}
							
							coinsParticles.SetActive(true);
							
							var recapCamAnim = camMgr.Current.GetComponent<Animator>();
							if(recapCamAnim) recapCamAnim.Play("zoom");
					}
				
				yield return new WaitForSeconds(onTaskExitDuration);
				
				// Reset
					uiMgr.Transition();
					onTaskExitTransitionSound.PlayAdditive();
					
					camMgr.SetPriority(selectedRoom.CameraPoint);
					
					currentTask.ResetPlayerPosition();
					selectedRoom.ShowCharacters(true);
					
					selectedRoom.OnEnter();
				
				// Proceed
					if(isCompleted) fairy.SayWow(0.5f, OnTaskFinished);
					else fairy.SayFail(0.5f, OnTaskFinished);
				
				// Cleanup
					if(currentTask)
						Destroy(currentTask.gameObject);
					
					yield return new WaitForSeconds(1);
					coinsParticles.SetActive(false);
			}
			
			uiMgr.SetExclamationPoint(Vector3.zero, false);
		}
		
		void SpawnParticle(
			GameObject[] array,
			Transform targetPoint,
			float durationSeconds = 3f
		){
			var prefab = Tools.Random(array);
			
			var particle = Instantiate(
				prefab,
				targetPoint.position,
				targetPoint.rotation,
				targetPoint
			);
			
			Destroy(particle, durationSeconds);
		}
	}
}