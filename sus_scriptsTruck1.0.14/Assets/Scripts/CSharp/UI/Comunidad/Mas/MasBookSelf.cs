using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

public class MasBookSelf : MonoBehaviour
{
    private Text BookName;
    private WriterBookList WriterBookList;
    private Image imgCover;
    private Vector2 defaultSize;
    private Sprite defaultSprite;
    private int m_downloadSeq = 1;

    private void Awake()
    {
        FindGameObject();
        EventTriggerListener.Get(this.gameObject).onClick = OnClick;
    }
    private void FindGameObject()
    {
        BookName = transform.Find("BookName").GetComponent<Text>();
        imgCover = this.transform.Find("BookMask/BookIconImage").GetComponent<Image>();
        defaultSize = imgCover.rectTransform.rect.size;
        defaultSprite = this.imgCover.sprite;
    }

    public void SetItemData(WriterBookList WriterBookList,int index, int row, int column)
    {
        m_downloadSeq += 1;
        this.WriterBookList = WriterBookList;
        Show();
    }

    public void DestroyGameObject()
    {
        if (gameObject != null)
            Destroy(gameObject);
    }


    private void Show()
    {
        BookName.text = WriterBookList.title.ToString();
        LoadSprite();
    }

    

    private void OnClick(GameObject go)
    {
        var luaenv = XLuaManager.Instance.GetLuaEnv();
        var res = luaenv.DoString(@"
return function(bookID)
    logic.StoryEditorMgr:ReadingOtherBook(bookID)
end");
        using (var func = (LuaFunction)res[0])
        {
            func.Action<int>(this.WriterBookList.id);
        }
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
            XLuaHelper.ParseImageUrl(this.WriterBookList.cover_image, out imgUrl, out md5);
            var filename = GameUtility.WritablePath + "/cache/story_image/" + md5;
            var f = (Action<object, object, object, object>)func.Cast(typeof(Action<object, object, object, object>));
            f(filename, md5, imgUrl, new Action<Sprite>((sprite) =>
            {
                if(downloadSeq != m_downloadSeq)
                {
                    return;
                }
                if (this.imgCover == null)
                {
                    return;
                }
                if(sprite == null)
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
