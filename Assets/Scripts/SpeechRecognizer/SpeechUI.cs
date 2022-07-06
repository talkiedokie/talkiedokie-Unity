using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
	public class SpeechUI : MonoBehaviour
	{
		[SerializeField] GameObject hypothesisObj;
		[SerializeField] Text resultTxt;
		[SerializeField] Animator popAnim;
		
		const FontStyle
			ITAL = FontStyle.Italic,
			BOLD = FontStyle.Bold;
		
		public void SetActive(bool b) => gameObject.SetActive(b);

		public void OnListen(){
			SetActive(true);
			resultTxt.text = "";
			
			popAnim.SetTrigger("pop");
		}
		
		public void OnHypothesis(string hypothesis){
			hypothesisObj.SetActive(true);
			resultTxt.fontStyle = ITAL;
			resultTxt.text = hypothesis;
		}
		
		public void OnResult(string result){
			resultTxt.fontStyle = BOLD;
			resultTxt.text = result;
			
			hypothesisObj.SetActive(false);
		}
	}
}