using UnityEngine;
using System.Collections;

namespace InsideBuilding
{
	public partial class GameManager
	{
		[Foldout("Rooms")]
		[SerializeField, LabelOverride("Selector UI")] GameObject roomSelector;
		[SerializeField, LabelOverride("Button Template")] RoomButton roomButtonTemplate;
		
		[SerializeField] GeneralAudioSelector roomEnterSound;
		
		RoomButton[] roomButtons;
		Room selectedRoom;
		
		public bool useSpeechOnRoomSelect{ get; set; } = true;
		
		public Room SelectedRoom{
			get{
				if(!selectedRoom)
					selectedRoom = level.rooms[selectedRoom_Index];
				
				return selectedRoom;
			}
			
			set{ selectedRoom = value; }
		}

		int selectedRoom_Index{
			get{ return level.selectedRoomIndex; }
			set{ level.selectedRoomIndex = value; }
		}
		
		[ContextMenu("Select Rooms")]
		public void SelectRoom(){
			fairy.Speak(
				wdywtg_Clip, 0.1f,
				
				()=>{
					uiMgr.Show(roomSelector, single);
				});
		}
		
		public void OnRoomSelected(int index){ // button call
			if(selectedRoom && index == selectedRoom_Index){
				Debug.LogWarning("You're in the " + selectedRoom.name + " already!", selectedRoom);
				return;
			}
			
			// Refs
				SelectedRoom = level.rooms[index];
				selectedRoom_Index = index;
			
			// Events
				if(useSpeechOnRoomSelect){
					var clips = new AudioClip[]{ say_Clip, SelectedRoom.RoomNameFairyVoiceClip };
					fairy.Speak(clips, 0.2f, SpeechListen);
				}
				
				else{
					GoToTheRoom();
					OnRoomEntered();
				}
				
			
			void SpeechListen(){
				WordPopup_Show(SelectedRoom.name, null);
				StartCoroutine(delay());
				
				IEnumerator delay(){
					yield return new WaitForSeconds(0.5f);
					
					fairy.ListenToSpeech( // Double event, on Speech Listen Recognize & on Speech Listen Finished
						SelectedRoom.name,
						GoToTheRoom, // on SpeechListen Recognize
						OnRoomEntered
					);
				}
			}
			
			void GoToTheRoom(){
				// UI
					WordPopup_Correct();
				
				StartCoroutine(delay());
				
				IEnumerator delay(){
					yield return new WaitForSeconds(1.25f);
					
					// Characters
						SetOrientation(fairy.transform, SelectedRoom.fairyPoint);
						SetOrientation(player.transform, SelectedRoom.playerPoint);
					
					// Transition
						camMgr.SetPriority(selectedRoom.CameraPoint);
						
						uiMgr.Transition();
						roomEnterSound.Play();
						
						SelectedRoom.OnEnter(); // play cam animation (ease-in)
				}
			}
			
			// UI
				for(int i = 0; i < roomButtons.Length; i++)
					roomButtons[i].IsInteractable(i != selectedRoom_Index);
				
				uiMgr.Hide(roomSelector);
		}
	}
}