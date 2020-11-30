//
//  IGGTSHybridAppearance.h
//  IGGTSHybrid
//
//  Created by iGG on 2020/8/3.
//  Copyright © 2020 IGG. All rights reserved.
//

@protocol IGGTSHybridAppearance

//设置返回按钮的图标路径
- (NSString *)getBackButtonIcon;

//设置返回游戏按钮的图标路径
- (NSString *)getExitButtonIcon;

//设置顶部 Titlebar 的背景颜色, 范例 0xffffff
- (int)getHeaderBackgroundColor;

@end
