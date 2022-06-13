using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace AccountsManagement
{
	[CreateAssetMenu(menuName = "Managers/Accounts")]
	public class AccountManager : Singleton<AccountManager>
	{
		public List<User> users = new List<User>();
		public UserSelector currentUser;
		
		public int maxUserCount = 5;
		
		public User GetUser(int index){
			if(index < users.Count && index > -1)
				return users[index];
			
			else return null;
		}
		
		public User CurrentUser(){ return GetUser(currentUser); }
		
		public bool UserExists(string name){
			var user = users.Find(user => user.name == name);
			return user != null;
		}
		
		public void AddUser(
			User user,
			out int index,
			out bool success
		){
			index = users.Count;
			success = users.Count <= maxUserCount;
			
			if(success) users.Add(user);
		}
		
		public void RemoveUser(int index){
			users.RemoveAt(index);
		}
		
		#region Serialization
		
			string dataPath => Application.persistentDataPath + "/accMgr.json";
			const bool prettyPrint = true;
			
			[ContextMenu("Save")]
			public void Serialize(){
				var data = new DataWrapper(users, currentUser);
				string newJson = JsonUtility.ToJson(data, prettyPrint);
				
				string path = dataPath;
				File.WriteAllText(path, newJson);
				
				DataPath();
			}
			
			[ContextMenu("Load")]
			public void LoadData(){
				string path = dataPath;
				
				if(File.Exists(path)){
					string json = File.ReadAllText(path);
					var data = JsonUtility.FromJson<DataWrapper>(json);
					
					users = data.users;
					currentUser = data.currentUser;
				}
			}
			
			[ContextMenu("Data Path")]
			public void DataPath(){
				Debug.Log(dataPath);
			}
			
		#endregion
	}
	
	[System.Serializable]
	public class DataWrapper{
		public List<User> users;
		public int currentUser;
		
		public DataWrapper(List<User> users, int currentUser){
			this.users = users;
			this.currentUser = currentUser;
		}
	}
}