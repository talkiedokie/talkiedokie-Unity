using UnityEngine;
using System.Collections;

namespace Prototype.Cars
{
	public class StopLightSystem : MonoBehaviour
	{
		public StopLight[] stopLights;
		public float duration = 10f;
		
		IEnumerator Start(){
			var step = new WaitForSeconds(duration);

			int count = stopLights.Length;
			int index = 0;
			
			while(true){
				for(int i = 0; i < count; i++)
					stopLights[i].IsGreen(index == i);
				
				index ++;
				index = index % count;
				
				yield return step;
			}
		}
	}
}