using UnityEngine;
using UnityEngine.UI;

namespace Minigame.Zoo
{
	public class GameTimer : MonoBehaviour
	{
		public static bool TimeOver{ get; private set; }
		
		[SerializeField] float duration = 60f;
		float currentTime;
		
		[Foldout("Warning")]
		[SerializeField, Range(0,1)] float warningPercent = 0.167f;
		[SerializeField, LabelOverride("Animator")] Animator anim;
		[SerializeField] AudioSource audioPlayer;
		
		[Foldout("UI")]
		[SerializeField] GameObject ui;
		[SerializeField] Image fill;
		[SerializeField] Text text;
		
		[Space()]
		[SerializeField] GameObject timeOverUi;
		[SerializeField] GameObject timeOverUiButtons;
		
		[Space()]
		[SerializeField] GeneralAudioSelector uiShowSound;
		[SerializeField] GeneralAudioSelector buttonsShowSound;

#region Unity Methods
	
		void Start()
		{
			SetCurrentTime(duration);
			ui.SetActive(true);
			
			uiShowSound.Play();
		}
		
		void Update()
		{
			if(currentTime <= 0f && !TimeOver)
				OnTimeOver();
			
			else if(!TimeOver)
				currentTime -= Time.deltaTime;
		}
		
		void LateUpdate()
		{
			if(TimeOver)
				return;
			
			float percent = Mathf.Clamp01(currentTime / duration);
			
			if(percent < warningPercent)
				PlayWarning();
			
			UpdateUI(percent);
		}
		
		void OnDestroy() => TimeOver = false;

#endregion
	
		public void SetCurrentTime(float duration) =>
			currentTime = duration;
		
		void PlayWarning()
		{
			if(audioPlayer.isPlaying)
				return;
		
			anim.SetTrigger("pop");
			audioPlayer.Play();
		}
		
		void UpdateUI(float percent)
		{
			fill.fillAmount = percent;
			text.text = Mathf.Round(currentTime).ToString();
		}
		
		void OnTimeOver()
		{
			Debug.LogWarning("TIME'S UP!", this);
			
			TimeOver = true;
			
			timeOverUi.SetActive(true);
			
			var speech = Gameplay.Speech.Instance;
				speech.StopListening();
				speech.FinishUsing();
			
			Invoke(nameof(ShowTimeOverUIButtons), 2f);
		}
		
		void ShowTimeOverUIButtons()
		{
			timeOverUiButtons.SetActive(true);
			buttonsShowSound.Play();
		}
	}
}