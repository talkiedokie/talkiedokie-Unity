namespace AccountsManagement
{
	[System.Serializable]
	public class User
	{
		public string name;
		public string password;
		
		public int coins;
		public Gender gender;
		
		public User(string name, string password, Gender gender){
			this.name = name;
			this.password = password;
			this.gender = gender;
		}
	}
}

public enum Gender{ Male, Female }