namespace UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using UnityEngine.Events;
    using System;


    [XLua.Hotfix, XLua.CSharpCallLua, XLua.LuaCallCSharp]
    public class UIBubbleEvent : Graphic, IPointerDownHandler, IPointerClickHandler, IBeginDragHandler
    {
        [Serializable]
        public class ButtonClickedEvent : UnityEvent { }
        
        [SerializeField]
        private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();
        public ButtonClickedEvent onClick
        {
            get { return m_OnClick; }
            set { m_OnClick = value; }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }

        #region 事件
        bool m_hasDrag = false;
        public void OnPointerDown(PointerEventData eventData)
        {
            m_hasDrag = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (m_hasDrag)
            {
                return;
            }

            m_OnClick.Invoke();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            m_hasDrag = true;
        }
        #endregion
    }
}