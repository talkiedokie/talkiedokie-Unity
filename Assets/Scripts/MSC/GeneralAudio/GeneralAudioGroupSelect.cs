using UnityEngine;

[System.Serializable]
public partial struct GeneralAudioGroupSelect
{
	public int value;
	
	public GeneralAudioGroupSelect(int v){ value = v; }
	
	public static implicit operator int(GeneralAudioGroupSelect gas){
		return gas.value;
	}
	
	public static implicit operator GeneralAudioGroupSelect(int value){
		return new GeneralAudioGroupSelect(value);
	}
}