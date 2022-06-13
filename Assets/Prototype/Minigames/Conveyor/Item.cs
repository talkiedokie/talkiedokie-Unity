using UnityEngine;

namespace Minigame{
	namespace Conveyor
	{
		[RequireComponent(typeof(Rigidbody))]
		[RequireComponent(typeof(Tweener))]
		public class Item : MonoBehaviour
		{
			public Basket basket;
			[SerializeField] Transform broken;
			
			Rigidbody rb;
			Tweener tweener;
			
			static GameManager gameMgr;
			static ConveyorBelt belt;
			
			void Start(){
				rb = GetComponent<Rigidbody>();
				tweener = GetComponent<Tweener>();
				
				if(!gameMgr) gameMgr = GameManager.Instance;
				if(!belt) belt = ConveyorBelt.Instance;
			}
			
			void FixedUpdate(){
				if(!rb.isKinematic)
					rb.velocity = belt.ItemVelocity + (Vector3.up * rb.velocity.y);
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
				else gameMgr.Score(-1);
				
				Destroy(gameObject);
			}
			
			public void Break(){
				if(rb.isKinematic) return; // not breakable if kinmeatic (it means the tweener is traveling towards the basket)
				
				gameMgr.Score(-1);
				
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