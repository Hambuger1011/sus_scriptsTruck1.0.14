using Framework;

using System;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class UGUIEvent : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IEventSystemHandler
{
    public event Action<PointerEventData, UGUIEvent> onBeginDrag;

    public event Action<PointerEventData, UGUIEvent> onDrag;

    public event Action<PointerEventData, UGUIEvent> onEndDrag;

    public event Action<PointerEventData, UGUIEvent> onPointerClick;

    public event Action<PointerEventData, UGUIEvent> onPointerDown;

    public event Action<PointerEventData, UGUIEvent> onPointerEnter;

    public event Action<PointerEventData, UGUIEvent> onPointerUp;

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (this.onBeginDrag != null)
		{
			this.onBeginDrag(eventData, this);
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (this.onDrag != null)
		{
			this.onDrag(eventData, this);
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (this.onEndDrag != null)
		{
			this.onEndDrag(eventData, this);
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (this.onPointerClick != null)
		{
			this.onPointerClick(eventData, this);
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (this.onPointerDown != null)
		{
			this.onPointerDown(eventData, this);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (this.onPointerEnter != null)
		{
			this.onPointerEnter(eventData, this);
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (this.onPointerUp != null)
		{
			this.onPointerUp(eventData, this);
		}
	}
}
