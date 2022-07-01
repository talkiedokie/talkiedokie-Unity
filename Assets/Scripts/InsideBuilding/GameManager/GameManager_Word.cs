using UnityEngine;
using System.Collections;

namespace InsideBuilding
{
	public partial class GameManager
	{
		[Foldout("Word of the Day")]
		[SerializeField] WordOfTheDay[] wordOfTheDays;
		[SerializeField, LabelOverride("Delay")] float wotd_ClipDelay = 0.45f;
		
		[Space()]
		[SerializeField, LabelOverride("The word of the day is...")] AudioClip wotd_Clip1;
		[SerializeField, LabelOverride("I want you to say the word...")] AudioClip wotd_Clip2;
		
		WordOfTheDay wordOfTheDay;
		
		[ContextMenu("Change the Word of the Day")]
		public void ChangeTheWordOfTheDay(){
			wordOfTheDay = Tools.Random(wordOfTheDays);
			Debug.Log(wordOfTheDay.name);
		}
		
		[ContextMenu("Word of the Day")]
		public void WordOfTheDay(){
			var fairyVoiceClip = wordOfTheDay.fairyVoiceClip;
			var voiceClips = new AudioClip[]{ wotd_Clip1, fairyVoiceClip, wotd_Clip2, fairyVoiceClip };
				
				fairy.Speak(voiceClips, wotd_ClipDelay, ListenFairy_WordOfTheDay);
			
			Invoke(nameof(ShowWordOfTheDayUI), 1f);
		}
		
		void ListenFairy_WordOfTheDay(){
			StartCoroutine(delay());
			
			IEnumerator delay(){
				yield return new WaitForSeconds(0.5f);
				
				fairy.ListenToSpeech(wordOfTheDay.name, WordPopup_Correct, OnListenFinish);
				void OnListenFinish() => fairy.Speak(ycneyft_Clip, 1f, OnLevelFinished);
			}
		}
		
		void ShowWordOfTheDayUI(){
			// Extract String Value
				string word = wordOfTheDay.name;
			
			// Get Icon
				Sprite sprite = null;
				
				if(wordOfTheDay.sprites.Length > 0)
					sprite = Tools.Random(wordOfTheDay.sprites);
			
			// Setup UI
				string wordPopupText = sprite? word: "Word of the Day \n" + "'" + word + "'";
				WordPopup_Show(wordPopupText, sprite);
		}
	}
}