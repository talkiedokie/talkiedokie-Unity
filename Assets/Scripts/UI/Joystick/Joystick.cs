using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour,
	IPointerDownHandler,
	IPointerUpHandler,
	IDragHandler
{
	public Vector2 axis{ get; private set; }
	
	#region Inspector
		
		[Foldout("Settings")]
		[SerializeField, Range(0,1)] float handleMinPosition = 0.8f;
		
		[SerializeField] bool
			x = true,
			y = true;
		
		[Space()]
		[SerializeField] bool showHideHandle;
		
		[Foldout("References")]
		[SerializeField] RectTransform rectTransform;
		[SerializeField] Transform handle, direction;
		
		[Foldout("Events")]
		public UnityEvent<Vector2> onAxisUpdate;
		public UnityEvent<float> onXUpdate, onYUpdate;
		
	#endregion
	
	#region Unity Ticks
		
		void OnValidate(){
			if(!rectTransform)
				rectTransform = transform as RectTransform;
		}
		
		void OnEnable() => ResetPosition();
		
	#endregion
	
	#region Event Systems
		
		public void OnPointerDown(PointerEventData data){
			if(showHideHandle){
				handle.gameObject.SetActive(true);
				
				UpdatePosition(data.position);
				EventsCallback();
			}
			
			direction.gameObject.SetActive(true);
		}
		
		public void OnDrag(PointerEventData data){
			UpdatePosition(data.position);
			EventsCallback();
		}
		
		public void OnPointerUp(PointerEventData data){
			ResetPosition();
			
			if(showHideHandle) handle.gameObject.SetActive(false);
			direction.gameObject.SetActive(false);
		
			EventsCallback();
		}
		
		void EventsCallback(){
			onAxisUpdate?.Invoke(axis);
			onXUpdate?.Invoke(axis.x);
			onYUpdate?.Invoke(axis.y);
		}
		
	#endregion
	
	#region Logic
		
		void UpdatePosition(Vector2 dataPosition){
			var position = (Vector2) transform.position;
				var difference = dataPosition - position;
				float size = rectTransform.sizeDelta.x / 2f * handleMinPosition;
				
					difference = Vector2.ClampMagnitude(difference, size);
					axis = difference / size;
				
				var handlePosition = new Vector2(
					x? position.x + difference.x: position.x,
					y? position.y + difference.y: position.y
				);
				
				handle.position = handlePosition;
				direction.up = difference;
		}
		
		void ResetPosition(){
			axis = Vector2.zero;
			handle.localPosition = Vector3.zero;
			direction.rotation = Quaternion.identity;
		}
		
	#endregion
}