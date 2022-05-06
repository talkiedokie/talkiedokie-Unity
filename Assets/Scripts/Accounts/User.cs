using DataManagement;

namespace AccountsManagement
{
	[System.Serializable]
	public class User
	{
		public string name;
		public int coins;
		public Gender gender;
		public PlayerProgress progress;
	}
}