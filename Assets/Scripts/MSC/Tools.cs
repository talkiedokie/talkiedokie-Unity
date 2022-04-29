using UnityEngine;
using System;
using System.Collections.Generic;

public static class Tools
{
	public static string Clamp(string str, int length){
		string output = "";
		char[] chars = str.ToCharArray();
		
		for(int i = 0; i < length; i++)
			if(i < chars.Length)
				output += chars[i];
		
		if(chars.Length > length)
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
	
	#region Random
		
		public static T Random<T>(T[] array){
			return array[UnityEngine.Random.Range(0, array.Length)];
		}
		
		public static T Random<T>(List<T> list){
			return list[UnityEngine.Random.Range(0, list.Count)];
		}
		
		public static bool RandomCondition(){
			return UnityEngine.Random.value < 0.5f;
		}
		
		public static bool RandomCondition(float probability){
			return UnityEngine.Random.value < Mathf.Clamp01(probability);
		}
		
		public static bool RandomCondition(float probability, out float probableValue){
			probableValue = UnityEngine.Random.value;
			probability = Mathf.Clamp01(probability);
			
			return probableValue < probability;
		}
		
	#endregion
}