//
//  IGGTSHybird.h
//  IGGTSHybird
//
//  Created by iGG on 2019/10/11.
//  Copyright © 2019 IGG. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

#import "IGGTSHybridAppearance.h"
#import "IGGTSHybridDefinitions.h"

@class IGGTSHybrid;

//! Project version number for IGGTSHybird.
FOUNDATION_EXPORT NSString* IGGTSHybirdVersion;

// In this header, you should import all the public headers of your framework using statements like #import <IGGTSHybird/PublicHeader.h>

typedef void(^IGGTSHybirdUnreadCallback)(int unreadCount);

@protocol IGGTSHybridDelegate <NSObject>

/// GameId
- (NSString *)gameId;

- (void)getSSOTokenForWeb:(void (^)(IGGError *error, NSString *webSSOToken))onComplete;

@end

@interface IGGTSHybrid : NSObject

/// 必须实现该委托
@property (assign, nonatomic) id<IGGTSHybridDelegate> delegate;

/// 可选实现，用于自定义按钮颜色
@property (assign, nonatomic) id<IGGTSHybridAppearance> appearance;

/// 获取 TSHybrid instance
+ (instancetype)sharedInstance;

/// 设置未读消息数回调，会收到是否有客服收到消息玩家还未查看
- (void)setUnreadMessageCountCallback:(IGGTSHybirdUnreadCallback)callback;

- (void)showPanel:(NSDictionary *)userInfo
       onComplete:(void (^)(IGGError *error))onComplete;

/// 关闭客服界面
- (void)closePanel;

/// 客户端消息转发
- (BOOL)didReceiveRemoteNotifications:(NSDictionary *)userInfo;

- (void)registerRepayment:(id<IGGRepaymentProtocol>)repayment;

@end


