//
//  IGGError.h
//  IGGSDK
//
//  Created by Liao JunHui on 2017/8/31.
//  Copyright © 2017年 IGG. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface IGGError : NSObject

//唯一错误码
@property (retain, nonatomic) NSString *uniqueCode;
//建议错误提示语，可能为空
@property (retain, nonatomic) NSString *suggestion;
//错误信息，可能为空
@property (retain, nonatomic) NSString *message;
//原始错误
@property (retain, nonatomic) NSError *underlyingError;

+ (IGGError *)errorWithCode:(NSString *)code suggestion:(NSString *)suggestion underlyingError:(NSError *)error;

@end
