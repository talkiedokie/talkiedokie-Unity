using UnityEngine;
using UnityEngine.Events;

namespace Prototype.TrafficSystems
{
	public class CarHonkTrigger : MonoBehaviour
	{
		public UnityEvent onCarHonk;
		
		public void OnCarHonk() => onCarHonk?.Invoke();
	}
}