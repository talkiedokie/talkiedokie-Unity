using UnityEngine;
using UnityEngine.UI;
using System;

namespace InsideBuilding
{
	public class EntryPointUI : MonoBehaviour
	{
		[SerializeField] Text descriptionTxt;
		
		public Action onAccept, onReject;
		
		public void OnAccept() => onAccept?.Invoke();
		public void OnReject() => onReject?.Invoke();
		
		public void SetDescription(string description) =>
			descriptionTxt.text = description;
		
		public void SetActive(bool b) => gameObject.SetActive(b);
	}
}