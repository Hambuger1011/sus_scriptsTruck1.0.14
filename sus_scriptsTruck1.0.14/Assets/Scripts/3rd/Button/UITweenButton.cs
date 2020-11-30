using Object = UnityEngine.Object;
using UnityEngine;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using DG.Tweening;
using XLua;

namespace GameCore.UGUI
{
    // Button that's meant to work with mouse or touch-based devices.
    [AddComponentMenu("UI/UITweenButton", 20)]
    [LuaCallCSharp]
    public class UITweenButton : Selectable, IPointerClickHandler, ISubmitHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, ICancelHandler
    {
        public static Action<UITweenButton> defaultSounds = (_)=> { };

        public Transform tweenTransform;

        public Action<UITweenButton> buttonSounds;

        public Transform TweenTransform
        {
            get
            {
                if(tweenTransform != null)
                {
                    return tweenTransform;
                }
                return this.transform;
            }
        }

        string tweenId;

        [Serializable,LuaCallCSharp]
        public class ButtonClickedEvent : UnityEvent {}

        // Event delegates triggered on click.
        [FormerlySerializedAs("onClick")]
        [SerializeField]
        private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

        protected UITweenButton()
        {}

        protected override void Awake()
        {
            base.Awake();
            tweenId = "Tween_" + this.GetInstanceID();
            UpdateTweenScale();
        }

        public ButtonClickedEvent onClick
        {
            get { return m_OnClick; }
            set { m_OnClick = value; }
        }

        private void Press()
        {
            if (!IsActive() || !IsInteractable())
                return;

            if(buttonSounds != null)
            {
                buttonSounds(this);
            }else
            {
                defaultSounds(this);
            }

            UISystemProfilerApi.AddMarker("Button.onClick", this);
            m_OnClick.Invoke();
        }

        // Trigger all registered callbacks.
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            Press();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            Press();

            // if we get set disabled during the press
            // don't run the coroutine.
            if (!IsActive() || !IsInteractable())
                return;

            DoStateTransition(SelectionState.Pressed, false);
            StartCoroutine(OnFinishSubmit());
        }

        private IEnumerator OnFinishSubmit()
        {
            var fadeTime = colors.fadeDuration;
            var elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }

        #region 按钮事件
        private Vector2 originScale;
        public bool enableTween = true;
        public Vector2 tweenScale = new Vector2(0.9f, 0.9f);
        public float _tweenDuration = 0.25f;

        public void UpdateTweenScale()
        {
            originScale = this.transform.localScale;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            if(enableTween)
            {
                DOTween.Kill(tweenId);
                TweenTransform.DOScale(
                    new Vector3(originScale.x * tweenScale.x, originScale.y * tweenScale.y, 1),
                    _tweenDuration)
                    .SetId(tweenId).Play();
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            CancelTween();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            CancelTween();
        }

        public void OnCancel(BaseEventData eventData)
        {
            CancelTween();
        }

        void CancelTween()
        {
            if (!enableTween)
            {
                return;
            }
            DOTween.Kill(tweenId);
            TweenTransform.DOScale(originScale, _tweenDuration).SetId(tweenId).Play();
        }
        #endregion
    }
}
