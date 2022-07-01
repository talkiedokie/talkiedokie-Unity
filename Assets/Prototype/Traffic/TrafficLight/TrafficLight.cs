using UnityEngine;

namespace Prototype.TrafficSystems
{
	public class TrafficLight : MonoBehaviour
	{
		[SerializeField] Waypoint[] affectedPoints;
		[SerializeField] GameObject[] lights;
		
		public enum State{ Red, Yellow, Green }
		public State state;
		
		public void SetState(State state){
			switch(state){
				case State.Red:
					foreach(var point in affectedPoints)
						point.isOpen = false;
					
					break;
				
				case State.Yellow:
					break;
				
				case State.Green:
					foreach(var point in affectedPoints)
						point.isOpen = true;
					
					break;
			}
			
			this.state = state;
			
			for(int i = 0; i < 3; i ++)
				lights[i].SetActive(i == (int) state);
		}
		
		public void IsGreen(bool isGreen){
			var state = isGreen? State.Green: State.Red;
				SetState(state);
		}
	}
}