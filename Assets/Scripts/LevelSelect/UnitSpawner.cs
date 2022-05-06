using UnityEngine;
using System.Collections.Generic;

namespace LevelSelect
{
	public class UnitSpawner : MonoBehaviour
	{
		public List<Unit> units;
		
		[Space()]
		public float forwardOffset = 20f;
		public Vector2 sideValues = new Vector2(-24.5f, 24.5f);
		
			float GetSideValue(Side side){
				float output = 0f;
				
				switch(side){
					case Side.Left: output = sideValues.x; break;
					case Side.Right: output = sideValues.y; break;
				}
				
				return output;
			}
			
			Vector3 GetSideFacingDirection(Side side){
				var output = Vector3.zero;
				
				switch(side){
					case Side.Left: output = Vector3.right; break;
					case Side.Right: output = Vector3.left; break;
				}
				
				return output;
			}
			
		[Space()]
		public Unit newUnit;
		public Transform template;
		
		[ContextMenu("Spawn New Unit")]
		public void SpawnNewUnit(){
			var lastUnit = Tools.LerpOn(units, 1f);
				lastUnit = lastUnit != null? lastUnit: new Unit();
			
			// Overwrite the "side" of "newUnit"
				int side = (int) lastUnit.side;
					side ++;
					side = side % 2;
				
				var newSide = (Side) side;
			
			// Handle Orientations
				var targetPosition = new Vector3(GetSideValue(newSide), 0f, lastUnit.position.z + forwardOffset);
				var targetDirection = GetSideFacingDirection(newSide);
			
			// Instantiate Object
				var newObj = Instantiate(template, transform);
					newObj.name = newUnit.name;
					
				var building = Instantiate(newUnit.building, newObj);
					building.name = newUnit.name;
				
				newObj.position = targetPosition;
				newObj.forward = targetDirection;
			
				newObj.gameObject.SetActive(true);
			
			// Record
				var newUnitSpawned = new Unit();
					newUnitSpawned.name = newUnit.name;
					newUnitSpawned.building = building;
					newUnitSpawned.side = newSide;
					
				units.Add(newUnitSpawned);
		}
		
		public enum Side{ Left, Right }
		
		[System.Serializable]
		public class Unit{
			public string name;
			public Building building;
		
			[HideInInspector]
			public Side side;
			
			public Vector3 position{
				get{
					if(building) return building.transform.position;
					else return Vector3.zero;
				}
			}
		}
	}
}