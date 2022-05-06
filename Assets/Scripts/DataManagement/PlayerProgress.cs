using UnityEngine;

namespace DataManagement
{
	[CreateAssetMenu(menuName = "Player Progress")]
	public class PlayerProgress : ScriptableObject
	{
		public LevelData[] levels;
		public LevelDataSelector currentLevel;
	}
}