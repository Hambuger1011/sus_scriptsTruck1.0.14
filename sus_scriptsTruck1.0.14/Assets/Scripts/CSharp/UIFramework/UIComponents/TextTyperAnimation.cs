using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.UI
{
    public class TextTyperAnimation : Text
    {
        public float Progress
        {
            get { return m_fProgress; }
            set
            {
                m_fProgress = Mathf.Clamp(value, 0f, 1);
                SetVerticesDirty();
            }
        }
        [SerializeField]
        [Range(0f, 1)]
        private float m_fProgress;

        readonly UIVertex[] m_TempVerts = new UIVertex[4];
        protected override void OnPopulateMesh(VertexHelper toFill)
        {

            if (font == null)
                return;

            // We don't care if we the font Texture changes while we are doing our Update.
            // The end result of cachedTextGenerator will be valid for this instance.
            // Otherwise we can get issues like Case 619238.
            m_DisableFontTextureRebuiltCallback = true;

            Vector2 extents = rectTransform.rect.size;

            var settings = GetGenerationSettings(extents);
            cachedTextGenerator.PopulateWithErrors(text, settings, gameObject);

            // Apply the offset to the vertices
            IList<UIVertex> verts = cachedTextGenerator.verts;
            float unitsPerPixel = 1 / pixelsPerUnit;
            //Last 4 verts are always a new line... (\n)
            int vertCount = verts.Count - 4;

            // We have no verts to process just return (case 1037923)
            if (vertCount <= 0)
            {
                toFill.Clear();
                return;
            }

            /*
            一个字符由4个顶点绘制,每3个顶点逆时针绘制是朝着屏幕可见，从左上角开始
            逆时针三角面渲染顶点顺序：012 230 
            */
            Vector2 roundingOffset = new Vector2(verts[0].position.x, verts[0].position.y) * unitsPerPixel;
            roundingOffset = PixelAdjustPoint(roundingOffset) - roundingOffset;
            toFill.Clear();

            int maxIdx = Mathf.RoundToInt(Mathf.Lerp(0, vertCount, this.m_fProgress));
            int lasShakeMaxIdx = Mathf.RoundToInt(Mathf.Lerp(0, vertCount, this.lastShakeProgress));

            if (roundingOffset != Vector2.zero)
            {
                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    m_TempVerts[tempVertsIndex] = verts[i];
                    m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                    m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
                    m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;
                    if (tempVertsIndex == 3)
                    {
                        SetAlpha(m_TempVerts, i, maxIdx);
                        ShakeVert(m_TempVerts, i, lasShakeMaxIdx);
                        toFill.AddUIVertexQuad(m_TempVerts);
                    }
                }
            }
            else
            {
                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    m_TempVerts[tempVertsIndex] = verts[i];
                    m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                    if (tempVertsIndex == 3)
                    {
                        SetAlpha(m_TempVerts, i, maxIdx);
                        ShakeVert(m_TempVerts, i, lasShakeMaxIdx);
                        toFill.AddUIVertexQuad(m_TempVerts);
                    }
                }
            }

            m_DisableFontTextureRebuiltCallback = false;

        }

        void SetAlpha(UIVertex[] verts, int beginIdx, int maxIdx)
        {
            Color32 color = verts[0].color;
            //左半边
            if (beginIdx <= maxIdx)
            {
                color.a = 255;
                verts[0].color = color;
                verts[3].color = color;
            }
            else
            {
                color.a = 0;
                verts[0].color = color;
                verts[3].color = color;
            }

            //右半边
            if (beginIdx + 1 <= maxIdx)
            {
                color.a = 255;
                verts[1].color = color;
                verts[2].color = color;
            }
            else
            {
                color.a = 0;
                verts[1].color = color;
                verts[2].color = color;
            }
        }

        void ShakeVert(UIVertex[] verts, int beginIdx, int lasShakeMaxIdx)
        {
            if (this.isShake /*&& beginIdx >= lasShakeMaxIdx*/)
            {
                Vector3 offset = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
                //var v = Random.Range(-2f, 2f);
                //Vector3 offset = new Vector3(v,v, 0);
                for (int i = 0; i < 4; ++i)
                {
                    verts[i].position += offset;
                }
            }
        }

        // private float getX(Vector2 anchorsMin,Vector2 anchorsMax)
        // {
        //     float per = 1f / cachedTextGenerator.lineCount;
        //     float range = 2f;
        //     if (anchorsMin == new Vector2(0.5f, 0.5f) && anchorsMax == new Vector2(0.5f, 0.5f))
        //     {
        //         float val = (Progress % per) / per;
        //         return this.RangeToRange(val, 0f, 1f, -rectTransform.sizeDelta.x / 2 - range, rectTransform.sizeDelta.x / 2 + range);
        //     }
        //     if (anchorsMin == new Vector2(0, 1) && anchorsMax == new Vector2(0, 1))
        //     {
        //         float val = (Progress % per) / per;
        //         return this.RangeToRange(val, 0f, 1f, (-rectTransform.sizeDelta.x) - range, 0f + range);
        //     }
        //     return 0f;
        // }


        // public float RangeToRange(float value, float orignalMin, float orignalMax, float targetMin, float targetMax)
        // {
        //     return (targetMax - targetMin) / (orignalMax - orignalMin) * (value - orignalMin) + targetMin;
        // }

        //tween
        private bool isTween = false;
        private float tweenDuration = 0.4f;
        public float tweenInterval = 0.02f;
        private float tweenTimer;
        public System.Action OnComplete;

        //shake
        private bool isShake = false;
        private float shakeDuration = 2;
        private float shakeInterval = 0.03f;
        private float shakeTimer = 0;
        private float shakeTick = 0;
        private float lastShakeProgress = 0;

        //CatGuid
        private bool isCatGuit = false;
        private float CatTweenTimer = 0;
        public float NeedTime = 0; //这个是调节文字的速度

        protected override void OnDisable()
        {
            base.OnDisable();
            EventDispatcher.Dispatch(UIEventMethodName.BookReadingForm_IsTweening.ToString(), false);
        }

        private void Update()
        {
            if (isTween)
            {
#if !NOT_USE_LUA
                if (!BookReadingWrapper.Instance.IsTextTween)
#else
                if (!DialogDisplaySystem.Instance.IsTextTween)
#endif
                {
                    EventDispatcher.Dispatch(UIEventMethodName.BookReadingForm_IsTweening.ToString(), true);
                }
               
                tweenTimer += Time.deltaTime;
                while (tweenTimer > tweenInterval)
                {
                    Progress += 1f / (tweenDuration / tweenInterval);
                    tweenTimer -= tweenInterval;
                    if (Progress >= 0.999f)
                    {
                        isTween = false;
                        if (OnComplete != null) OnComplete();

                        EventDispatcher.Dispatch(UIEventMethodName.BookReadingForm_IsTweening.ToString(), false);
                        tweenInterval = 0.02f;
                    }
                }
            }

            if (isShake)
            {
                shakeTick += Time.deltaTime;
                while (shakeTick >= shakeDuration)
                {
                    shakeTick -= shakeDuration;
                    this.lastShakeProgress = this.m_fProgress;
                    if (this.lastShakeProgress >= 0.999f)
                    {
                        this.isShake = false;
                        this.SetVerticesDirty();
                    }
                }
                shakeTimer += Time.deltaTime;
                while (shakeTimer > shakeInterval)
                {
                    shakeTimer -= shakeInterval;
                    this.SetVerticesDirty();
                }
            }


            if (isCatGuit)
            {
                CatTweenTimer += Time.deltaTime;
                while (CatTweenTimer >= NeedTime)
                {
                    Progress +=0.02f;
                    CatTweenTimer = 0;
                    if (Progress >= 0.999f)
                    {
                        Progress = 1;
                        isCatGuit = false;
                        UserDataManager.Instance.CatGuidIsCanTouch = true;

                        EventDispatcher.Dispatch(EventEnum.TouchAreaCanTouch);
                    }
                }              
            }
        }



        public void StopTyperTween()
        {
            if (isTween)
            {
                isTween = false;
                tweenTimer = 0f;
                Progress = 1f;
                if (OnComplete != null) OnComplete();

                EventDispatcher.Dispatch(UIEventMethodName.BookReadingForm_IsTweening.ToString(), false);

            }
        }

        [ContextMenu("DoTyperTween")]
        public void DoTyperTween()
        {
            isShake = false;
            isTween = true;
            tweenTimer = 0f;
            Progress = 0f;
            this.tweenDuration = Mathf.Max(0.4f, this.text.Length / 110.0f);
            //DoShake();
        }


        [ContextMenu("DoShake")]
        public void DoShake()
        {
            isShake = true;
            this.shakeTimer = 0f;
            this.shakeTick = 0;
            this.lastShakeProgress = 0;
        }

        [ContextMenu("CatDoTyperTween")]
        public void CatDoTyperTween()
        {
            isCatGuit = true;
            Progress = 0f;
            CatTweenTimer = 0;
        }

        public float GetPreferredHeight(string value)
        {
            var gen = this.cachedTextGeneratorForLayout;
            var settings = this.GetGenerationSettings(Vector2.zero);
            return gen.GetPreferredHeight(value, settings);
        }
    }

}


#if UNITY_EDITOR
public class TextTyperAnimationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        //if (GUILayout.Button("TextButtonText"))
        //{
        //}
    }
}


[CanEditMultipleObjects()]
[CustomEditor(typeof(TextTyperAnimation), true)]
public class CustomExtension : TextTyperAnimationEditor
{
}
#endif