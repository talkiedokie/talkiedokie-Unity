using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public partial struct GeneralAudioGroupSelect
{	
	#if UNITY_EDITOR
	
		[CustomPropertyDrawer(typeof(GeneralAudioGroupSelect))]
		class GeneralAudioGroupSelectPropertyDrawer : PropertyDrawer
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
				var groups = GeneralAudio.Instance.Groups;
				int count = groups.Length;
				names = new string[count];
				
				for(int i = 0; i < count; i++)
					names[i] = groups[i].name;
			}
			
		}
		
	#endif

}
