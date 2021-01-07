#import "OnyxSocialShare.h"
#import <Social/Social.h>
 
NSString* const STR_SPLITTER = @"|";
NSString* const STR_EOF = @"endofline";
NSString* const STR_ARRAY_SPLITTER = @"%%%";
 
@implementation OnyxSocialShare
 
static OnyxSocialShare* onyx_sharedInstance;
 
+ (id)sharedInstance {
    
    if (onyx_sharedInstance == nil)  {
        onyx_sharedInstance = [[self alloc] init];
    }
    
    return onyx_sharedInstance;
}

- (void)objc_copyTextToClipboard : (NSString*)text
{
     UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];
     pasteboard.string = text;
}
 
-(void) nativeShare:(NSString*)text  media:(NSString*)media {
    NSLog(@"==============>nativeShare");
    UIActivityViewController *controller;
    
    if(media.length != 0) {
        NSData* imageData = [[NSData alloc]initWithBase64EncodedString:media options:0];
        UIImage* image = [[UIImage alloc] initWithData:imageData];
        
        NSLog(@"==============>image added");
        if(text.length != 0) {
            NSLog(@"==============>text added");
            controller = [[UIActivityViewController alloc] initWithActivityItems:@[text, image] applicationActivities:nil];
        } else {
            NSLog(@"==============>no text");
            controller = [[UIActivityViewController alloc] initWithActivityItems:@[image] applicationActivities:nil];
        }
        
    } else {
        NSLog(@"==============>no image");
        controller = [[UIActivityViewController alloc] initWithActivityItems:@[text] applicationActivities:nil];
    }
    
    UIViewController* vc =  UnityGetGLViewController();
    
    
    NSArray* vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
    if ([[vComp objectAtIndex:0] intValue] >= 8) {
        UIPopoverPresentationController *presentationController = [controller popoverPresentationController];
        presentationController.sourceView = vc.view;
        presentationController.sourceRect = CGRectMake(vc.view.bounds.origin.x+vc.view.bounds.size.width/2,
                                                       vc.view.bounds.origin.y+vc.view.bounds.size.height,
                                                       0, 0);
        presentationController.permittedArrowDirections = 0;
    }
    
    [vc presentViewController:controller animated:YES completion:nil];
 
    controller.completionWithItemsHandler = ^(UIActivityType  _Nullable activityType, BOOL completed, NSArray * _Nullable returnedItems, NSError * _Nullable activityError) {
//        NSLog(@"activityType :%@", activityType);
        if (completed){
            UnitySendMessage("_GameMain", "OnNativeShareSuccess", [Onyx_DataConvertor NSStringToChar:activityType]);
//            NSLog(@"completed!");
        }else{
            if (activityType != nil){
                UnitySendMessage("_GameMain", "OnNativeShareCancel", [Onyx_DataConvertor NSStringToChar:activityType]);
            }else{
                UnitySendMessage("_GameMain", "OnNativeShareCancel", [Onyx_DataConvertor NSStringToChar:@""]);
            }
//            NSLog(@"cancel!");
        }
    };
}
 
@end
 
 
@implementation Onyx_DataConvertor
 
 
+(NSString*) charToNSString:(char*)value {
    if (value != NULL) {
        return [NSString stringWithUTF8String: value];
    } else {
        return [NSString stringWithUTF8String: ""];
    }
}
 
+(const char*)NSIntToChar:(NSInteger)value {
    NSString* tmp = [NSString stringWithFormat:@"%ld", (long)value];
    return [tmp UTF8String];
}
 
+ (const char*) NSStringToChar:(NSString*)value {
    return [value UTF8String];
}
 
+ (NSArray*)charToNSArray:(char*)value {
    NSString* strValue = [Onyx_DataConvertor charToNSString:value];
    
    NSArray* array;
    if([strValue length] == 0) {
        array = [[NSArray alloc] init];
    } else {
        array = [strValue componentsSeparatedByString:STR_ARRAY_SPLITTER];
    }
    
    return array;
}
 
+ (const char*) NSStringsArrayToChar:(NSArray*) array {
    return [Onyx_DataConvertor NSStringToChar:[Onyx_DataConvertor serializeNSStringsArray:array]];
}
 
+ (NSString*) serializeNSStringsArray:(NSArray*) array {
    
    NSMutableString* data = [[NSMutableString alloc] init];
    
    
    for(NSString* str in array) {
        [data appendString:str];
        [data appendString: STR_ARRAY_SPLITTER];
    }
    
    [data appendString: STR_EOF];
    
    NSString* str = [data copy];
#if UNITY_VERSION < 500
    [str autorelease];
#endif
    
    return str;
}
+ (NSMutableString*)serializeErrorToNSString:(NSError*)error {
    NSString* description = @"";
    if(error.description != nil) {
        description = error.description;
    }
    
    return  [self serializeErrorWithDataToNSString:description code: (int) error.code];
}
+ (NSMutableString*)serializeErrorWithDataToNSString:(NSString*)description code:(int)code {
    NSMutableString* data = [[NSMutableString alloc] init];
    
    [data appendFormat:@"%i", code];
    [data appendString: STR_SPLITTER];
    [data appendString: description];
    
    return  data;
}
+ (const char*) serializeErrorWithData:(NSString*)description code: (int) code {
    NSString* str = [Onyx_DataConvertor serializeErrorWithDataToNSString:description code:code];
    return [Onyx_DataConvertor NSStringToChar:str];
}
+ (const char*) serializeError:(NSError*)error  {
    NSString* str = [Onyx_DataConvertor serializeErrorToNSString:error];
    return [Onyx_DataConvertor NSStringToChar:str];
}
@end
extern "C" {
    void Onyx_NativeShare(char* text, char* encodedMedia) {
        NSString* status = [Onyx_DataConvertor charToNSString:text];
        NSString* media = [Onyx_DataConvertor charToNSString:encodedMedia];
        
        [[OnyxSocialShare sharedInstance] nativeShare:status media:media];
    }

 
     void copyTextToClipboard(const char *textList)
    {  
        NSString *text = [NSString stringWithUTF8String: textList] ;
 
        [[OnyxSocialShare sharedInstance] objc_copyTextToClipboard: text];
    }
}
