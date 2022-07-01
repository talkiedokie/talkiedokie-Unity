using UnityEngine;

namespace Prototype.Cars
{
	public class ColorRandomizer : MonoBehaviour
	{
		public Gradient gradient;
		public Renderer[] rends;
		
		void Start(){
			foreach(var rend in rends){
				var mat = rend.material;
					mat.color = gradient.Evaluate(Random.value);
			}
		}
	}
}