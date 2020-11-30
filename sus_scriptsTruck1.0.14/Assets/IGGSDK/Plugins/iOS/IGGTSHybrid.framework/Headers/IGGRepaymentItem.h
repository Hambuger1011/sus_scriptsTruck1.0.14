//
//  IGGRepaymentItem.h
//  IGGSDK
//
//  Created by iGG on 2019/12/25.
//  Copyright © 2019 IGG. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface IGGRepaymentItem : NSObject

/// 商品 ID
@property (retain, nonatomic) NSString *productId;
/// 商品数量
@property (retain, nonatomic) NSNumber *quantity;
/// 拓展信息，用于透传
@property (retain, nonatomic) NSDictionary *extra;

@end

NS_ASSUME_NONNULL_END
