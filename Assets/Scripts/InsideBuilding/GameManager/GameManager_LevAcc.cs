using UnityEngine;
using UnityEngine.UI;
using AccountsManagement;

namespace InsideBuilding
{
	public partial class GameManager
	{
		[Foldout("Level and Account")]
		[SerializeField] SceneLoader loginScene;
		
		[SerializeField] SceneLoader levelScene;
		public static SceneLoader LevelScene;
		
		[SerializeField] UserSelector user;
		[SerializeField] Text usernameTxt;
		
		[Space()]
		[SerializeField] GameObject[] characterPrefabs;
		
		Level level;
		AccountManager accMgr;
		
		public User UserData{ get; private set; }
		public Gender PlayerGender => UserData.gender;
		
		public string confirmPassword{ get; set; }
		
		public void SwitchAccount(){
			accMgr.Serialize();
			loginScene.Load();
		}
		
		public void DeleteAccount(){
			if(confirmPassword != UserData.password) return;
			
			accMgr.RemoveUser(user);
			accMgr.Serialize();
			
			loginScene.Load();
		}
	}
}