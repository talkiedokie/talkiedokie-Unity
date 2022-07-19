using UnityEngine;

[CreateAssetMenu]
public class LevelScenesManager : Singleton<LevelScenesManager>
{
	public SceneLoader[]
		insideBuildingLevels,
		minigameLevels;
	
	[Space()]
	public SceneLoader allScenesInBuildSettings;
}