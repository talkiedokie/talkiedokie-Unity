using UnityEngine;
using UnityEngine.UI;
using AccountsManagement;
using System;
using System.Collections.Generic;

public class Login : MonoBehaviour
{
	#region Inspector
		
		public int maxPassword = 8;
		
		public SceneLoader levelScene;
		public Text loginMessageTxt;
		
		public GameObject
			buttons,
			confirmCreateObj;
		
		[Space()]
		public Dropdown rememberUserDropdown;
		public InputField usernameField;
		
	#endregion
	
	#region Properties
		
		public static bool hasLoggedIn{ get; private set; }
		public static UserSelector user{ get; private set; }
		
		public string username{ get; set; } = "";
		public string password{ get; set; }  = "";
		
		public string confirmPassword{ get; set; }
		public int gender{ get; set; }
		
		public bool remember{ get; set; }
		
		static List<User> rememberedUsers = new List<User>();
		
	#endregion
	
	#region Init
	
		AccountManager accMgr;
		
		void Awake(){
			accMgr = AccountManager.Instance;
			accMgr.LoadData();
		}
		
		void Start(){
			var options = new List<string>();
			
			foreach(var user in rememberedUsers)
				options.Add(user.name);
			
			rememberUserDropdown.AddOptions(options);
		}
		
	#endregion
	
	#region UI Input Calls
		
		public void OnCreate(){
			if(username == "" || password == ""){
				ShowWarning("Please fill the empty fields");
				return;
			}
			
			if(accMgr.UserExists(username)){
				ShowWarning("Username already exists");
				return;
			}
			
			if(password.Length < maxPassword){
				ShowWarning("Max password requires " + maxPassword + " characters");
				return;
			}
			
			buttons.SetActive(false);
			confirmCreateObj.SetActive(true);
		}
		
		public void OnCreateSubmit(){
			if(password != confirmPassword){
				ShowWarning("Password does not match"); return;
			}
			
			var newUser = new User(username, password, (Gender) gender);
				accMgr.AddUser(newUser, out int index, out bool success);
			
			if(success){
				accMgr.Serialize();
				LoginUser(index);
			}
			
			else ShowWarning("Max number of Users has been reached");
		}
		
		public void OnLogin(){
			if(username == "" || password == ""){
				ShowWarning("Please fill the empty fields"); return;
			}
			
			var users = accMgr.users;
			int index = users.FindIndex(user => user.name == username);
			var target = index >= 0? users[index]: null;
			
			if(target == null) ShowWarning("User not found");
			else if(target.password != password) ShowWarning("Incorrect password");
			else LoginUser(index);
		}
		
		public void OnRemeberedUserSelect(){
			var user = rememberedUsers[rememberUserDropdown.value];
			string name = user.name;
			usernameField.text = name;
		}
		
	#endregion
	
	#region MSC
		
		void ShowWarning(string message){
			loginMessageTxt.gameObject.SetActive(true);
			loginMessageTxt.text = message;
		}
		
		void LoginUser(int index){
			user = index;
			hasLoggedIn = true;
			
			if(remember){
				var user = ((UserSelector) index).Reference;
				
				if(!rememberedUsers.Contains(user))
					rememberedUsers.Add(user);
			}
			
			levelScene.LoadAsync();
		}
		
	#endregion
}