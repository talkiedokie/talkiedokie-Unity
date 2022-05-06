using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class SetOrientationPlaymode : ScriptableObject
{
	#if UNITY_EDITOR
		
		public List<Transformm> saves = new List<Transformm>();
		const string childName = "CamRig";
		
		[ContextMenu("Save")]
		public void Save(){
			var selection = Selection.transforms[0];
			
			var newSave = new Transformm();
				newSave.name = selection.name;
				newSave.position = selection.position;
				newSave.rotation = selection.rotation;
				newSave.scale = selection.localScale;
			
			saves.Add(newSave);
		}
		
		[ContextMenu("Apply")]
		public void Apply(){
			foreach(var save in saves) save.Apply(childName);
		}
		
		[System.Serializable]
		public class Transformm{
			public string name;
			
			public Vector3 position;
			public Quaternion rotation;
			public Vector3 scale;
			
			public Transform target;
			
			public void Apply(string childName){
				var target = this.target.Find(childName);
					
					target.position = position;
					target.rotation = rotation;
					target.localScale = scale;
				
				var child = target.GetChild(0);
					child.localPosition = Vector3.zero;
					child.localRotation = Quaternion.identity;
					child.localScale = Vector3.one;
			}
		}
		
	#endif
}