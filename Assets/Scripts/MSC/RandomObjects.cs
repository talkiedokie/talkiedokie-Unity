using UnityEngine;

public class RandomObjects : MonoBehaviour
{
	[SerializeField, Range(0,1)] float probability = 0.25f;
	[SerializeField] GameObject[] objects;
	
	void Awake(){
		foreach(var obj in objects){
			bool isActive = Tools.RandomCondition(probability, out float probableValue);
			
			obj.SetActive(isActive);
			
			if(isActive)
				Debug.Log(obj.name + " is active at the probability of " + probableValue, obj);
		}
	}
}