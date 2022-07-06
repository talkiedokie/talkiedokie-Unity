using UnityEngine;

namespace Minigame{
	namespace Conveyor
	{
		[RequireComponent(typeof(Rigidbody))]
		[RequireComponent(typeof(Tweener))]
		public class Item : MonoBehaviour
		{
			public Basket basket;
			public float speed = 1f;
			
			[SerializeField] Transform broken;
			
			[SerializeField] Rigidbody rb;
			[SerializeField] Tweener tweener;
			
			static GameManager gameMgr => GameManager.Instance;
			
			void FixedUpdate(){
				if(!rb.isKinematic)
					rb.velocity = speed * new Vector3(1f, rb.velocity.y, 0f);
			}
			
			public Item Instantiate(){
				var instance = Instantiate(this, transform.position, transform.rotation);
					instance.gameObject.SetActive(true);
					
				return instance;
			}
			
			public void SendTo(Basket basket){
				rb.isKinematic = true;
				tweener.SetTarget(basket.transform);
			}
			
			public void OnBasketEnter(Basket basket){
				if(basket == this.basket) gameMgr.Score(1);
				else gameMgr.Score(0);
				
				Destroy(gameObject);
			}
			
			public void Break(){
				if(rb.isKinematic) return; // not breakable if kinmeatic (it means the tweener is traveling towards the basket)
				
				gameMgr.Score(0);
				
				if(broken){
					broken.SetParent(null);
					broken.gameObject.SetActive(true);
					Destroy(broken.gameObject, 2f);
				}
				
				gameMgr.OnItemBroken(this);
				Destroy(gameObject);
			}
		}
	}
}