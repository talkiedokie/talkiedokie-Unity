using UnityEngine;

namespace Prototype.Cars
{
	public class StopLight : MonoBehaviour
	{
		public GameObject red, yellow, green;
		public CarTargetPoint affectedPoint;
		
		[Space()]
		public float gizmoOffset = 2f;
		public float gizmoRadius = 2f;
		
		public Color color = Color.white;
		
		public void IsGreen(bool b){
			green.SetActive(b);
			red.SetActive(!b);
			
			affectedPoint.stop = !b;
			
			color = b? Color.green: Color.red;
		}
		
		void OnDrawGizmos(){
			Gizmos.color = color;
			Gizmos.DrawSphere(transform.position + Vector3.up * gizmoOffset, gizmoRadius);
		}
	}
}