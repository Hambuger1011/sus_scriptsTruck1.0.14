//
//  IGGRepaymentProtocol.h
//  IGGSDK
//
//  Created by iGG on 2019/12/25.
//  Copyright © 2019 IGG. All rights reserved.
//

#import <Foundation/Foundation.h>

#import "IGGError.h"
#import "IGGProduct.h"
#import "IGGRepaymentItem.h"

@protocol IGGRepaymentProtocol

/// 还款
- (void)repay:(IGGRepaymentItem *)item;
/// 查询本地化商品信息 120501: 查询失败  120502：有未消耗的订单
- (void)queryProducts:(void(^)(IGGError *error, NSArray<IGGProduct *> *products))onComplete;

@end

