using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Minigame.Conveyor
{
	public partial class GameManager : SceneObjectSingleton<GameManager>
	{
		[Foldout("References")]
		[SerializeField] Item[] items;
		[SerializeField] Basket[] baskets;
		
		[Foldout("Item Spawn")]
		[SerializeField] Animator roboticArmAnim;
		public float itemSpawnDelay = 1f;
		
			IEnumerator itemSpawnRoutine;
			List<Item> spawnedItems = new List<Item>();
			
			public Basket targetBasket{
				get{
					return
						spawnedItems.Count < 1? null:
						spawnedItems[0].basket;
				}
			}
			
			Basket selectedBasket;
		
		[Foldout("Scoring")]
		[SerializeField] int maxScore = 10;
		[SerializeField] Text scoreTxt;
		
			public int MaxScore => maxScore;
			public int score{ get; private set; }
			
			public int coinRewards = 20;
			public Action onScoreUpdate, onItemBroken, onWinning;
		
		[Foldout("Effects")]
		[SerializeField] GameObject[] particles;
		
		[SerializeField] GeneralAudioSelector
			spawnSound,
			correctSound,
			wrongSound;
		
		[Space()]
		[SerializeField] GameObject[] winParticles;
		[SerializeField] GeneralAudioSelector winSound;

		public bool donePlaying;
		[ContextMenu("New Item")]
		public Item NewItem(){

			Item newItem = null;
			
			if(itemSpawnRoutine != null || donePlaying)
				return newItem;
			
			roboticArmAnim.SetTrigger("move");
			
			newItem = Tools.Random(items).Instantiate();
				spawnedItems.Add(newItem);
				newItem.gameObject.SetActive(false);
			
			itemSpawnRoutine = routine();
			StartCoroutine(itemSpawnRoutine);
			
			IEnumerator routine(){
				yield return new WaitForSeconds(itemSpawnDelay);
				
				newItem.gameObject.SetActive(true);
				
				spawnSound.PlayAdditive();
				SpawnRandomParticle(newItem.transform.position);
				
				itemSpawnRoutine = null;
			}
			
			return newItem;
		}
		
		public Basket FindBasket(string speechResult){
			Basket output = null;
			
			foreach(var basket in baskets){
				string basketName = basket.name.ToLower();
				// Debug.Log(basketName + " " + speechResult);
				
				if(speechResult.Contains(basketName)){
					output = basket;
					break;
				}
				
				else{
					foreach(var sl in basket.soundsLike){
						string soundLike = sl.ToLower();
						
						if(speechResult.Contains(soundLike)){
							output = basket;
							break;
						}
					}
				}
			}
			
			return output;
		}
		
		public void SelectBasket(Basket basket){
			if(spawnedItems.Count == 0) return;
			
			var item = spawnedItems[0];
				
				if(item){
					basket.OpenLid();
					
					item.SendTo(basket);
					selectedBasket = basket;
				}

			spawnedItems.RemoveAt(0);
		}
		
		public void Score(int amount){
			score += amount;
			score = Mathf.Clamp(score, 0, int.MaxValue);
			
			if(score >= maxScore){
				Tools.Random(winParticles).SetActive(true);
				winSound.Play();
				
				foreach(var item in spawnedItems)
					item.enabled = false;

				
				onWinning?.Invoke();
			}
			
			else if(amount > 0){
				correctSound.PlayAdditive();
				SpawnRandomParticle(selectedBasket.transform.position);
			}
			
			else wrongSound.PlayAdditive();
			
			scoreTxt.text = score.ToString();
			onScoreUpdate?.Invoke();
		}
		
		public void OnItemBroken(Item item){
			if(spawnedItems.Contains(item))
				spawnedItems.Remove(item);
			
			onItemBroken?.Invoke();
		}
		
		void SpawnRandomParticle(Vector3 position){
			var prefab = Tools.Random(particles);
			var rotation = Quaternion.identity;
			
			var particle = Instantiate(prefab, position, rotation);
				Destroy(particle, 2f);
		}
	}
}