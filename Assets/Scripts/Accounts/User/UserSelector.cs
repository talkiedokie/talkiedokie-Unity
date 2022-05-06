using UnityEngine;

namespace AccountsManagement
{
	[System.Serializable]
	public partial struct UserSelector
	{
		public int value;
		public UserSelector(int v){ value = v; }
		
		public static implicit operator int(UserSelector us){ return us.value; }
		public static implicit operator UserSelector(int value){ return new UserSelector(value); }
		
		public User Reference{
			get{
				return AccountManager.Instance.GetUser(value);
			}
		}
	}
}