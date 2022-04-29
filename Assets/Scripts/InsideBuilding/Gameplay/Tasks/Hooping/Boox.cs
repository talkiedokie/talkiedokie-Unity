using UnityEngine;
using System.Collections;

namespace Gameplay
{
	public class Boox : Hooping
	{
		public Layer[] layers;
		public int maxIteration = 100;
		
		protected override void OnEnable(){
			base.OnEnable();
			
			foreach(var layer in layers)
				layer.OnEnable();
		}
		
		public void OnDrop(){
			var layer = Tools.Random(layers);
			bool success = false;
			int iteration = 0;
			
			while(!success && iteration < maxIteration){
				layer.AddBook(out success);
				iteration ++;
			}
			
			Debug.Log(iteration, this);
		}
		
		new public void Score(){
			StartCoroutine(delayOneFrame());
			
			IEnumerator delayOneFrame(){
				yield return null;
				
				selected.gameObject.SetActive(false);
				selected = null;
			}
			
			score ++;
			if(score >= maxScore) CompleteTask();
		}
		
		[System.Serializable]
		public class Layer{
			public GameObject[] objects;
			int currentIndex;
			
			public void OnEnable(){
				foreach(var obj in objects)
					obj.SetActive(false);
			}
			
			public void AddBook(out bool success){
				var obj = objects[currentIndex];
				obj.SetActive(true);
				
				currentIndex ++;
				
				int length = objects.Length;
				success = currentIndex < length;
				currentIndex = Mathf.Clamp(currentIndex, 0, length - 1);
			}
			
			public void SubtractBook(out bool success){
				var obj = objects[currentIndex];
				obj.SetActive(false);
				
				currentIndex --;
				
				int length = objects.Length;
				success = currentIndex > 0;
				currentIndex = Mathf.Clamp(currentIndex, 0, objects.Length - 1);
			}
		}
	}
}