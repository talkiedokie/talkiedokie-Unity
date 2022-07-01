using System.Collections;
using UnityEngine;

namespace Prototype.Cars
{
	public class CarSpawner : MonoBehaviour
	{
		public Car[] cars;
		public float offset = 3f;
		
		[ContextMenu("Setup")]
		public void Awake(){
			var wayPoints = WayPointDrawer.Instance.targetPoints;
			
			foreach(var car in cars){
				var wayPoint = Tools.Random(wayPoints);
					car.targetPoint = wayPoint;
				
				car.transform.position = wayPoint.position - car.transform.forward * offset;
				car.transform.rotation = Quaternion.LookRotation(wayPoint.position - car.transform.position);;
			}
		}
		
		void Start(){
			foreach(var car in cars)
				car.gameObject.SetActive(true);
		}
	}
}