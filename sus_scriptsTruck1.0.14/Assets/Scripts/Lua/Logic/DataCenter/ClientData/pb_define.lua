local pb = {}
---@class t_Banned_Words
pb.t_Banned_Words = 
{
	ID = 0,                                                                                            --int32,单词ID
	BannedWord = 0,                                                                                    --string,单词内容
}


---@class t_Localization
pb.t_Localization = 
{
	id = 0,                                                                                            --int32,编号
	text = 0,                                                                                          --string,文本内容
	dec = 0,                                                                                           --string,描述
}


---@class t_MetaGameDetails
pb.t_MetaGameDetails = 
{
	ID = 0,                                                                                            --int32,唯一ID
	GameType = 0,                                                                                      --int32,游戏类型
	ShapeDetails = 0,                                                                                  --int32,显示类型
	CharacterA = 0,                                                                                    --int32,角色A
	CharacterB = 0,                                                                                    --int32,角色B
	CharacterBClothes = 0,                                                                             --int32,角色B服装
	Pieces = 0,                                                                                        --int32,图片模块
	FinishEffect = 0,                                                                                  --int32,结束特效
	Description = 0,                                                                                   --string,玩法描述
	Postition_1 = 0,                                                                                   --string,位置1
	Postition_2 = 0,                                                                                   --string,位置2
	Postition_3 = 0,                                                                                   --string,位置3
	Postition_4 = 0,                                                                                   --string,位置4
	Postition_5 = 0,                                                                                   --string,位置5
	Postition_6 = 0,                                                                                   --string,位置6
	Postition_7 = 0,                                                                                   --string,位置7
	Postition_8 = 0,                                                                                   --string,位置8
}


---@class t_PlayerNames
pb.t_PlayerNames = 
{
	Name_ID = 0,                                                                                       --int32,名字ID
	male_name = 0,                                                                                     --string,男生名字
	female_name = 0,                                                                                   --string,女生名字
}

return pb
