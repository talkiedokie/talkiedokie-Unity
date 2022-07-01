using UnityEngine;
using UnityEngine.Events;

namespace Prototype.Cars
{
	public class CarHonkTrigger : MonoBehaviour
	{
		public UnityEvent onCarHonk;
		
		public void OnCarHonk() => onCarHonk?.Invoke();
	}
}