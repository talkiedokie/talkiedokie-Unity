using UnityEngine;

namespace Minigame{
	namespace Conveyor
	{
		[RequireComponent(typeof(Collider))]
		public class Basket : MonoBehaviour
		{
			[SerializeField] Animator anim;
			[SerializeField] GeneralAudioSelector sound;
			
			public string[] soundsLike;
			
			[Foldout("MSC")]
			public Transform lidTarget;
			public Tweener lidTweener;
			Transform lidDefaultPoint;
			
			void Start(){
				lidDefaultPoint = lidTweener.CreateDefaultPoint();
			}
			
			void OnTriggerEnter(Collider col){
				var item = col.GetComponent<Item>();
					item?.OnBasketEnter(this);
			
				anim.SetTrigger("pop");
				sound.PlayAdditive();
				
				lidTweener.SetTarget(lidDefaultPoint);
			}
			
			public void OpenLid(){
				lidTweener.SetTarget(lidTarget);
			}
		}
	}
}