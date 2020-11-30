using AB;
using DG.Tweening;
using pb;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using XLua;

public class ComuniadaBookSelf : BookSelfBasse
{

    private Text BookName;
    private Image imgCover;
    private WriterIndexBass WriterIndexBass;
    private Vector2 defaultSize;
    private Sprite defaultSprite;
    private int m_downloadSeq = 1;

    private void Awake()
    {
        imgCover = this.transform.Find("BookMask/BookIconImage").GetComponent<Image>();
        defaultSize = imgCover.rectTransform.rect.size;
        defaultSprite = this.imgCover.sprite;

        BookName = transform.Find("BookName").GetComponent<Text>();
        EventTriggerListener.Get(this.gameObject).onClick = OnClick;
    }
    public string bookTypeName = "";
    private void OnClick(GameObject go)
    {
        var luaenv = XLuaManager.Instance.GetLuaEnv();
        var res = luaenv.DoString(@"
return function(bookID)
    logic.StoryEditorMgr:ReadingOtherBook(bookID)
end");
        using (var func = (LuaFunction)res[0])
        {
            func.Action<int>(this.WriterIndexBass.id);
        }


        int _bookid = this.WriterIndexBass.id;

        if (bookTypeName != "")
        {
            if (bookTypeName == "Historias")
            {
                //埋点*点击最受欢迎书本
                GamePointManager.Instance.BuriedPoint(EventEnum.UgcSelectHotBook, "", "", _bookid.ToString());
            }
            else if (bookTypeName == "Nuevas")
            {
                //埋点*点击最新书本
                GamePointManager.Instance.BuriedPoint(EventEnum.UgcSelectNewBook, "", "", _bookid.ToString());
            }
        }


    }

    public void Init(WriterIndexBass WriterIndexBass)
    {
        m_downloadSeq += 1;
        this.WriterIndexBass = WriterIndexBass;
        Show();
        LoadSprite();
    }

    public void Show()
    {
        BookName.text = WriterIndexBass.title.ToString();
        
    }

    public void DestroyGameObject()
    {
        if (gameObject != null)
            Destroy(gameObject);
    }


    void LoadSprite()
    {
        this.imgCover.sprite = defaultSprite;
        var downloadSeq = m_downloadSeq;
        var luaenv = XLuaManager.Instance.GetLuaEnv();
        var res = luaenv.DoString(@"
return function(filename, fileMd5, cover_image,callback)
        logic.StoryEditorMgr.data:LoadSprite(filename,fileMd5, cover_image,callback)
end"
);
        using (var func = (LuaFunction)res[0])
        {
            //var bookID = this.WriterBookList.id;
            string imgUrl;
            string md5;
            XLuaHelper.ParseImageUrl(this.WriterIndexBass.cover_image, out imgUrl, out md5);
            var filename = GameUtility.WritablePath + "/cache/story_image/" + md5;
            var f = (Action<object, object, object, object>)func.Cast(typeof(Action<object, object, object, object>));
            f(filename, md5, imgUrl, new Action<Sprite>((sprite) =>
            {
                if (downloadSeq != m_downloadSeq)
                {
                    return;
                }
                if (this.imgCover == null)
                {
                    return;
                }
                if (sprite == null)
                {
                    sprite = this.defaultSprite;
                }
                //XLuaHelper.SetSprite(this.imgCover, sprite, defaultSize);
                var doSetCover = luaenv.DoString(@"
                    return function(uiImage,sprite,defaultSize)
                            logic.StoryEditorMgr:SetCover(uiImage,sprite,defaultSize)
                    end"
                    );
                using (var func2 = (LuaFunction)doSetCover[0])
                {
                    func2.Call(this.imgCover, sprite, defaultSize);
                }
            }));
        }
    }
}
