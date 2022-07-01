using UnityEngine;

public partial class Spline : MonoBehaviour
{
	public ConnectedPoint[] connectedPoints;
	
	Transform t;
	public Transform _transform{
		get{
			if(!t) t = transform;
			return t;
		}
	}
	
	public Vector3 position => _transform.position;
	
	[System.Serializable]
	public class ConnectedPoint{
		[HideInInspector] public string name;
		
		public Spline target;
		
		public enum InterpolationType{ Linear, Quadratic, Cubic }
		public InterpolationType interpolationType;
		
		public const InterpolationType
			LIN = InterpolationType.Linear,
			QUA = InterpolationType.Quadratic,
			CUB = InterpolationType.Cubic;
		
		public Vector3 handleA, handleB;
		public float length{ get; private set; }
		
		public Vector3 GetPosition(Vector3 start, float t){
			Vector3 output = Vector3.zero;
			Vector3 targetPosition = target.position;
			
			switch(interpolationType){
				case LIN:
					output = GetLinear(start, targetPosition, t);
					break;
				
				case QUA:
					output = GetQuadratic(start, handleA, targetPosition, t);
					break;
				
				case CUB:
					output = GetCubic(start, handleA, handleB, targetPosition, t);
					break;
			}
			
			return output;
		}
		
		public Vector3 GetPositionNormalized(Vector3 start, float space){
			return Vector3.zero;
		}
		
		Vector3 GetLinear(Vector3 a, Vector3 b, float t){
			return Vector3.Lerp(a, b, t);
		}
		
		Vector3 GetQuadratic(Vector3 a, Vector3 b, Vector3 c, float t){
			var posA = GetLinear(a, b, t);
			var posB = GetLinear(b, c, t);
			return GetLinear(posA, posB, t);
		}
		
		Vector3 GetCubic(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t){
			var posA = GetQuadratic(a, b, c, t);
			var posB = GetQuadratic(b, c, d, t);
			return GetLinear(posA, posB, t);
		}
		
		public void RecalculateLength(
			Vector3 startPosition,
			float resolution
		){
			var previous = startPosition;
			
			for(float i = 1f; i < resolution; i ++){
				var current = GetPosition(startPosition, i / resolution);
				length += (current - previous).magnitude;
				previous = current;
			}
		}
	}
}