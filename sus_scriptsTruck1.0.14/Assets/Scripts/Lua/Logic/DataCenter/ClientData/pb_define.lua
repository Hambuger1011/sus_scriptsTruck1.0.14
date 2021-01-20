local pb = {}
---@class t_Activity
pb.t_Activity = 
{
	ID = 0,                                                                                            --int32,编号
	Activityname = 0,                                                                                  --string,活动名字
	Activityid = 0,                                                                                    --int32,活动编号
	Picture = 0,                                                                                       --string,活动图片
	state = 0,                                                                                         --int32,活动状态
}


---@class t_Attribute
pb.t_Attribute = 
{
	id = 0,                                                                                            --int32,ID
	name = 0,                                                                                          --string,动物名
	describe = 0,                                                                                      --string,动物描述
	type = 0,                                                                                          --int32,动物类型
	personality = 0,                                                                                   --string,动物性格描述
	appearance = 0,                                                                                    --string,外貌
	limit = 0,                                                                                         --string,性格要求
	level = 0,                                                                                         --int32,动物稀有度
	headres = 0,                                                                                       --string,动物头像资源
	pro = 0,                                                                                           --float,基础出现概率
	consumption = 0,                                                                                   --float,食物消耗速度/min
	love1 = 0,                                                                                         --float,未收养爱心产出/min
	diamond1 = 0,                                                                                      --float,未收养钻石产出/min
	love2 = 0,                                                                                         --float,收养爱心产出/min
	diamond2 = 0,                                                                                      --float,收养钻石产出/min
	adopt_pro = 0,                                                                                     --float,收养成功基础概率
	diamond_qty = 0,                                                                                   --int32,收养价格
	Affinity_up = 0,                                                                                   --float,亲密度增长速度/min
	Affinity_down = 0,                                                                                 --float,亲密度减少速度/min
	fulltime = 0,                                                                                      --int32,产出满所需时间
	sort = 0,                                                                                          --int32,排序
}


---@class t_Banned_Words
pb.t_Banned_Words = 
{
	ID = 0,                                                                                            --int32,单词ID
	BannedWord = 0,                                                                                    --string,单词内容
}


---@class t_BookChat_1
pb.t_BookChat_1 = 
{
	id = 0,                                                                                            --int32,对话id
	type1 = 0,                                                                                         --int32,角色类型
	type2 = 0,                                                                                         --int32,消息类型
	content = 0,                                                                                       --string,消息内容
}


---@class t_BookDetails
pb.t_BookDetails = 
{
	id = 0,                                                                                            --int32,书id
	BookName = 0,                                                                                      --string,书名
	BookTypeName = 0,                                                                                  --string,书本类型名字
	BookSearchType = 0,                                                                                --string,书本搜索类型
	BookIcon = 0,                                                                                      --int32,书本图标
	Novice = 0,                                                                                        --int32,新手推荐书本
	Gender = 0,                                                                                        --int32,故事性别
	Type1 = 0,                                                                                         --string,主要类型
	TypeTotal = 0,                                                                                     --string,类型比例
	NewBook = 0,                                                                                       --int32,是否新书本
	GayBook = 0,                                                                                       --int32,是否同性恋内容
	ChapterCount = 0,                                                                                  --int32,章节数量
	BannerIcon = 0,                                                                                    --string,Banner图标
	Subscribe = 0,                                                                                     --int32,是否为预约书
	ChapterDivision = 0,                                                                               --string,章节结束ID
	ChapterName = 0,                                                                                   --string,章节名
	ChapterDiscription = 0,                                                                            --string,章节描述
	BookCharacterName = 0,                                                                             --string,角色名
	BookShelfPriority = 0,                                                                             --int32,书架优先级
	IsOpen = 0,                                                                                        --int32,是否开放
	OpenTime = 0,                                                                                      --string,开放时间
	EndTime = 0,                                                                                       --string,结束时间
	ChapterPrice = 0,                                                                                  --string,章节消耗
	EggPrice = 0,                                                                                      --int32,彩蛋奖励次数
	Availability = 0,                                                                                  --int32,书本开放地区
	ChapterOpen = 0,                                                                                   --int32,章节开放阅读
	DescriptionColor = 0,                                                                              --string,描述颜色
	DialogColor = 0,                                                                                   --string,对话颜色
	EnterNameColor = 0,                                                                                --string,输入名颜色
	SelectionColor = 0,                                                                                --string,选项颜色
	DialogFrameHeight = 0,                                                                             --int32,对话框高度
	RoleScale = 0,                                                                                     --int32,角色的大小比例
	ChapterRelease = 0,                                                                                --int32,书本是否连载完
	ReleaseDay = 0,                                                                                    --int32,章节开放日期
	ChapterUpdateNum = 0,                                                                              --int32,每周章节更新的数量
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
	sceneColor = 0,                                                                                    --string,场景颜色
	propertyCheck = 0,                                                                                 --int32,属性检测
}


---@class t_BookShelf
pb.t_BookShelf = 
{
	Id = 0,                                                                                            --int32,书架ID
	BookShelfIcon = 0,                                                                                 --int32,书架图标
	BookListName = 0,                                                                                  --string,书架昵称
	BookIDList = 0,                                                                                    --string,书架书本ID
}


---@class t_BookTutorial
pb.t_BookTutorial = 
{
	dialogID = 0,                                                                                      --int32,对话编号
	role_id = 0,                                                                                       --int32,角色ID
	phiz_id = 0,                                                                                       --int32,表情ID
	icon_bg = 0,                                                                                       --int32,头像框底色
	is_tingle = 0,                                                                                     --int32,窗口抖动
	dialog_type = 0,                                                                                   --int32,对话类型
	Scenes_X = 0,                                                                                      --float,场景对话坐标
	dialog = 0,                                                                                        --string,对白内容
	trigger = 0,                                                                                       --int32,
	next = 0,                                                                                          --int32,转进对话
	selection_num = 0,                                                                                 --int32,选项数目
	selection_1 = 0,                                                                                   --string,选项1
	next_1 = 0,                                                                                        --int32,转进对话1
	Type1 = 0,                                                                                         --string,类型选项1
	TypeTotal1 = 0,                                                                                    --string,类型比例1
	selection_2 = 0,                                                                                   --string,选项2
	next_2 = 0,                                                                                        --int32,转进对话2
	Type2 = 0,                                                                                         --string,类型选项2
	TypeTotal2 = 0,                                                                                    --string,类型比例2
	selection_3 = 0,                                                                                   --string,选项3
	next_3 = 0,                                                                                        --int32,转进对话3
	Type3 = 0,                                                                                         --string,类型选项3
	TypeTotal3 = 0,                                                                                    --string,类型比例3
}


---@class t_BulletinBoard
pb.t_BulletinBoard = 
{
	id = 0,                                                                                            --int32,编号
	lasttime = 0,                                                                                      --int32,持续时间/天
	text = 0,                                                                                          --string,版本更新内容
	num = 0,                                                                                           --int32,图片数量
	picture1 = 0,                                                                                      --string,图片1
	type1 = 0,                                                                                         --int32,类型1
	idtype1 = 0,                                                                                       --int32,类型id1
	picture2 = 0,                                                                                      --string,图片2
	type2 = 0,                                                                                         --int32,类型2
	idtype2 = 0,                                                                                       --int32,类型id2
	picture3 = 0,                                                                                      --string,图片3
	type3 = 0,                                                                                         --int32,类型3
	idtype3 = 0,                                                                                       --int32,类型id3
	picture4 = 0,                                                                                      --string,图片4
	type4 = 0,                                                                                         --int32,类型4
	idtype4 = 0,                                                                                       --int32,类型id4
	picture5 = 0,                                                                                      --string,图片5
	type5 = 0,                                                                                         --int32,类型5
	idtype5 = 0,                                                                                       --int32,类型id5
}


---@class t_Clotheprice
pb.t_Clotheprice = 
{
	UniqueID = 0,                                                                                      --int32,服装唯一ID
	BookID = 0,                                                                                        --int32,书本ID
	ClotheID = 0,                                                                                      --int32,服装ID
	ClothePrice = 0,                                                                                   --int32,价格
	PriceType = 0,                                                                                     --int32,价格类型
}


---@class t_contrast
pb.t_contrast = 
{
	id = 0,                                                                                            --int32,ID
	name = 0,                                                                                          --int32,猫id
	decorations = 0,                                                                                   --string,装饰物
	pro = 0,                                                                                           --string,猫的出现概率
	param1 = 0,                                                                                        --string,参数1
	display = 0,                                                                                       --int32,原装饰物是否显示
	res = 0,                                                                                           --string,装饰物名
	num = 0,                                                                                           --string,动作序号
	frame = 0,                                                                                         --string,动作帧数
}


---@class t_decorations
pb.t_decorations = 
{
	id = 0,                                                                                            --int32,ID
	name = 0,                                                                                          --string,装饰物名称
	type = 0,                                                                                          --int32,装饰物类型
	size = 0,                                                                                          --int32,装饰物大小
	res = 0,                                                                                           --string,资源
}


---@class t_BookFAQ
pb.t_BookFAQ = 
{
	ID = 0,                                                                                            --int32,编号
	QuestionID = 0,                                                                                    --string,问题编号
	Question = 0,                                                                                      --string,问题
	AnswerID = 0,                                                                                      --string,答案编号
	Answer = 0,                                                                                        --string,答案
}


---@class t_food
pb.t_food = 
{
	id = 0,                                                                                            --int32,ID
	name = 0,                                                                                          --string,食物名称
	type = 0,                                                                                          --int32,食物类型
	weight = 0,                                                                                        --int32,重量
	pro1 = 0,                                                                                          --float,1星概率
	pro2 = 0,                                                                                          --float,2星概率
	pro3 = 0,                                                                                          --float,3星概率
	pro4 = 0,                                                                                          --float,4星概率
	pro5 = 0,                                                                                          --float,5星概率
}


---@class t_gif
pb.t_gif = 
{
	id = 0,                                                                                            --int32,ID
	name = 0,                                                                                          --int32,猫id
	act = 0,                                                                                           --int32,动作计数
	frame = 0,                                                                                         --string,序列帧
	time = 0,                                                                                          --string,每一帧对应的时间
	stop = 0,                                                                                          --int32,停留帧
	frequency = 0,                                                                                     --string,播放频率(区间)
	num = 0,                                                                                           --string,每次循环次数(区间)
}


---@class t_InviteTask
pb.t_InviteTask = 
{
	ID = 0,                                                                                            --int32,编号
	TaskName = 0,                                                                                      --string,任务名
	Type = 0,                                                                                          --int32,任务类型
	TaskDdescribe = 0,                                                                                 --string,任务描述
	RewardDiamonds = 0,                                                                                --int32,邀请奖励（钻石）
	RewardKey = 0,                                                                                     --int32,邀请奖励（钥匙）
	RewardWeek = 0,                                                                                    --int32,邀请奖励（周卡）
	Param = 0,                                                                                         --int32,参数
	Icon = 0,                                                                                          --string,奖励图标
	Sort = 0,                                                                                          --int32,排序
}


---@class t_Loading_Tips
pb.t_Loading_Tips = 
{
	ID = 0,                                                                                            --int32,提示ID
	LoadingTips = 0,                                                                                   --string,英文提示内容
}


---@class t_Localization
pb.t_Localization = 
{
	id = 0,                                                                                            --int32,编号
	text = 0,                                                                                          --string,文本内容
	dec = 0,                                                                                           --string,描述
}


---@class t_map
pb.t_map = 
{
	id = 0,                                                                                            --int32,ID
	type = 0,                                                                                          --int32,坐标类型
	coordinates = 0,                                                                                   --string,坐标
	proportion = 0,                                                                                    --float,放大比例
	sort = 0,                                                                                          --int32,开放顺序
	right = 0,                                                                                         --float,开放大小（向右）
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


---@class t_RoleModelData
pb.t_RoleModelData = 
{
	book_id = 0,                                                                                       --int32,书本ID
	type = 0,                                                                                          --int32,类型
	item_id = 0,                                                                                       --int32,类型对应ID
	price = 0,                                                                                         --int32,价格
	description = 0,                                                                                   --string,描述
}


---@class t_ChapterDivide
pb.t_ChapterDivide = 
{
	id = 0,                                                                                            --int32,ID
	bookId = 0,                                                                                        --int32,书本ID
	chapter = 0,                                                                                       --int32,章节ID
	ChapterStart = 0,                                                                                  --int32,章节开始ID
	ChapterFinish = 0,                                                                                 --int32,章节结束ID
	chapterName = 0,                                                                                   --string,章节名字
	dsc = 0,                                                                                           --string,章节描述
	open = 0,                                                                                          --int32,是否开放
	chapterPay = 0,                                                                                    --int32,是否需付费
	payType = 0,                                                                                       --int32,付费类型
	payAmount = 0,                                                                                     --int32,付费数量
	rewardType = 0,                                                                                    --int32,鼓励类型
	rewardAmount = 0,                                                                                  --int32,鼓励数量
}


---@class t_Ornament
pb.t_Ornament = 
{
	ID = 0,                                                                                            --int32,编号
	Name = 0,                                                                                          --string,名称
	Res = 0,                                                                                           --int32,资源
	Type = 0,                                                                                          --int32,类型
	Base = 0,                                                                                          --int32,是否默认
	Describe = 0,                                                                                      --string,描述
	Param = 0,                                                                                         --int32,参数
	Sort = 0,                                                                                          --int32,排序
}


---@class t_personality
pb.t_personality = 
{
	ID = 0,                                                                                            --int32,编号
	Personality = 0,                                                                                   --string,性格特点
}


---@class t_analysisA
pb.t_analysisA = 
{
	id = 0,                                                                                            --int32,ID
	group = 0,                                                                                         --string,性格组
	text1 = 0,                                                                                         --string,文本1
	text2 = 0,                                                                                         --string,文本2
	text3 = 0,                                                                                         --string,文本3
	text4 = 0,                                                                                         --string,文本4
	text5 = 0,                                                                                         --string,文本5
}


---@class t_analysisB
pb.t_analysisB = 
{
	id = 0,                                                                                            --int32,ID
	group = 0,                                                                                         --string,性格组
	text1 = 0,                                                                                         --string,文本1
	text2 = 0,                                                                                         --string,文本2
	text3 = 0,                                                                                         --string,文本3
	text4 = 0,                                                                                         --string,文本4
	text5 = 0,                                                                                         --string,文本5
}


---@class t_analysisC
pb.t_analysisC = 
{
	id = 0,                                                                                            --int32,ID
	group = 0,                                                                                         --string,性格组
	text1 = 0,                                                                                         --string,文本1
	text2 = 0,                                                                                         --string,文本2
	text3 = 0,                                                                                         --string,文本3
	text4 = 0,                                                                                         --string,文本4
	text5 = 0,                                                                                         --string,文本5
}


---@class t_PlayerNames
pb.t_PlayerNames = 
{
	Name_ID = 0,                                                                                       --int32,名字ID
	male_name = 0,                                                                                     --string,男生名字
	female_name = 0,                                                                                   --string,女生名字
}


---@class t_RoleModel
pb.t_RoleModel = 
{
	book_id = 0,                                                                                       --int32,书本ID
	model_id = 0,                                                                                      --int32,model
	character_type1 = 0,                                                                               --int32,形象ID
	hair_type2 = 0,                                                                                    --int32,头发ID
	outfit_type3 = 0,                                                                                  --int32,服装ID
	remark = 0,                                                                                        --string,备注
}


---@class t_shop
pb.t_shop = 
{
	id = 0,                                                                                            --int32,ID
	name = 0,                                                                                          --string,装饰物名称
	shopid = 0,                                                                                        --int32,装饰物id
	describe = 0,                                                                                      --string,装饰物描述
	pay = 0,                                                                                           --int32,货币类型
	love = 0,                                                                                          --int32,消耗爱心
	diamond = 0,                                                                                       --int32,消耗钻石
	size = 0,                                                                                          --int32,商品类型
	res = 0,                                                                                           --string,资源
}


---@class t_Skin
pb.t_Skin = 
{
	skin_id = 0,                                                                                       --int32,皮肤ID
	book_id = 0,                                                                                       --int32,书本ID
	icon_id = 0,                                                                                       --int32,图片资源ID
	dec = 0,                                                                                           --string,描述
}


---@class t_story
pb.t_story = 
{
	id = 0,                                                                                            --int32,ID
	title = 0,                                                                                         --string,标题
	story1 = 0,                                                                                        --string,故事1
	pic1 = 0,                                                                                          --string,插图配置1
	story2 = 0,                                                                                        --string,故事2
	pic2 = 0,                                                                                          --string,插图配置2
	story3 = 0,                                                                                        --string,故事3
	pic3 = 0,                                                                                          --string,插图配置3
}

return pb
