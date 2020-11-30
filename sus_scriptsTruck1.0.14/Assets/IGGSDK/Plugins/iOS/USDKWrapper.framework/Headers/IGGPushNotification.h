//
//  IGGPushNotification.h
//  IGGSDK
//
//  Created by Wayne <wei.zheng@igg.com> on 2/17/14.
//  Copyright (c) 2014 IGG. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

typedef NS_ENUM(NSUInteger, IGGFamily) {
    /// IGG 运营
    IGGFamilyIGG,
    /// FP 运营
    IGGFamilyFP
};

@interface IGGPushNotification : NSObject

@property (assign, nonatomic, readonly) IGGFamily family;

@property (retain, nonatomic, readonly) NSString *userAgent;

@property (retain, nonatomic, readonly) NSString *UIID;

/*!
 推送单例

 @return IGGPushNotification
 */
+ (instancetype)sharedInstance;

/// 初始化
/// @param family 运营地区
/// @param userAgent 自定义 user-agent
/// @param UIID 用户 UIID
- (void)initialization:(IGGFamily)family userAgent:(NSString *)userAgent UIID:(NSString *)UIID;

/// 登录成功后调用
/// @param IGGId IGG ID
/// @param gameId 游戏 ID
- (void)onLoggedIn:(NSString *)IGGId gameId:(NSString *)gameId;

- (void)sessionExpired;

/*!
 请求推送权限
 */
- (void)requestRemoteNotificationPermission;

/*!
 推送消息转发

 @param application appdelegate.m 同名方法的参数
 @param launchOptions appdelegate.m 同名方法的参数
 */
- (void)onApplication:(UIApplication *)application willFinishLaunchingWithOptions:(NSDictionary *)launchOptions;

/*!
 推送消息转发
 
 @param application appdelegate.m 同名方法的参数
 @param deviceToken appdelegate.m 同名方法的参数
 */
- (void)onApplication:(UIApplication *)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData *)deviceToken;

/*!
 推送消息转发
 
 @param application appdelegate.m 同名方法的参数
 @param userInfo appdelegate.m 同名方法的参数
 */
- (void)onApplication:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo;


@end
