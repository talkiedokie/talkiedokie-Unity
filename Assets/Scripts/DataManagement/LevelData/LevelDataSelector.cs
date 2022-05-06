using UnityEngine;

namespace DataManagement
{
	[System.Serializable]
	public partial struct LevelDataSelector
	{
		public int value;
		
		public LevelDataSelector(int v){ value = v; }
		
		#region MSC
			
			// Integer
				public static implicit operator int(LevelDataSelector lds){ return lds.value; }
				public static implicit operator LevelDataSelector(int value){ return new LevelDataSelector(value); }
			
			// Arithmetic
				public static LevelDataSelector operator +(LevelDataSelector a, LevelDataSelector b){ return new LevelDataSelector(a.value + b.value); }
				public static LevelDataSelector operator -(LevelDataSelector a, LevelDataSelector b){ return new LevelDataSelector(a.value - b.value); }
				public static LevelDataSelector operator *(LevelDataSelector a, LevelDataSelector b){ return new LevelDataSelector(a.value * b.value); }
				public static LevelDataSelector operator /(LevelDataSelector a, LevelDataSelector b){ return new LevelDataSelector(a.value / b.value); }
			
			// Conditions
				public static bool operator ==(LevelDataSelector a, LevelDataSelector b){ return a.value == b.value; }
				public static bool operator !=(LevelDataSelector a, LevelDataSelector b){ return a.value != b.value; }
				
				public static bool operator ==(int a, LevelDataSelector b){ return a == b.value; }
				public static bool operator !=(int a, LevelDataSelector b){ return a != b.value; }
			
			// Conditions Overrides
				public override bool Equals(object obj){
					bool output = false;
					
					if(obj is LevelDataSelector)
						output = value == ((LevelDataSelector) obj).value;
					
					else if(obj is int) output = value == ((int) obj);
					
					return output;
				}
				
				public bool Equals(LevelDataSelector lds) => value == lds.value;
				public override int GetHashCode() => value.GetHashCode();
			
		#endregion
	}
}