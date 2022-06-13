using UnityEngine;

namespace InsideBuilding.Gameplay
{
	public class BookShelfLayer : MonoBehaviour
	{
		public Transform[] slots;
		int currentSlot;
		
		public void AddObject(Transform obj, out bool successful){
			var slot = slots[currentSlot];
			bool isSlotEmpty = slot.childCount == 0;
			
			if(isSlotEmpty){
				obj.parent = slot;
				obj.localPosition = Vector3.zero;
				obj.localRotation = Quaternion.identity;
				
				currentSlot ++;
				currentSlot = currentSlot % slots.Length;
			}
			
			successful = isSlotEmpty;
		}
		
		public void EmptySlot(int index){
			if(slots[index].childCount > 0){
				var child = slots[index].GetChild(0);
				Destroy(child.gameObject);
			}
		}
		
		public void EmptyAllSlots(){
			for(int i = 0; i < slots.Length; i++)
				EmptySlot(i);
			
			currentSlot = 0;
		}
	}
}