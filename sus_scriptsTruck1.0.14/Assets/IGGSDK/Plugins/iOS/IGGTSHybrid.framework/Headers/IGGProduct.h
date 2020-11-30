//
//  IGGProduct.h
//  IGGSDK
//
//  Created by iGG on 2019/12/25.
//  Copyright © 2019 IGG. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>

NS_ASSUME_NONNULL_BEGIN

@interface IGGProduct : NSObject

/// 商品 ID
@property (retain, nonatomic) NSString *productId;
/// 商品价格
@property (retain, nonatomic) NSString *price;
/// 商品本地化货币符号
@property (retain, nonatomic) NSString *currencySymbol; /// 国家编码
/// 国家编码
@property (retain, nonatomic) NSString *currencyCode;
/// 元数据
@property (retain, nonatomic) SKProduct *product;

@end

NS_ASSUME_NONNULL_END
