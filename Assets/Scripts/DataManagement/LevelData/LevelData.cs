namespace DataManagement
{
	[System.Serializable]
	public class LevelData
	{
		public string name;
		public Level levelPrefab;
		
		// [Space(), Range(0,1)]
		public float progress;
	}
}