
namespace BookReading
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System;

    public class UISceneInteraction :
#if !NOT_USE_LUA
        MonoBehaviour
#else
        UIBookReadingElement
#endif
    {
        public RectTransform ActionTarget;
#if !NOT_USE_LUA
        private Action onComplete;
#endif


        private void Awake()
        {
            UIEventListener.AddOnClickListener(ActionTarget.gameObject, ActionHandler);
        }
        public void OnDestroy()
        {
            UIEventListener.RemoveOnClickListener(ActionTarget.gameObject, ActionHandler);
        }
#if NOT_USE_LUA
        private BookReadingForm _form;
        public override void Bind(BookReadingForm form)
        {
            _form = form;
            form.sceneInteraction = this;

            UIEventListener.AddOnClickListener(ActionTarget.gameObject, ActionHandler);
        }


        public override void SetSkin() { }
        public override void ResetUI()
        {
            this.gameObject.SetActive(false);
        }

        public override void Dispose()
        {
            
        }
#endif
        private void ActionHandler(PointerEventData data)
        {
            AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
            this.gameObject.SetActive(false);
#if !NOT_USE_LUA
            onComplete();
#else
            _form.setBGOnClickListenerActive(true);
            EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, null);
#endif
        }

        public void SetPos(int posX, int posY)
        {
            ActionTarget.anchoredPosition = new Vector2(posX, posY);
        }

#if !NOT_USE_LUA
        public void Show(Action callback)
        {
            this.onComplete = callback;
            this.gameObject.SetActive(true);
        }
#else
        public void Show()
        {
            this.gameObject.SetActive(true);
        }
#endif

    }
}