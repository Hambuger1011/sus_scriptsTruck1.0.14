//
//  IGGLog.h
//  IGGSDK
//
//  Created by Liao JunHui on 2017/7/25.
//  Copyright © 2017年 IGG. All rights reserved.
//

#import <Foundation/Foundation.h>

#define IGGLOG_BASIC(mold, frmt, ...) [IGGLog log:Basic module:mold format:(frmt), ##__VA_ARGS__]
#define IGGLOG_IMPORTANT(mold, frmt, ...) [IGGLog log:Important module:mold format:(frmt), ##__VA_ARGS__]
#define IGGLOG_CRUCIAL(mold, frmt, ...) [IGGLog log:Crucial module:mold format:(frmt), ##__VA_ARGS__]

/*!
 *  Log levels are used to filter out logs. Used together with flags.
 */
typedef NS_ENUM(NSUInteger, IGGLogLevel){
    Basic = 0,
    Important,
    Crucial
};


@interface IGGLog : NSObject

+ (void)log:(IGGLogLevel)level module:(NSString *)module format:(NSString *)format, ... NS_FORMAT_FUNCTION(3,4);

@end
