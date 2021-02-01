using pb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[XLua.LuaCallCSharp, XLua.Hotfix]
public class BaseDialogData {

    #region property
    public int dialogID;
    public int chapterID;
    public string sceneID;
    public int role_id;
    public int icon;
    public int phiz_id;
    public int icon_bg;
    public int dialog_type;
    public string tips;
    public int BGMID;
    public float Scenes_X;
    public string dialog;
    public int trigger;
    public int next;
    public int selection_num;
    public string selection_1;
    public int requirement1;
    public int next_1;
    public string selection_2;
    public int requirement2;
    public int next_2;
    public string selection_3;
    public int requirement3;
    public int next_3;
    public string selection_4;
    public int requirement4;
    public int next_4;
    public int is_tingle;
    public int hidden_egg1;
    public int hidden_egg2;
    public int hidden_egg3;
    public int hidden_egg4;
    public int sceneActionX;
    public int sceneActionY;
    public int Orientation;
    public string Personalist1;
    public string Personalist2;
    public string Personalist3;
    public string Personalist4;
    public int TutorialEgg;
    public int ConsequenceID;
    public float sceneAlpha;
    public float propertyCheck;
    public string sceneColor;
    public int bubbleType;

    public int[] SceneParticalsArray;
    #endregion

    public BaseDialogData()
    {

    }

    public BaseDialogData(JDT_Dialog data)
    {
        this.dialogID = data.dialogid;
        this.chapterID = data.chapterid;
        this.sceneID = data.sceneid;
        this.role_id = data.role_id;
        this.icon = data.icon;
        this.phiz_id = data.phiz_id;
        this.icon_bg = data.icon_bg;
        this.dialog_type = data.dialog_type;
        this.tips = data.tips;
        this.BGMID = data.bgmid;
        this.Scenes_X = data.scenes_x;
        this.dialog = data.dialog;
        this.trigger = data.trigger;
        this.next = data.next;
        this.selection_num = data.selection_num;
        this.selection_1 = data.selection_1;
        this.requirement1 = data.requirement1;
        this.next_1 = data.next_1;
        this.selection_2 = data.selection_2;
        this.requirement2 = data.requirement2;
        this.next_2 = data.next_2;
        this.selection_3 = data.selection_3;
        this.requirement3 = data.requirement3;
        this.next_3 = data.next_3;
        this.selection_4 = data.selection_4;
        this.requirement4 = data.requirement4;
        this.next_4 = data.next_4;
        this.is_tingle = data.is_tingle;
        //this.SceneParticalsArray = data.sceneparticals.Split(',');
        // this.hidden_egg1 = data.hidden_egg1;
        // this.hidden_egg2 = data.hidden_egg2;
        // this.hidden_egg3 = data.hidden_egg3;
        // this.hidden_egg4 = data.hidden_egg4;
        // this.sceneActionX = data.SceneActionX;
        // this.sceneActionY = data.SceneActionY;
        this.Orientation = data.orientation;
        // this.Personalist1 = data.Personalit_1;
        // this.Personalist2 = data.Personalit_2;
        // this.Personalist3 = data.Personalit_3;
        // this.Personalist4 = data.Personalit_4;
        // this.TutorialEgg = data.tutorialegg;
        this.ConsequenceID = data.consequenceid;
        this.sceneAlpha = data.scenealpha;
        //this.bubbleType = data.bubbleType;
    }


    public virtual void ShowDialog()
    {
        EventDispatcher.Dispatch(UIEventMethodName.BookReadingForm_ShowDialogueType.ToString(), new Notification(this));
    }

    public virtual void StopDialog()
    {

    }

    public int[] GetSelectionsNext()
    {
        return new int[4] { next_1, next_2, next_3, next_4 };
    }
    public string[] GetSelectionsText()
    {
        return new string[4] { selection_1, selection_2, selection_3, selection_4 };
    }
    public int[] GetSelectionsCost()
    {
        return new int[4] { requirement1, requirement2, requirement3, requirement4 };
    }

    public string GetPersonalist(int vIndex)
    {
        if (vIndex == 0)
            return Personalist1;
        else if (vIndex == 1)
            return Personalist2;
        else if (vIndex == 2)
            return Personalist3;
        else if (vIndex == 3)
            return Personalist4;
        return string.Empty;
    }

    /// <summary>
    /// 彩蛋
    /// 类型1，奖励一个钻石
    /// </summary>
    /// <returns></returns>
    public int[] GetSelectionsHiddenEgg()
    {
        return new int[4] { hidden_egg1, hidden_egg2, hidden_egg3, hidden_egg4 };
    }
}

