using UnityEngine;
using UnityEngine.Serialization;

namespace Prototype.Cars
{
	public class CarTargetPoint : MonoBehaviour
	{
		public Transform _transform;
		public CarTargetPoint[] connectedPoints;
		
		public Color color = Color.yellow;
		
		public Vector3 position => _transform.position;
		public bool isJunction => connectedPoints.Length > 1;
		
		public bool stop;
		
		public CarTargetPoint GetNextPoint(){
			if(connectedPoints.Length == 0) return null;
			return Tools.Random(connectedPoints);
		}
		
		void OnValidate()=> _transform = transform;
		
		void OnDrawGizmos(){
			if(connectedPoints == null) return;
			Gizmos.color = color;
			
			foreach(var point in connectedPoints)
				if(point)
					Gizmos.DrawLine(position, point.position);
		}
	}
}