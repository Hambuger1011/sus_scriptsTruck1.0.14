using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.UI;
using iTween.Easing;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;
using Framework;

public static class UITween
{
    public static void AddDiamond(Image imgDiamond, Text DiamondNumText, int oldNum, int newNum)
    {
        LOG.Info("钻石变更:"+ oldNum+" => "+ newNum);
        int num = newNum - oldNum;
        if(num <= 0)
        {
            //LOG.Error("添加钻石数量不正确:" + num);
            return;
        }
        GameFrameworkImpl.Instance.StartCoroutine(DoAddDiamondTween(imgDiamond, DiamondNumText, oldNum, newNum));
    }

    static IEnumerator DoAddDiamondTween(Image uiIcon, Text uiText, int oldNum, int newNum, Action callback = null)
    {
        int num = newNum - oldNum;
        var tipPos = new Vector2(0, 0);
        UITipsMgr.Instance.ShowUIPopTips("+" + num, tipPos);
        var uiForm = UITipsMgr.Instance.uiForm;
        Func<Vector3, Vector2> toUILocalPos = (worldPos) =>
        {
            return CUIUtility.World_To_UGUI_LocalPoint(uiForm.GetCamera(), uiForm.GetCamera(), worldPos, uiForm.rectTransform());
        };


        Vector2 startPos = Vector2.zero; ;
        var targetPos = toUILocalPos(uiIcon.transform.position);


        int count = Math.Min(num, 10);
        List <RectTransform> list = new List<RectTransform>(count);
        List<Vector2> posList = new List<Vector2>(count);
        List<float> moveTime = new List<float>(count);

        for (int i=0;i<count;++i)
        {
            var insideUnitCircle = Random.insideUnitCircle;
            if(insideUnitCircle.y < 0)
            {
                insideUnitCircle.y = 0;
            }
            var pos = insideUnitCircle * 100;
            posList.Add(pos);
            moveTime.Add(0);
        }
        //yield return new WaitForSeconds(1);

        float generateTime = 1f;//生成时间间隔
        float moveSpeed = 20;//飞行速度
        float _generateTime = 0;
        float _generateCount = 0;
        float flyTime = (targetPos - startPos).magnitude / moveSpeed;
        float _moveSpeed = 1 / flyTime;
        bool isArriveFirst = false;
        float rate = generateTime / count;
        float speed = 5;
        //int time = System.Environment.TickCount;
        while (posList.Count > 0)
        {
            if (_generateCount < count)
            {
                _generateTime += Time.deltaTime;
                for (int i = 0; i < Mathf.Ceil(_generateTime / rate); i++)
                {
                    if (_generateCount < count)
                    {
                        _generateCount++;
                        _generateTime -= rate;

                        int idx = list.Count;

                        if (uiIcon!=null)
                        {
                            var img = GameObject.Instantiate(uiIcon);
                            var trans = img.rectTransform;
                            trans.anchorMin = new Vector2(0.5f, 0.5f);
                            trans.anchorMax = new Vector2(0.5f, 0.5f);
                            trans.pivot = new Vector2(0.5f, 0.5f);

                            trans.SetParent(UITipsMgr.Instance.layers[0], false);
                            trans.anchoredPosition = posList[idx];
                            trans.Rotate(new Vector3(0, 0, Random.Range(-60, 60)));
                            list.Add(trans);
                        }                                        
                    }
                    else
                    {
                        break;
                    }
                }
            }

            var deltaTime = Time.deltaTime;//(System.Environment.TickCount - time) * 0.001f;
            //time = System.Environment.TickCount;
            for (int i=list.Count-1;i>=0;--i)
            {
                var t = list[i];
                moveTime[i] += deltaTime;
                //t.position = EasingLerps.EasingLerp(EasingLerps.EasingLerpsType.Sine, EasingLerps.EasingInOutType.EaseOut, posList[i], imgDiamond.rectTransform.position, moveTime[i]);//Vector3.Lerp(posList[i], imgDiamond.rectTransform.position, moveTime[i]);

                if(uiIcon)
                {
                    targetPos = toUILocalPos(uiIcon.transform.position);
                }
                var changeValue = targetPos - posList[i];
                float easeVal = EaseManager.Evaluate(iTween.Easing.Ease.Linear, null, moveTime[i], 1);
                changeValue *= easeVal;
                t.anchoredPosition = posList[i] + changeValue;
                if (easeVal >= 1)
                {
                    list.RemoveAt(i);
                    moveTime.RemoveAt(i);
                    posList.RemoveAt(i);
                    GameObject.Destroy(t.gameObject);
                }
            }
            yield return null;
        }

        
        int v = oldNum;
        DOTween.Kill(uiText);
        DOTween.To(() =>
        {
            return v;
        }, (f) =>
        {
            if (!uiText)
            {
                return;
            }
            v = f;
            uiText.text = v.ToString();
        }, newNum, 0.6f).SetId(uiText);
        if(callback != null)
        {
            callback();
        }

    }
}
