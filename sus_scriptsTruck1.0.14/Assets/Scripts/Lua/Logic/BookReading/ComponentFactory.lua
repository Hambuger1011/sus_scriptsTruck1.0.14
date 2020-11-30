local Class = core.Class

local ComponentFactory = Class("ComponentFactory")

logic.BookReading = {}
logic.BookReading.BaseComponent = require("Logic/BookReading/Component/BaseComponent")
logic.BookReading.BaseDialogueComponent = require("Logic/BookReading/Component/Dialogue/BaseDialogueComponent")
logic.DialogType =
{
    Negative = 0,
    Narration = 1,                  --旁白
    EnterName = 2,				--输入名字
    PlayerDialogue = 3,
    OtherDialogue = 4,
    ChangeClothes = 5,
    ChangeSceneByBlack = 6,
    PlayerImagineDialogue = 7,
    OtherImagineDialogue = 8,
    ChoiceCharacter = 9,
    PhoneCallDialogue = 10,
    SceneInteraction = 11,  --场景互动，即：背景图上，有互动的特效
    ManualChangeScene = 12, --手动切换场景
    SceneTap = 13,          --（记录一段时间内）点击屏幕的次数
    Puzzle = 14,            --拼图
    ClockChangeScene = 15,  --旋转时钟的形式，来切换场景
    EnterNPCname=16,--Npc 的名字   
    ChangeSceneByWhite = 17,--场景变白切换场景
    ChangeSceneByWave = 18,-- 水波纹切换场景
    ChangeSceneByShutter = 19,--百叶窗切换场景
    ChangeWholeClothes = 20,    --换全身服装
    ChoiceRole = 21,    --选择角色
    StoryItems = 23,    --获得饰品
    ChangeSceneToBlack = 24,    --场景变黑切换场景(不移动场景)
    BubbleChat = 25,        --气泡聊天（指微信、短信、facebook的聊天模式）,切进，或者切出
    BubbleNarration = 26,   --气泡-场白
    BubblePlayerDialog = 27,    --气泡-主角表述
    BubbleOtherPlayerDialog = 28,   --气泡-配角表述
    AutoSelectClothes = 29, --自动帮主角选择服装
    Splitview = 30, --分屏背景
    GotoChapter = 31, --章节id跳转
    ChoiceSex = 32, --性别选择
    ChoiceModel = 33, --选择头发、形象
    TitlePage = 34, --扉页
    AutoSelectClothesModel = 35, --自动帮主角选择服装(model)
}

local components = {}

function ComponentFactory:__init()

  --region 对白
  components[logic.DialogType.Narration] = function(index,cfg)
    local class = require("Logic/BookReading/Component/Dialogue/NarrationComponent")
    local obj = class.New(index,cfg)
		return obj
  end
  components[logic.DialogType.PlayerDialogue] = function(index,cfg)
    local class = require("Logic/BookReading/Component/Dialogue/PlayerDialogueComponent")
    local obj = class.New(index,cfg)
    obj.type = 0
    return obj
	end
  components[logic.DialogType.OtherDialogue] = function(index,cfg)
    local class = require("Logic/BookReading/Component/Dialogue/OtherDialogueComponent")
    local obj = class.New(index,cfg)
    obj.type = 0
    return obj
	end
  components[logic.DialogType.PlayerImagineDialogue] = function(index,cfg)
    local class = require("Logic/BookReading/Component/Dialogue/PlayerDialogueComponent")
    local obj = class.New(index,cfg)
    obj.type = 1
    return obj
	end
  components[logic.DialogType.OtherImagineDialogue] = function(index,cfg)
    local class = require("Logic/BookReading/Component/Dialogue/OtherDialogueComponent")
    local obj = class.New(index,cfg)
    obj.type = 1
    return obj
	end
  components[logic.DialogType.PhoneCallDialogue] = function(index,cfg)
    local class = require("Logic/BookReading/Component/Dialogue/PhoneCallDialogueComponent")
    local obj = class.New(index,cfg)
		return obj
	end
  --endregion

    
--region 场景切换
  components[logic.DialogType.ChangeSceneByBlack] = function(index,cfg)
    local class = require("Logic/BookReading/Component/ChangeScene/ChangeSceneByBlackComponent")
    local obj = class.New(index,cfg)
		return obj
  end
  components[logic.DialogType.ChangeSceneToBlack] = function(index,cfg)
    local class = require("Logic/BookReading/Component/ChangeScene/ChangeSceneToBlackComponent")
    local obj = class.New(index,cfg)
		return obj
	end
  components[logic.DialogType.ChangeSceneByWhite] = function(index,cfg)
    local class = require("Logic/BookReading/Component/ChangeScene/ChangeSceneByWhiteComponent")
    local obj = class.New(index,cfg)
		return obj
	end
  components[logic.DialogType.ChangeSceneByWave] = function(index,cfg)
    local class = require("Logic/BookReading/Component/ChangeScene/ChangeSceneByWaveComponent")
    local obj = class.New(index,cfg)
		return obj
	end
  components[logic.DialogType.ChangeSceneByShutter] = function(index,cfg)
    local class = require("Logic/BookReading/Component/ChangeScene/ChangeSceneByShutterComponent")
    local obj = class.New(index,cfg)
		return obj
	end
  components[logic.DialogType.ManualChangeScene] = function(index,cfg)
    local class = require("Logic/BookReading/Component/ChangeScene/ManualChangeSceneComponent")
    local obj = class.New(index,cfg)
		return obj
	end
  components[logic.DialogType.ClockChangeScene] = function(index,cfg)
    local class = require("Logic/BookReading/Component/ChangeScene/ClockChangeSceneComponent")
    local obj = class.New(index,cfg)
		return obj
  end
  components[logic.DialogType.SceneTap] = function(index,cfg)
    local class = require("Logic/BookReading/Component/ChangeScene/SceneTapComponent")
    local obj = class.New(index,cfg)
		return obj
	end
--endregion

--region 选择

  components[logic.DialogType.EnterName] = function(index,cfg)
    if cfg.trigger == 1 then
      local class = require("Logic/BookReading/Component/EnterNameComponent")
      local obj = class.New(index,cfg)
      obj.isNpc = (cfg.dialog_type == logic.DialogType.EnterNPCname)
      return obj
    elseif cfg.trigger == 2 then

      if logic.config.channel == Channel.Spain then
        local class = require("Logic/BookReading/Component/Choice/spain/ChoiceCharacter")
        local obj = class.New(index,cfg)
        obj.isNpc = (cfg.dialog_type == logic.DialogType.EnterNPCname)
        return obj
      else
        local class = require("Logic/BookReading/Component/Choice/onyx/ChoiceCharacter")
        local obj = class.New(index,cfg)
        obj.isNpc = (cfg.dialog_type == logic.DialogType.EnterNPCname)
        return obj
      end
    end
  end
  components[logic.DialogType.EnterNPCname] = components[logic.DialogType.EnterName]

  components[logic.DialogType.ChangeClothes] = function(index,cfg)
    if logic.config.channel == Channel.Spain then
      local class = require("Logic/BookReading/Component/Choice/spain/ChoiceClothes")
      local obj = class.New(index,cfg)
      return obj
    else
      local class = require("Logic/BookReading/Component/Choice/onyx/ChangeClothes")
      local obj = class.New(index,cfg)
      return obj
    end
  end
  components[logic.DialogType.ChangeWholeClothes] = function(index,cfg)
    if logic.config.channel == Channel.Spain then
      local class = require("Logic/BookReading/Component/Choice/spain/ChoiceWholeClothes")
      local obj = class.New(index,cfg)
      return obj
    else
      local class = require("Logic/BookReading/Component/Choice/onyx/ChoiceWholeClothes")
      local obj = class.New(index,cfg)
      return obj
    end
  end
  
  components[logic.DialogType.AutoSelectClothes] = function(index,cfg)
    local class = require("Logic/BookReading/Component/Choice/onyx/AutoSelectClothes")
    local obj = class.New(index,cfg)
		return obj
  end

  components[logic.DialogType.ChoiceSex] = function(index,cfg)
    local class = require("Logic/BookReading/Component/Choice/onyx/ChoiceSex")
    local obj = class.New(index,cfg)
		return obj
  end
--endregion
  components[logic.DialogType.SceneInteraction] = function(index,cfg)
    local class = require("Logic/BookReading/Component/SceneInteractionComponent")
    local obj = class.New(index,cfg)
		return obj
	end
    
  components[logic.DialogType.Puzzle] = function(index,cfg)
    local class = require("Logic/BookReading/Component/PuzzleComponent")
    local obj = class.New(index,cfg)
		return obj
	end
  components[logic.DialogType.StoryItems] = function(index,cfg)
    local class = require("Logic/BookReading/Component/StoryItemsComponent")
    local obj = class.New(index,cfg)
		return obj
  end


  --region 气泡聊天
  components[logic.DialogType.BubbleChat] = function(index,cfg)
    local class = require("Logic/BookReading/Component/BubbleChat/BubbleChatComponent")
    local obj = class.New(index,cfg)
		return obj
  end
  components[logic.DialogType.BubbleNarration] = components[logic.DialogType.BubbleChat]
  components[logic.DialogType.BubblePlayerDialog] = components[logic.DialogType.BubbleChat]
  components[logic.DialogType.BubbleOtherPlayerDialog] = components[logic.DialogType.BubbleChat]
  --endregion

  
  components[logic.DialogType.Splitview] = function(index,cfg)
    local class = require("Logic/BookReading/Component/SplitviewComponent")
    local obj = class.New(index,cfg)
		return obj
  end

  components[logic.DialogType.GotoChapter] = function(index,cfg)
    local class = require("Logic/BookReading/Component/GotoChapter")
    local obj = class.New(index,cfg)
		return obj
  end

  components[logic.DialogType.ChoiceSex] = function(index,cfg)
    local class = require("Logic/BookReading/Component/Choice/onyx/ChoiceSex")
    local obj = class.New(index,cfg)
		return obj
  end

  components[logic.DialogType.ChoiceModel] = function(index,cfg)
    local class = require("Logic/BookReading/Component/Choice/onyx/ChoiceModel")
    local obj = class.New(index,cfg)
		return obj
  end

  components[logic.DialogType.TitlePage] = function(index, cfg)
    local class = require("Logic/BookReading/Component/TitlePageComponent")
    local obj = class.New(index,cfg)
		return obj
  end

  components[logic.DialogType.AutoSelectClothesModel] = function(index, cfg)
    local class = require("Logic/BookReading/Component/Choice/onyx/AutoSelectClothesModel")
    local obj = class.New(index,cfg)
		return obj
  end
end

function ComponentFactory:__delete()
end

function ComponentFactory.Create(index,cfg)
  --cfg.dialog_type = logic.DialogType.ChoiceModel
  local func = components[cfg.dialog_type]
  if not func then
      logic.debug.LogError('未知对话类型:'..(cfg.dialog_type or 'null')..",请检查ID="..cfg.dialogID)
      return logic.BookReading.BaseComponent.New(index,cfg)
  end
	local obj = func(index,cfg)
	if not obj then
		logic.debug.LogError('未知对话类型:'..cfg.dialog_type.." other?")
		return logic.BookReading.BaseComponent.New(index,cfg)
	end
  return obj
end

return ComponentFactory.New()