using UnityEngine;

namespace DataManagement
{
	[CreateAssetMenu(menuName = "Managers/Player Progress")]
	public class PlayerProgress : Singleton<PlayerProgress>
	{
		public LevelData[] levels;
	}
}