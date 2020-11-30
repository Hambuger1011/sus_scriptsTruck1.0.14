/*
 * UGUI事件脚本
 */

using System;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework
{
    public enum enUIEventType
    {
        Down,
        Click,
        HoldStart,
        Hold,
        HoldEnd,
        DragStart,
        Drag,
        DragEnd,
        Drop,
        Up,
        PointerEnter,
        PointerExit,
        DragDropObjStart
    }

    public class CUIEventScript : 
        CUIInputHandler, 
        IBeginDragHandler, 
        IEventSystemHandler, 
        IEndDragHandler, 
        IDragHandler, 
        IPointerDownHandler, 
        IPointerClickHandler, 
        IPointerUpHandler, 
        IDropHandler
    {

        private const float c_clickAreaValue = 40f;
        

        [HideInInspector]
        public bool m_closeFormWhenClicked;

        [HideInInspector]
        public bool m_isDispatchDragEventForBelongList = true;

        protected bool m_isDown;

        protected bool m_isHold;

        protected bool m_canClickOrHold;

        public float c_holdTimeValue = 1f;

        private float m_downTimestamp;

        private Vector2 m_downPosition;

        protected PointerEventData m_holdPointerEventData;

        private bool m_needClearInputStatus;

        private bool m_isApplicationPaused;

        private float m_applicationPausedTimestamp;

        public override void Initialize(CUIForm formScript)
        {
            if (!base.m_isInitialized)
            {
                this.m_customUpdateFlags |= enCustomUpdateFlag.eAll;
                base.Initialize(formScript);
                base.m_isInitialized = true;
            }
        }

        public override void UnInitialize()
        {
            if (base.m_isInitialized)
            {
                m_holdPointerEventData = null;
                base.UnInitialize();
                base.m_isInitialized = false;
            }
        }

        protected override void OnDestroy()
        {
            UnInitialize();
            base.OnDestroy();
        }

        public override void OnClose()
        {
            ExecuteClearInputStatus();
        }

      
        
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            m_isDown = true;
            m_isHold = false;
            m_canClickOrHold = true;
            m_downTimestamp = Time.realtimeSinceStartup;
            m_downPosition = eventData.position;
            m_holdPointerEventData = eventData;
            m_needClearInputStatus = false;
            //DispatchUIEvent(enUIEventType.Down, eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (m_isHold && m_holdPointerEventData != null)
            {
                //DispatchUIEvent(enUIEventType.HoldEnd, m_holdPointerEventData);
            }
            //DispatchUIEvent(enUIEventType.Up, eventData);
            ClearInputStatus();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            bool flag = true;
            if (m_canClickOrHold && flag)
            {
                //DispatchUIEvent(enUIEventType.Click, eventData);
                if (m_closeFormWhenClicked && (UnityEngine.Object)base.myForm != (UnityEngine.Object)null)
                {
                    base.myForm.Close();
                }
            }
            ClearInputStatus();
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            CUIInputHandler.RecordTouchHandlerID(eventData, base.m_instanceID);
            if (m_canClickOrHold && (UnityEngine.Object)base.myForm != (UnityEngine.Object)null /*&& base.myForm.ChangeScreenValueToForm(Vector2.Distance(eventData.position, m_downPosition)) > 40f*/)
            {
                m_canClickOrHold = false;
            }
            //DispatchUIEvent(enUIEventType.DragStart, eventData);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            CUIInputHandler.RecordTouchHandlerID(eventData, base.m_instanceID);
            if (m_canClickOrHold && (UnityEngine.Object)base.myForm != (UnityEngine.Object)null /*&& base.myForm.ChangeScreenValueToForm(Vector2.Distance(eventData.position, m_downPosition)) > 40f*/)
            {
                m_canClickOrHold = false;
            }
            //DispatchUIEvent(enUIEventType.Drag, eventData);
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (m_canClickOrHold && (UnityEngine.Object)base.myForm != (UnityEngine.Object)null /*&& base.myForm.ChangeScreenValueToForm(Vector2.Distance(eventData.position, m_downPosition)) > 40f*/)
            {
                m_canClickOrHold = false;
            }
            //DispatchUIEvent(enUIEventType.DragEnd, eventData);
            ClearInputStatus();
        }

        public void OnDrop(PointerEventData eventData)
        {
            //DispatchUIEvent(enUIEventType.Drop, eventData);
        }

        public void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                m_isApplicationPaused = true;
                m_applicationPausedTimestamp = Time.realtimeSinceStartup;
            }
            else
            {
                if (m_isApplicationPaused && Time.realtimeSinceStartup - m_applicationPausedTimestamp >= 2f && m_isDown && m_holdPointerEventData != null)
                {
                    OnPointerUp(m_holdPointerEventData);
                    OnPointerClick(m_holdPointerEventData);
                    OnEndDrag(m_holdPointerEventData);
                }
                m_isApplicationPaused = false;
                m_applicationPausedTimestamp = 0f;
            }
        }

        public bool ClearInputStatus()
        {
            m_needClearInputStatus = true;
            return m_isDown;
        }

        public void ExecuteClearInputStatus()
        {
            m_isDown = false;
            m_isHold = false;
            m_canClickOrHold = false;
            m_downTimestamp = 0f;
            m_downPosition = Vector2.zero;
            m_holdPointerEventData = null;
        }

        public override void CustomUpdate()
        {
            if (!m_needClearInputStatus && m_isDown)
            {
                if (!m_isHold)
                {
                    if (m_canClickOrHold && Time.realtimeSinceStartup - m_downTimestamp >= c_holdTimeValue)
                    {
                        m_isHold = true;
                        m_canClickOrHold = false;
                        //DispatchUIEvent(enUIEventType.HoldStart, m_holdPointerEventData);
                    }
                }
                else
                {
                    //DispatchUIEvent(enUIEventType.Hold, m_holdPointerEventData);
                }
            }
        }

        public override void CustomLateUpdate()
        {
            if (m_needClearInputStatus)
            {
                ExecuteClearInputStatus();
                m_needClearInputStatus = false;
            }
        }

        private void OnDisable()
        {
            if (m_needClearInputStatus)
            {
                ExecuteClearInputStatus();
                m_needClearInputStatus = false;
            }
        }
        
        private void PostWwiseEvent(string[] wwiseEvents)
        {
            for (int i = 0; i < wwiseEvents.Length; i++)
            {
                if (!string.IsNullOrEmpty(wwiseEvents[i]))
                {
                    //Singleton<CSoundManager>.GetInstance().PostEvent(wwiseEvents[i], null);
                }
            }
        }
    }
}
