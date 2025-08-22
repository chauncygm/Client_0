using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityGameFramework.Runtime;

namespace GameBase
{
	public class EventTriggerListener : EventTrigger
	{
		public delegate void VoidDelegate(GameObject go);
		public delegate void EventDelegate(GameObject go, PointerEventData ev);
		[FormerlySerializedAs("onClick")]
		public VoidDelegate OnClick;
		[FormerlySerializedAs("onDown")]
		public EventDelegate OnDown;
		[FormerlySerializedAs("v")]
		public EventDelegate OnExit;
		[FormerlySerializedAs("onUp")]
		public EventDelegate OnUp;
		[FormerlySerializedAs("onSelect")]
		public VoidDelegate OnSelectEvent;
		[FormerlySerializedAs("onUpdateSelect")]
		public VoidDelegate OnUpdateSelect;
		[FormerlySerializedAs("onDragBegin")]
		public EventDelegate OnDragBegin;
		[FormerlySerializedAs("onDrag")]
		public EventDelegate OnDragEvent;
		[FormerlySerializedAs("onDragEnd")]
		public EventDelegate OnDragEnd;
		[FormerlySerializedAs("onEnter")]
		public EventDelegate OnEnter;
		[FormerlySerializedAs("onDrop")]
		public EventDelegate OnDropEvent;

		public delegate void ClickEffectDelegate();
		public static EventTriggerListener Get(GameObject go, float time = -1, bool play_ani = true, float scale = 1)
		{
			if (!go)
			{
				Log.Warning("EventTriggerListener.Get, GameObject is null!!");
			}
			EventTriggerListener listener = go.GetComponent<EventTriggerListener>();

			if (listener == null)
				listener = go.AddComponent<EventTriggerListener>();
			return listener;
		}

		private bool _IsValidTrigger()
		{
			return true;
		}

		public override void OnPointerClick(PointerEventData eventData)
		{
			if (!_IsValidTrigger())
			{
				return;
			}

			if (Input.touchCount > 1)
			{
				return;
			}
			OnClick?.Invoke(gameObject);
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			if (!this._IsValidTrigger())
			{
				return;
			}
			OnDown?.Invoke(gameObject, eventData);
		}

		public override void OnPointerEnter(PointerEventData eventData)
		{
			if (!_IsValidTrigger())
			{
				return;
			}
			OnEnter?.Invoke(gameObject, eventData);
		}

		public override void OnPointerExit(PointerEventData eventData)
		{
			if (!_IsValidTrigger())
			{
				return;
			}

			OnExit?.Invoke(gameObject, eventData);
		}

		public override void OnPointerUp(PointerEventData eventData)
		{
			if (!_IsValidTrigger())
			{
				return;
			}

			OnUp?.Invoke(gameObject, eventData);
		}

		public override void OnSelect(BaseEventData eventData)
		{
			if (!_IsValidTrigger())
			{
				return;
			}

			OnSelectEvent?.Invoke(gameObject);
		}

		public override void OnUpdateSelected(BaseEventData eventData)
		{
			if (!_IsValidTrigger())
			{
				return;
			}

			OnUpdateSelect?.Invoke(gameObject);
		}

		public override void OnBeginDrag(PointerEventData eventData)
		{
			if (!_IsValidTrigger())
			{
				return;
			}

			OnDragBegin?.Invoke(gameObject, eventData);
		}

		public override void OnDrag(PointerEventData eventData)
		{
			if (!_IsValidTrigger())
			{
				return;
			}

			OnDragEvent?.Invoke(gameObject, eventData);
		}

		public override void OnEndDrag(PointerEventData eventData)
		{
			if (!_IsValidTrigger())
			{
				return;
			}

			OnDragEnd?.Invoke(gameObject, eventData);
		}

		public override void OnDrop(PointerEventData eventData)
		{
			if (!_IsValidTrigger())
			{
				return;
			}

			OnDropEvent?.Invoke(gameObject, eventData);
		}

		private void OnDestroy()
		{
			OnClick = null;
			OnDown = null;
			OnExit = null;
			OnUp = null;
			OnSelectEvent = null;
			OnDragBegin = null;
			OnDragEvent = null;
			OnDragEnd = null;
			OnEnter = null;
			OnDropEvent = null;
			OnDragEnd = null;
		}
	}
}
