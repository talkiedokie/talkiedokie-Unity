using UnityEngine;

namespace AccountsManagement
{
	[CreateAssetMenu(menuName = "Managers/Accounts")]
	public class AccountManager : Singleton<AccountManager>
	{
		public User[] users;
		public UserSelector currentUser;
		
		public User GetUser(int index){ return users[index]; }
		public User CurrentUser(){ return GetUser(currentUser); }
	}
}