using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace InsideBuilding.Gameplay
{
	public class Arrange : Task
	{
		[SerializeField] Bounds[] randomAreas;

		[SerializeField] Transform[] objects;
		List<Transform> _objects = new List<Transform>();
		
		
		[SerializeField] float objColliderRadius = 0.5f;
		
		Transform[] defaultPoints;
		int currentPoint;
		bool gate = true;
		
		[SerializeField] HoopUI _handUI;
		static HoopUI handUI;
		
		[Space()]
		[SerializeField] GameObject[] onClickParticles;
		[SerializeField] float particleDespawnTime = 2f;
		
		[SerializeField] GeneralAudioSelector
			onTargetSelectSound = 3,
			onTweenerSound = 8;
		
		public override void Play(bool b){
			base.Play(b);
			
			if(b){
				_objects = objects.ToList();
				
				int count = _objects.Count;
				defaultPoints = new Transform[count];
				
				for(int i = 0; i < count; i++){
					var obj = _objects[i];
					defaultPoints[i] = new GameObject(obj.name).transform;
					defaultPoints[i].parent = obj.parent;
					defaultPoints[i].position = obj.position;
					defaultPoints[i].rotation = obj.rotation;
				}
				
				ShowHandUI();
			}
			
			foreach(var obj in objects){
				if(b){
					var area = Tools.Random(randomAreas);
					
					var position = new Vector3(
						Random.Range(area.min.x, area.max.x),
						Random.Range(area.min.y, area.max.y),
						Random.Range(area.min.z, area.max.z)
					);
					
					obj.position = transform.position + position;
					obj.eulerAngles = Vector3.up * 360 * Random.value;
					
					var col = obj.gameObject.AddComponent<SphereCollider>();
						col.radius = objColliderRadius;
				}
				else{
					var col = obj.GetComponent<SphereCollider>();
					if(col) Destroy(col);
				}
			}
		}
		
		protected override void Update(){
			base.Update();
			
			if(Input.GetMouseButtonDown(0)){
				Ray ray = cam.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				
				if(Physics.Raycast(ray, out hit, 100)){
					Debug.Log(hit.transform.name, hit.transform);
					
					var target = _objects.Find(obj => obj == hit.transform);
					
					if(target){
						var tweener = target.GetComponent<Tweener>();
						
						if(tweener){
							tweener.SetTarget(defaultPoints[currentPoint]);
							currentPoint ++;
							
							if(currentPoint >= objects.Length && gate){
								CompleteTask();
								gate = false;
							}
							
							onTweenerSound.PlayAdditive();
						}
						
						onTargetSelectSound.PlayAdditive();
						
						var particlePrefab = Tools.Random(onClickParticles);
						
						var particle = Instantiate(
							particlePrefab,
							target.position,
							particlePrefab.transform.rotation
						);
						
						Destroy(particle, particleDespawnTime);
						_objects.Remove(target);
					}
				}
			}
		}
		
		void OnDrawGizmosSelected(){
			if(randomAreas == null) return;
			
			var pos = transform.position;
			Gizmos.color = Color.yellow;
			
			foreach(var area in randomAreas)
				Gizmos.DrawWireCube(pos + area.center, area.size);
		}
		
		void ShowHandUI(){
			if(!handUI)
				handUI = _handUI.CreateInstance();
			
			var startObjects = new Component[]{ Tools.Random(_objects) };
			handUI.Show(0.5f, 3, 1, startObjects, transform);
		}
	}
}