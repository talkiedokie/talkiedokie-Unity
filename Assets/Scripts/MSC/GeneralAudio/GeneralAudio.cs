using UnityEngine;
using UnityEngine.Audio;
using System;

[CreateAssetMenu(menuName = "Managers/General Audio")]
public class GeneralAudio : Singleton<GeneralAudio>
{
	[SerializeField, LabelOverride("Background Music")] AudioClip bgm;
	[SerializeField, Range(0,1)] float bgmVolume = 0.1f;
	
	[Space()]
	[SerializeField] AudioClip[] clips;
	[SerializeField] Group[] groups;
	
	#region Properties
		
		AudioSource bgmPlayer;
		AudioSource[] sources;
		
		GameObject _gameObject;
		GameObject gameObject{
			get{
				if(!_gameObject){
					_gameObject = new GameObject("GeneralAudioPlayer");
					
					int count = clips.Length;
					sources = new AudioSource[count];
					{
						for(int i = 0; i < count; i++){
							sources[i] = _gameObject.AddComponent<AudioSource>();
							sources[i].clip = clips[i];
							sources[i].playOnAwake = false;
						}
					}
					
					DontDestroyOnLoad(_gameObject);
				}
				
				return _gameObject;
			}
		}
		
		AudioSource source;
		AudioSource Source{
			get{
				if(!source)
					source = gameObject.AddComponent<AudioSource>();
				
				return source;
			}
		}
		
		public AudioClip[] Clips => clips;
		public Group[] Groups => groups;
		public bool isPlaying => source.isPlaying;
		
	#endregion
	
	#region Play
		
		public void PlayMusic(){
			if(bgmPlayer) return;
			
			bgmPlayer = gameObject.AddComponent<AudioSource>();
				bgmPlayer.clip = bgm;
				bgmPlayer.volume = bgmVolume;
				bgmPlayer.loop = true;
			
			bgmPlayer.Play();
		}
		
		public void Play(int clipIndex){
			var clip = clips[clipIndex];
			
			Play(clip);
		}
		
		public void Play(string clipName){
			var clip = Array.Find(clips, clip => clip.name == clipName);
			Play(clip);
		}
		
		public void Play(AudioClip clip){
			Source.Stop();
			Source.clip = clip;
			Source.Play();
		}
		
		public void PlayAdditive(GeneralAudioSelector selector){
			if(!_gameObject){
				var go = gameObject;
				Debug.Log(go.name, go);
			}
			
			var source = sources[selector];
			
			if(source.isPlaying)
				source.Stop();
		
			source.Play();
		}
		
	#endregion
	
	#region Play Random
		
		public void PlayRandom(){ Play(Tools.Random(clips)); }
		
		#region in Group
			
			public void PlayRandom(Group grp){
				if(grp == null) Play(Tools.Random(clips));
				else Play(Tools.Random(grp.clips));
			}
			
			public void PlayRandom(int groupIndex){
				var grp = groups[groupIndex];
					PlayRandom(grp);
			}
			
			public void PlayRandom(string groupName){
				var grp = Array.Find(groups, grp => grp.name == groupName);
					PlayRandom(grp);
			}
			
		#endregion
		
	#endregion
	
	public void Stop(){ source.Stop(); }
	
	public void SetBGMVolume(float percent){
		if(!bgmPlayer) return;
		bgmPlayer.volume = bgmVolume * percent;
	}
	
	#if UNITY_EDITOR
	
		public void SetBGMVolumeOnBuild(float amount) =>
			bgmVolume = amount;
		
	#endif
	
	[System.Serializable]
	public class Group{
		public string name;
		public AudioClip[] clips;
	}
}