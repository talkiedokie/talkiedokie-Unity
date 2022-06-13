using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections;

namespace InsideBuilding.Gameplay
{
	public class FishTank : Task
	{
		public Aquarium aquarium;
		
		[Range(0,1)]
		public float minWaterLevel = 0.25f;
		
		public float
			siphonDuration = 5f,
			swapDuration = 0.25f;
		
		[Space()]
		public Transform bucketA;
		public Transform bucketB;
		
		public Transform waterA, waterB;
		public Vector2 bucketWaterLevelMinMax;
		public Transform exit;
		
		[Space()]
		public GameObject ui;
		public Button removeButton, addButton;
		public GeneralAudioSelector uiPopSound = 5;
		
		[Space()]
		public AudioSource siphonAudio;
		public GameObject stinkParticle, sparkle;
		
		public GeneralAudioSelector sparkleSound = 7;
		
		IEnumerator removeRoutine, addRoutine;
		GeneralAudio genAudio;
		
		protected override void Awake(){
			base.Awake();
			genAudio = GeneralAudio.Instance;
		}
		
		public override void Play(bool b){
			base.Play(b);
			
			if(b){
				StopAllCoroutines();
				StartCoroutine(ShowButtonDelay());
			}
			
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
		
		IEnumerator ShowButtonDelay(){
			yield return new WaitForSeconds(2f);
			
			UIManager.Instance.Show(ui, ShowType.Single);
			removeButton.gameObject.SetActive(true);
			addButton.gameObject.SetActive(false);
			
			genAudio.Play(uiPopSound);
		}
		
		IEnumerator RemoveRoutine(){
			removeButton.gameObject.SetActive(false);
			
			float timer = 0f;
			var bucketWaterPos = waterA.localPosition;
			
			siphonAudio.Play();
			
			while(timer < siphonDuration){
				float lerp = timer / siphonDuration;
				
				aquarium.waterLevel = Mathf.Lerp(1, minWaterLevel, lerp);
				
				waterA.localPosition = new Vector3(
					bucketWaterPos.x,
					bucketWaterPos.y,
					
					Mathf.Lerp(
						bucketWaterLevelMinMax.x,
						bucketWaterLevelMinMax.y, lerp
					)
				);
				
				timer += Time.deltaTime;
				yield return null;
			}
			
			stinkParticle.SetActive(false);
			
			if(bucketA.TryGetComponent<Animator>(out var anim))
				anim.SetTrigger("pop");
			
			sparkle.SetActive(true);
			genAudio.Play(sparkleSound);
			
			siphonAudio.Stop();
			
			yield return new WaitForSeconds(1.25f);
			
			addButton.gameObject.SetActive(true);
			genAudio.Play(uiPopSound);
			
			sparkle.SetActive(false);
		}
		
		IEnumerator AddRoutine(){
			addButton.gameObject.SetActive(false);
			
			var bucketAStartPos = bucketA.position;
			var bucketBStartPos = bucketB.position;
			
			float timer = 0f;
			
			siphonAudio.Play();
			
			while(timer < swapDuration){
				var lerp = timer / swapDuration;
				
				bucketA.position = Vector3.Lerp(bucketAStartPos, exit.position, lerp);
				bucketB.position = Vector3.Lerp(bucketBStartPos, bucketAStartPos, lerp);
				
				timer += Time.deltaTime;
				yield return null;
			}
			
			timer = 0f;
			float currentWaterLevel = aquarium.waterLevel;
			
			var bucketWaterPos = waterB.localPosition;
			
			while(timer < siphonDuration){
				float lerp = timer / siphonDuration;
				aquarium.waterLevel = Mathf.Lerp(currentWaterLevel, 1, lerp);
				
				waterB.localPosition = new Vector3(
					bucketWaterPos.x,
					bucketWaterPos.y,
					Mathf.Lerp(bucketWaterLevelMinMax.y, bucketWaterLevelMinMax.x, lerp)
				);
				
				timer += Time.deltaTime;
				yield return null;
			}
			
			if(bucketB.TryGetComponent<Animator>(out var anim))
				anim.SetTrigger("pop");
			
			sparkle.SetActive(true);
			genAudio.Play(sparkleSound);
			
			siphonAudio.Stop();
			
			yield return new WaitForSeconds(1.25f);
			CompleteTask();
		}
	}
}