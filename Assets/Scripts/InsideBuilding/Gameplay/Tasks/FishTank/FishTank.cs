using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Gameplay
{
	public class FishTank : Task
	{
		public Aquarium aquarium;
		
		[Range(0,1)]
		public float minWaterLevel = 0.25f;
		public float siphonDuration = 5f, swapDuration = 0.25f;
		
		[Space()]
		public Transform bucketA;
		public Transform bucketB;
		public Vector2 bucketWaterLevelMinMax;
		public Transform exit;
		
		[Space()]
		public GameObject ui;
		public Button removeButton, addButton;
		
		IEnumerator removeRoutine, addRoutine;
		
		public override void Play(bool b){
			base.Play(b);
			
			removeButton.gameObject.SetActive(b);
			addButton.gameObject.SetActive(!b);
			
			if(b) UIManager.Instance.Show(ui, ShowType.Single);
			else UIManager.Instance.Hide(ui);
		}
		
		public void OnRemoveButton(){
			if(removeRoutine != null) return;
			
			removeRoutine = RemoveRoutine();
			StartCoroutine(removeRoutine);
		}
		
		public void OnAddButton(){
			if(addRoutine != null) return;
			
			addRoutine = AddRoutine();
			StartCoroutine(addRoutine);
		}
		
		IEnumerator RemoveRoutine(){
			removeButton.gameObject.SetActive(false);
			
			float timer = 0f;
			var bucketWater = bucketA.GetChild(0);
			var bucketWaterPos = bucketWater.localPosition;
			
			while(timer < siphonDuration){
				float lerp = timer / siphonDuration;
				
				aquarium.waterLevel = Mathf.Lerp(1, minWaterLevel, lerp);
				
				bucketWater.localPosition = new Vector3(
					bucketWaterPos.x,
					bucketWaterPos.y,
					Mathf.Lerp(bucketWaterLevelMinMax.x, bucketWaterLevelMinMax.y, lerp)
				);
				
				timer += Time.deltaTime;
				yield return null;
			}
			
			addButton.gameObject.SetActive(true);
		}
		
		IEnumerator AddRoutine(){
			addButton.gameObject.SetActive(false);
			
			var bucketAStartPos = bucketA.position;
			var bucketBStartPos = bucketB.position;
			
			float timer = 0f;
			
			while(timer < swapDuration){
				var lerp = timer / swapDuration;
				
				bucketA.position = Vector3.Lerp(bucketAStartPos, exit.position, lerp);
				bucketB.position = Vector3.Lerp(bucketBStartPos, bucketAStartPos, lerp);
				
				timer += Time.deltaTime;
				yield return null;
			}
			
			timer = 0f;
			float currentWaterLevel = aquarium.waterLevel;
			
			var bucketWater = bucketB.GetChild(0);
			var bucketWaterPos = bucketWater.localPosition;
			
			while(timer < siphonDuration){
				float lerp = timer / siphonDuration;
				aquarium.waterLevel = Mathf.Lerp(currentWaterLevel, 1, lerp);
				
				bucketWater.localPosition = new Vector3(
					bucketWaterPos.x,
					bucketWaterPos.y,
					Mathf.Lerp(bucketWaterLevelMinMax.y, bucketWaterLevelMinMax.x, lerp)
				);
				
				timer += Time.deltaTime;
				yield return null;
			}
			
			CompleteTask();
		}
	}
}