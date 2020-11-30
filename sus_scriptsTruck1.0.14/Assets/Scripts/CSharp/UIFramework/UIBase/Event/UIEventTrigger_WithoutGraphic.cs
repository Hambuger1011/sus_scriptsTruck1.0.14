using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UIEventTriggerBase;

namespace UIEventTriggerBase
{
    [RequireComponent(typeof(RectTransform))]
    public class UIEventTrigger_WithoutGraphic :
            Graphic,
            IPointerEnterHandler,
            IPointerExitHandler,
            IPointerDownHandler,
            IPointerUpHandler,
            IPointerClickHandler
    {
        public UIVoidPointerEvent OnClick;
        public UIVoidPointerEvent OnEnter;
        public UIVoidPointerEvent OnExit;
        public UIVoidPointerEvent OnDown;
        public UIVoidPointerEvent OnUp;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (OnClick != null) OnClick(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (OnEnter != null) OnEnter(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (OnExit != null) OnExit(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (OnDown != null) OnDown(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (OnUp != null) OnUp(eventData);
        }

        protected override void Awake()
        {
            base.Awake();
            color = new Color(1, 1, 1, 0);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnClick = null;
            OnEnter = null;
            OnExit = null;
            OnDown = null;
            OnUp = null;
            OnClick = null;
        }
    }
}
