using UnityEngine;
using System.Collections;

namespace InsideBuilding.Gameplay
{
	public class FishFeeding : Swerving
	{
		[SerializeField] Transform feed;
		[SerializeField] float feedDespawnTime = 3f;
		
		[SerializeField] Transform _ui;
		static Transform ui;
		
		public override void Play(bool b){
			base.Play(b);
			
			if(b)
				StartCoroutine(showUI());
		}
		
		protected override void OnProgressTrigger(){
			base.OnProgressTrigger();
			
			var feed = Instantiate(
				this.feed,
				this.feed.position,
				Random.rotation,
				transform
			);
			
			feed.gameObject.SetActive(true);
			Destroy(feed.gameObject, feedDespawnTime);
		}
		
		IEnumerator showUI(){
			yield return new WaitForSeconds(1f);
			
			if(!ui){
				ui = Instantiate(_ui);
				UIManager.Instance.Show(ui.gameObject);
			}
			
			var timer = new Vector2(0, 1.5f);
			
			while(timer.x < timer.y){
				ui.position = Vector3.Lerp(
					cam.WorldToScreenPoint(gpx.position),
					cam.WorldToScreenPoint(targetInteractable.position),
					timer.x / timer.y
				);
				
				timer.x += Time.deltaTime;
				yield return null;
			}
			
			if(ui.TryGetComponent<Animator>(out var anim))
				anim.SetTrigger("hide");
		}
	}
}