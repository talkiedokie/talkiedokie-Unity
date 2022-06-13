using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static class Tools
{
	#region String
		
		public static string Clamp(string str, int length){
			string output = "";
			
			for(int i = 0; i < length; i++)
				if(i < str.Length)
					output += str[i];
			
			if(str.Length > length)
				output += "...";
			
			return output;
		}
		
		public static string AddStringSpaces(string str){
			string output = "";
			int length = str.Length;
			
			for(int i = 0; i < length; i++){
				output += str[i];
				
				int nextIndex = i + 1;
				
				if(nextIndex < length){
					if(System.Char.IsUpper(str, nextIndex))
						output += " ";
				}
			}
			
			return output;
		}
		
	#endregion
	
	#region Random
		
		#region Array/List
			
			public static T Random<T>(T[] array){
				return array[UnityEngine.Random.Range(0, array.Length)];
			}
			
			public static T Random<T>(List<T> list){
				return list[UnityEngine.Random.Range(0, list.Count)];
			}
			
			public static T Random<T>(T[] array, out int index){
				index = UnityEngine.Random.Range(0, array.Length);
				return array[index];
			}
			
			public static T Random<T>(List<T> list, out int index){
				index = UnityEngine.Random.Range(0, list.Count);
				return list[index];
			}
			
		#endregion
		
		#region Condition
			
			public static bool RandomCondition(){
				return UnityEngine.Random.value < 0.5f;
			}
			
			public static bool RandomCondition(float probability){
				return UnityEngine.Random.value < Mathf.Clamp01(probability);
			}
			
			public static bool RandomCondition(
				float probability,
				out float probableValue
			){
				probableValue = UnityEngine.Random.value;
				probability = Mathf.Clamp01(probability);
				
				return probableValue < probability;
			}
			
		#endregion
		
	#endregion
	
	public static T LerpOn<T>(T[] array, float t){
		int length = array.Length;
		
		if(length == 0) return default(T);
		if(length == 1) return array[0];
		
		t = Mathf.Clamp01(t);
		int index = Mathf.RoundToInt(Mathf.Lerp(0, length - 1, t));
		
		return array[index];
	}
	
	public static T LerpOn<T>(List<T> list, float t){
		int count = list.Count;
		
		if(count == 0) return default(T);
		if(count == 1) return list[0];
		
		t = Mathf.Clamp01(t);
		int index = Mathf.RoundToInt(Mathf.Lerp(0, count - 1, t));
		
		return list[index];
	}
	
	public static void AddToArray<T>(ref T[] current, T newEntry, out int newIndex){
		int count = current.Length;
		T[] newArray = new T[count + 1];
		
		for(int i = 0; i < count; i++)
			newArray[i] = current[i];
		
		newIndex = count;
		newArray[newIndex] = newEntry;
		
		current = newArray;
	}
	
	public static void StartCoroutine( // This will run a coroutine and making sure that the old routine is already stop (avoiding lags from duplicate routines)
		ref IEnumerator current,
		IEnumerator target,
		MonoBehaviour monoBehaviour
	){
		if(current != null)
			monoBehaviour.StopCoroutine(current);
		
		current = target;
		monoBehaviour.StartCoroutine(current);
	}
	
	public static bool CompareLayer(int layer, LayerMask mask){
		return ((1 << layer) & mask) != 0;
	}
}