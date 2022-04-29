using UnityEngine;

namespace Gameplay
{
	public class CleanFridge : Swiping
	{
		// [SerializeField] Animator anim;
		[SerializeField] float animDelay = 1.5f;
		bool play;
		
		public override void Play(bool b){
			base.Play(b);
			play = b;
			
			if(play) Invoke(nameof(playAnim), animDelay);
			else playAnim();
		}
		
		void playAnim(){
			anim.SetBool(param, play);
		}
	}
}