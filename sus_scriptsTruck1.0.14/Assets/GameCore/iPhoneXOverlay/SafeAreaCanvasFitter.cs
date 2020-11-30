/* SafeAreaCanvasFitter.cs
 * © Eddie Cameron 2019
 * ----------------------------
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace SafeAreaHelper
{
    /// <summary>
    /// Resizes the attached RectTransform to fill it's parent but adjusted to avoid safe areas
    /// E.G: Put on a child of a screen/overlay canvas to have it fill the safe area of the screen
    /// </summary>
    [RequireComponent( typeof( RectTransform ) )]
    [ExecuteInEditMode]
    public class SafeAreaCanvasFitter : MonoBehaviour
    {
        //刘海自适应类型
        public EnumAdaptation State = EnumAdaptation.TopAndBottom;

        private Rect _lastAdjustedSafeArea;

        private RectTransform _rectTransform;
        public RectTransform RectT {
            get {
                if ( _rectTransform == null )
                    _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        void Awake() {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            _lastAdjustedSafeArea = ResolutionAdapter.GetSafeArea();
            AdjustCanvasToArea(_lastAdjustedSafeArea);
        }


        private void AdjustCanvasToArea(Rect safeArea)
        {
            var uiform = this.GetComponentInParent<CUIForm>();
            var offset = uiform.Pixel2View(safeArea.position);

            if (ResolutionAdapter.androidisSafeArea == true)
            {
                offset = new Vector2(0, 60);
            }

            RectT.anchorMin = new Vector2(0,0);
            RectT.anchorMax = new Vector2(1,1);


            if (State == EnumAdaptation.Top)
            {
                RectT.offsetMax -= offset;
                return;
            }
            else if (State == EnumAdaptation.Bottom)
            {
                RectT.offsetMin += offset;
                return;
            }

            RectT.offsetMin += offset;
            RectT.offsetMax -= offset;
        }
    }
}