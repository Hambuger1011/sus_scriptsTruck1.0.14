using UnityEngine;
using UnityEngine.UI;

public class MoneyChange : MonoBehaviour
{
    private Text CloverCount;
    private Text KeyCount;
    private Text DiamondCount;




    private Image imgClover;
    private Image imgKey;
    private Image imgDiamond;

    private void Awake()
    {
        CloverCount = DisplayUtil.GetChildComponent<Text>(this.gameObject, "CloverCount");
        KeyCount = DisplayUtil.GetChildComponent<Text>(this.gameObject, "KeyCount");
        DiamondCount = DisplayUtil.GetChildComponent<Text>(this.gameObject, "DiamondCount");
    }


    public void DOTweenToClover(int oldNum, int newNum)
    {
        DG.Tweening.DOTween.To(() => oldNum, (value) => { CloverCount.text = value.ToString(); }, newNum, 2);
    }

    public void DOTweenToKey(int oldNum,int newNum)
    {
       DG.Tweening.DOTween.To(() => oldNum, (value) => { KeyCount.text = value.ToString(); }, newNum, 2);
    }

    public void DOTweenToDiamond(int oldNum, int newNum)
    {
        DG.Tweening.DOTween.To(() => oldNum, (value) => { DiamondCount.text = value.ToString(); }, newNum, 2);
    }



    public void PlayAnimaClover(int oldNum, int newNum)
    {
        UITween.AddDiamond(imgClover, CloverCount, oldNum, newNum);
    }

    public void PlayAnimaKey(int oldNum, int newNum)
    {
        UITween.AddDiamond(imgKey, KeyCount, oldNum, newNum);
    }

    public void PlayAnimaDiamond(int oldNum, int newNum)
    {
        UITween.AddDiamond(imgDiamond, DiamondCount, oldNum, newNum);
    }

}
