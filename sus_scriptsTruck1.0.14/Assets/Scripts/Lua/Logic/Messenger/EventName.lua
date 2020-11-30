local EventName = {
    on_story_chapter_new = 'on_story_chapter_new',  --创建章节
    on_story_role_new = 'on_story_role_new',  --创建角色
    on_story_role_change = 'on_story_role_change',  --修改角色
    on_story_role_select = 'on_story_role_select',  --角色选择
    on_story_delete_book = 'on_story_delete_book',  --删除书本
    on_story_chapter_save_click = 'on_story_chapter_save_click',  --保存章节信息
    on_story_chapter_editor = 'on_story_chapter_editor',  --修改章节信息
    on_story_chapter_refresh = 'on_story_chapter_refresh', --刷新章节
    on_story_chapter_content_editor = 'on_story_chapter_content_editor',  --编辑章节对话

    on_story_editor_dialog_list_refresh = 'on_story_editor_dialog_list_refresh',--刷新对话列表
    on_story_editor_dialog_click = 'on_story_editor_dialog_click',--点击对话
    on_story_editor_dialog_delete = 'on_story_editor_dialog_delete',--删除对话
    on_story_editor_selection_add = 'on_story_editor_selection_add',--添加选项
    on_story_editor_pickimage = 'on_story_editor_pickimage',--从相册选图
    on_story_editor_image_add = 'on_story_editor_image_add',--添加图片

    on_story_editor_selection_click = 'on_story_editor_selection_click',--删除选项子item
    on_story_editor_selection_item_click = 'on_story_editor_selection_item_click',--点击选项item
    on_story_editor_selection_dialog = 'on_story_editor_selection_dialog',--编辑选项剧情
    on_story_editor_selection_delete = 'on_story_editor_selection_delete',--删除选项子item

    on_story_editor_dialog_modify = 'on_story_editor_dialog_modify',--编辑对话
    on_story_editor_dialog_insert = 'on_story_editor_dialog_insert',--插入对话
    on_story_editor_dialog_append = 'on_story_editor_dialog_append',--添加对话
    on_story_editor_dialog_append_option = 'on_story_editor_dialog_append_option',--对话插入选项
    on_story_editor_dialog_append_modify = 'on_story_editor_dialog_append_modify',
    on_story_editor_dialog_modify = 'on_story_editor_dialog_modify',

    on_story_preview_click = 'on_story_preview_click',
    on_story_preview_dialog_click = 'on_story_preview_dialog_click',
    on_story_preview_selection_click = 'on_story_preview_selection_click',
    on_story_preview_selection_item_click = 'on_story_preview_selection_item_click',
    on_story_preview_selection_choice = 'on_story_preview_selection_choice',
}
return EventName