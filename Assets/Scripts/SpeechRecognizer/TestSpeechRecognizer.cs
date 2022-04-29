using UnityEngine;
using UnityEngine.UI;

namespace Proty
{
	public class TestSpeechRecognizer : MonoBehaviour
	{
		public string[] words;
		
		public Text txt;
	
		public SpeechRecognizer sr;
		
		void Start(){
			string word = Tools.Random(words);
			txt.text = word;
			
			sr.Listen(word, Start);
		}
	}
}