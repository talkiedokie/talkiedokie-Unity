using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AccountsManagement
{
	public partial struct UserSelector
	{	
		#if UNITY_EDITOR
		
			[CustomPropertyDrawer(typeof(UserSelector))]
			class UserSelector_PropertyDrawer : PropertyDrawer
			{
				string[] names;
				
				public override void OnGUI(
					Rect position,
					SerializedProperty property,
					GUIContent label
				){
					EditorGUI.BeginProperty(position, label, property);
					{
						var serializedValue = property.FindPropertyRelative(nameof(value));
						
						var indexRect = new Rect(
							position.x,
							position.y,
							position.width,
							EditorGUIUtility.singleLineHeight
						);
						
						GetNames();
						
						serializedValue.intValue = EditorGUI.Popup(
							indexRect,
							property.displayName,
							serializedValue.intValue,
							names
						);
					}
					EditorGUI.EndProperty();
				}
				
				void GetNames(){
					var users = AccountManager.Instance.users;
					int count = users.Count;
					names = new string[count];
					
					for(int i = 0; i < count; i++)
						names[i] = users[i].name;
				}
				
			}
			
		#endif
	}
}