//
//  IGGTSHybridDefinitions.h
//  IGGTSHybrid
//
//  Created by iGG on 2020/1/22.
//  Copyright © 2020 IGG. All rights reserved.
//

//#import <IGGSDKCore/IGGError.h>
//#import <IGGSDKPayment/IGGRepaymentProtocol.h>
//#import <IGGSDKPayment/IGGRepaymentProtocol.h>
//#import <IGGSDKPayment/IGGRepaymentDelegate.h>
//#import <IGGSDKPayment/IGGProduct.h>
//#import <IGGSDKPayment/IGGRepaymentItem.h>

#if __has_include(<IGGSDKCore/IGGError.h>)
#import <IGGSDKCore/IGGError.h>
#else
#import "IGGError.h"
#endif

#if __has_include(<IGGSDKPayment/IGGRepaymentProtocol.h>)
#import <IGGSDKPayment/IGGRepaymentProtocol.h>
#else
#import "IGGRepaymentProtocol.h"
#endif


#if __has_include(<IGGSDKPayment/IGGRepaymentDelegate.h>)
#import <IGGSDKPayment/IGGRepaymentDelegate.h>
#else
#import "IGGRepaymentDelegate.h"
#endif


#if __has_include(<IGGSDKPayment/IGGProduct.h>)
#import <IGGSDKPayment/IGGProduct.h>
#else
#import "IGGProduct.h"
#endif

#if __has_include(<IGGSDKPayment/IGGRepaymentItem.h>)
#import <IGGSDKPayment/IGGRepaymentItem.h>
#else
#import "IGGRepaymentItem.h"
#endif

