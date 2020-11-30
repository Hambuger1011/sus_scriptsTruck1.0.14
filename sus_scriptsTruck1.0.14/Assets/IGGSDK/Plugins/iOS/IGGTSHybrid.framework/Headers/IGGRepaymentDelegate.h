//
//  IGGRepaymentDelegate.h
//  IGGSDK
//
//  Created by iGG on 2019/12/26.
//  Copyright © 2019 IGG. All rights reserved.
//

#import <Foundation/Foundation.h>

#import "IGGError.h"
#import "IGGRepaymentItem.h"

NS_ASSUME_NONNULL_BEGIN

@protocol IGGRepaymentDelegate <NSObject>

/// 购买失败，包括: 120401:未找到商品，无法发起购买 120402:其它异常，无法获取到 receipt
- (void)repaymentFailed:(IGGError *)error item:(IGGRepaymentItem *)item;
/// 玩家支付失败
- (void)transactionPurchaseFailed:(IGGError *)error item:(IGGRepaymentItem *)item;
/// 玩家正在支付
- (void)transactionPurchasePurchasing:(IGGRepaymentItem *)item;
/// 玩家已完成支付
- (void)transactionPurchasePurchased:(IGGRepaymentItem *)item;
/// 交易已完成支付，已通知网关
- (void)transactionPurchaseFinished:(IGGRepaymentItem *)item;
/// 玩家已支付，通知网关失败
- (void)gatewayFailedListener:(IGGError *)error item:(IGGRepaymentItem *)item;

@end

NS_ASSUME_NONNULL_END
