using UnityEngine;
using System.Collections;

namespace InsideBuilding.Gameplay
{
	public class Boox : Hooping
	{
		public Bookshelf bookshelf;
		public Interactable[] bookPrefabs;
		
		[Range(0,1)]
		public float maxSpawnPercent = 0.5f;
		public SpawnInsideSphere spawnedBooksParent;
		
		int numOfSlots;
		
		public override void Play(bool b){
			if(b){
				foreach(var layer in bookshelf.layers){
					layer.EmptyAllSlots();
					numOfSlots += layer.slots.Length;
				}
				
				int spawnRate = Mathf.RoundToInt((float) numOfSlots * maxSpawnPercent);
				int numberOfBookSpawn = Random.Range(0, spawnRate);
				
				interactableObjects = new Interactable[numberOfBookSpawn];
				var spawnedBooksParent = this.spawnedBooksParent.transform;
				
				for(int i = 0; i < numberOfBookSpawn; i++){
					var prefab = Tools.Random(bookPrefabs);
					interactableObjects[i] = Instantiate(prefab, spawnedBooksParent, false);
				}
				
				this.spawnedBooksParent.Spawn();
				Debug.Log(numberOfBookSpawn +"/"+ numOfSlots +" books spawned");
			}
			
			base.Play(b);
		}
		
		protected override void OnDrop(RaycastHit rayInfo){
			var book = dragged;
			var layer = rayInfo.collider.GetComponent<BookShelfLayer>();
			bool isBookAdded = false;
			
			layer?.AddObject(book.transform, out isBookAdded);
			
			if(book.TryGetComponent<Rigidbody>(out var rb))
				rb.isKinematic = true;
			
			
			if(isBookAdded){
				base.OnDrop(rayInfo);
				dragged = null;
			}
		}
	}
}