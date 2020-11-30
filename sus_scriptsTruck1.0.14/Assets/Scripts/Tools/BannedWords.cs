using System.Collections.Generic;
using pb;
using UnityEngine;
using UnityEngine.UI;

public class BannedWords : MonoBehaviour
{
    [Header("是否需要自动替换")]
    public bool needReplace = true;

    private List<t_Banned_Words> BanWords;
    private InputField InputTrans;
    // Start is called before the first frame update
    void Start()
    {
        BanWords = GameDataMgr.Instance.table.GetBannedWordsList();
        InputTrans = transform.GetComponent<InputField>();
        InputTrans.onValueChanged.AddListener(OnValueChanged);
    }

    /// <summary>
    /// 监听方法，该方法会在监测到输入值改变时被触发
    /// </summary>
    /// <param name="str"></param> 参数为当前输入的值
    private void OnValueChanged(string str)
    {
        if (BanWords == null || !needReplace)
            return;
        foreach (t_Banned_Words banWord in BanWords)
        {
            string word = banWord.BannedWord;
            if (str.Contains(word))
            {
                if (!word.Equals(""))
                {
                    Debug.Log("包含敏感词汇:" + word + ",需要进行替换");
                    int length = word.ToCharArray().Length;
                    string s = "";
                    for (int i = 0; i < length; i++)
                        s += "*";
                    string inputText = InputTrans.text;
                    inputText = inputText.Replace(word, s);
                    InputTrans.text = inputText;
                }
            }
        }

    }

    public bool haveBannedWords()
    {
        if (BanWords == null || needReplace)
            return false;
        
        string str = InputTrans.text;
        foreach (t_Banned_Words banWord in BanWords)
        {
            string word = banWord.BannedWord;
            if (str.Contains(word))
            {
                if (!word.Equals(""))
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
