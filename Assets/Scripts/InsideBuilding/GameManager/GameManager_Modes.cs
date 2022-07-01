using UnityEngine;
using UnityEngine.UI;

namespace InsideBuilding
{
	public enum GameMode{ StepByStep, Free }
	
	public partial class GameManager
	{
		[Foldout("Game Mode")]
		[SerializeField, LabelOverride("Value")] GameMode gameMode;
		[SerializeField, LabelOverride("Dropdown UI")] Dropdown gameModeDrp;
		
		public void SetGameMode(GameMode mode){
			gameMode = mode;
			
			if(!fpCtrl)
				fpCtrl = player.GetComponent<FirstPersonController>();
			
			switch(gameMode){
				case GameMode.StepByStep:
					fpCtrl.enabled = false;
					var playerT = player.transform;
					
					var charPoint = selectedRoom?
						selectedRoom.playerPoint:
						playerDefaultPoint;
					
						SetOrientation(playerT, charPoint);
					
					fairy.Appear();
				
				break;
				
				case GameMode.Free:
					fairy.Disappear();
					fpCtrl.enabled = true;
					
					speech.StopListening();
			
				break;
			}
			
			gameModeDrp.SetValueWithoutNotify((int) mode);
			onGameModeChanged?.Invoke(mode);
		}
		
		public void SetGameMode(int index) =>
			SetGameMode((GameMode) index);
	}
}