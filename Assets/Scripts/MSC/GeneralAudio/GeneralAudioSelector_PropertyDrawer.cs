using UnityEngine;

#if UNITY_EDITOR

	using UnityEditor;

#endif

public partial struct GeneralAudioSelector
{	
	#if UNITY_EDITOR
	
		[CustomPropertyDrawer(typeof(GeneralAudioSelector))]
		class GeneralAudioSelectorPropertyDrawer : PropertyDrawer
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
				var clips = GeneralAudio.Instance.Clips;
				int count = clips.Length;
				names = new string[count];
				
				for(int i = 0; i < count; i++)
					names[i] = clips[i].name;
			}
			
		}
		
	#endif

}
