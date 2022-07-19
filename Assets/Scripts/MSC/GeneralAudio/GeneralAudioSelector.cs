using UnityEngine;

[System.Serializable]
public partial struct GeneralAudioSelector
{
	public int value;
	
	public GeneralAudioSelector(int v){ value = v; }
	
	public static implicit operator int(GeneralAudioSelector gas) =>
		gas.value;
	
	public static implicit operator GeneralAudioSelector(int value) =>
		new GeneralAudioSelector(value);
	
	static GeneralAudio mgr;
	static GeneralAudio Mgr{
		get{
			if(!mgr) mgr = GeneralAudio.Instance;
			return mgr;
		}
	}
	
	public void Play() => Mgr.Play(value);
	public void PlayAdditive() => Mgr.PlayAdditive(value);
}