local pb = {}
---@class t_Banned_Words
pb.t_Banned_Words = 
{
	ID = 0,                                                                                            --int32,单词ID
	BannedWord = 0,                                                                                    --string,单词内容
}


---@class t_BookDialog
pb.t_BookDialog = 
{
	dialogID = 0,                                                                                      --int32,对话编号
	chapterID = 0,                                                                                     --int32,章节编号
	sceneID = 0,                                                                                       --string,场景编号
	sceneAlpha = 0,                                                                                    --float,场景透明度
	role_id = 0,                                                                                       --int32,角色ID
	icon = 0,                                                                                          --int32,对话头像
	phiz_id = 0,                                                                                       --int32,表情ID
	icon_bg = 0,                                                                                       --int32,头像框底色
	is_tingle = 0,                                                                                     --int32,窗口抖动
	dialog_type = 0,                                                                                   --int32,对话类型
	Orientation = 0,                                                                                   --int32,角色朝向
	PhoneCall = 0,                                                                                     --int32,电话剧情
	tips = 0,                                                                                          --string,事件提示
	BGMID = 0,                                                                                         --int32,背景音乐ID
	SFX = 0,                                                                                           --string,音效ID
	SceneParticals = 0,                                                                                --string,特效
	Scenes_X = 0,                                                                                      --float,场景对话坐标
	SceneActionX = 0,                                                                                  --int32,场景操作X
	SceneActionY = 0,                                                                                  --int32,场景操作Y
	Barrage = 0,                                                                                       --int32,弹幕属性
	dialog = 0,                                                                                        --string,对白内容
	ConsequenceID = 0,                                                                                 --int32,选项记录
	trigger = 0,                                                                                       --int32,
	next = 0,                                                                                          --int32,转进对话
	selection_num = 0,                                                                                 --int32,选项数目
	TutorialEgg = 0,                                                                                   --int32,新手彩蛋
	selection_1 = 0,                                                                                   --string,选项1
	Personalit_1 = 0,                                                                                  --string,性格1
	requirement1 = 0,                                                                                  --int32,限制类型1
	next_1 = 0,                                                                                        --int32,转进对话1
	hidden_egg1 = 0,                                                                                   --int32,彩蛋
	selection_2 = 0,                                                                                   --string,选项2
	Personalit_2 = 0,                                                                                  --string,性格2
	requirement2 = 0,                                                                                  --int32,限制类型2
	next_2 = 0,                                                                                        --int32,转进对话2
	hidden_egg2 = 0,                                                                                   --int32,彩蛋
	selection_3 = 0,                                                                                   --string,选项3
	Personalit_3 = 0,                                                                                  --string,性格3
	requirement3 = 0,                                                                                  --int32,限制类型3
	next_3 = 0,                                                                                        --int32,转进对话3
	hidden_egg3 = 0,                                                                                   --int32,彩蛋
	selection_4 = 0,                                                                                   --string,选项4
	Personalit_4 = 0,                                                                                  --string,性格4
	requirement4 = 0,                                                                                  --int32,限制类型4
	next_4 = 0,                                                                                        --int32,转进对话4
	hidden_egg4 = 0,                                                                                   --int32,彩蛋
	modelid = 0,                                                                                       --int32,形象id
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


---@class t_Skin
pb.t_Skin = 
{
	skin_id = 0,                                                                                       --int32,皮肤ID
	book_id = 0,                                                                                       --int32,书本ID
	icon_id = 0,                                                                                       --int32,图片资源ID
	dec = 0,                                                                                           --string,描述
}

return pb
