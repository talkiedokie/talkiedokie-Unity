using UnityEngine;
using System.Collections;

namespace InsideBuilding.Gameplay
{
	public class Closet : MonoBehaviour
	{
		public Transform orientationRef;
		
		public float maxDistance = 1f;
		public enum Direction{ Right, Up, Forward }
		public Direction direction = Direction.Forward;
		
		public void ArrangeChildren(){
			StartCoroutine(oneFrameDelay());
			
			IEnumerator oneFrameDelay(){
				yield return null;
				
				var direction = GetDirection();
				
				for(int i = 0; i < transform.childCount; i++){
					var child = transform.GetChild(i);
						child.position = orientationRef.position;
						child.rotation = orientationRef.rotation;
				}
			}
		}
		
		Vector3 GetDirection(){
			var output = Vector3.zero;
			
			switch(direction){
				case (Direction) 0: output = transform.right; break;
				case (Direction) 1: output = transform.up; break;
				case (Direction) 2: output = transform.forward; break;
			}
			
			return output;
		}
	}
}