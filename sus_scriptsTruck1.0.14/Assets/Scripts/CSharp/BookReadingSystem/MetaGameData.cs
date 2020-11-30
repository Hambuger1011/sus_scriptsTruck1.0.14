using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using pb;
/// <summary>
/// 小游戏的配置数据
/// </summary>
public class MetaGameData  
{
    #region property
    public int ID { get; protected set; }
    public int GameType { get; protected set; }
    public int ShapeDetails { get; protected set; }
    public int CharacterA { get; protected set; }
    public int CharacterB { get; protected set; }
    public int CharacterBClothes { get; protected set; }
    public int Pieces { get; protected set; }
    public string Description { get; protected set; }
    public string Postition_1 { get; protected set; }
    public string Postition_2 { get; protected set; }
    public string Postition_3 { get; protected set; }
    public string Postition_4 { get; protected set; }
    public string Postition_5 { get; protected set; }
    public string Postition_6 { get; protected set; }
    public string Postition_7 { get; protected set; }
    public string Postition_8 { get; protected set; }
    #endregion

    public MetaGameData(t_MetaGameDetails data)
    {
        this.ID = data.ID;
        this.GameType = data.GameType;
        this.ShapeDetails = data.ShapeDetails;
        this.CharacterA = data.CharacterA;
        this.CharacterB = data.CharacterB;
        this.CharacterBClothes = data.CharacterBClothes;
        this.Pieces = data.Pieces;
        this.Description = data.Description;
        this.Postition_1 = data.Postition_1;
        this.Postition_2 = data.Postition_2;
        this.Postition_3 = data.Postition_3;
        this.Postition_4 = data.Postition_4;
        this.Postition_5 = data.Postition_5;
        this.Postition_6 = data.Postition_6;
        this.Postition_7 = data.Postition_7;
        this.Postition_8 = data.Postition_8;
    }

    public string[] GetPostList()
    {
        return new string[8] { Postition_1, Postition_2, Postition_3, Postition_4, Postition_5, Postition_6, Postition_7, Postition_8 };
    }


}
