#include "UnityAppController.h"
#import <AWSCore/AWSCore.h>
#import <AWSKinesis/AWSKinesis.h>

extern "C"
{
    const int GAME = 1;
    const int IGGSDK = 2;
    
    AWSKinesisRecorder* recorder1;
    AWSKinesisRecorder* recorder2;
    AWSKinesisRecorder* AWSKinessisWrapper_alloc(int type, const char* accessKeyId, const char* secretKey, const char*region, const char*streamName, const char*cacheDir)
    {
        NSString* accessKeyIdStr = [NSString stringWithUTF8String:accessKeyId];
        NSString* secretKeyStr = [NSString stringWithUTF8String:secretKey];
        NSString* regionStr = [NSString stringWithUTF8String:region];
        NSString* streamNameStr = [NSString stringWithUTF8String:streamName];
        NSString* cacheDirStr = [NSString stringWithUTF8String:cacheDir];
        NSLog(@"AWSKinessisWrapper iOS alloc: %@  %@ %@ %@ %d", accessKeyIdStr, secretKeyStr, regionStr, streamNameStr, type);
        
        if (type == GAME) {
            if (nil == recorder1 ) {
                // 根据 appconf 的配置，初始化 Provider
                AWSStaticCredentialsProvider *provider = [[AWSStaticCredentialsProvider alloc] initWithAccessKey:accessKeyIdStr secretKey:secretKeyStr];
                AWSServiceConfiguration *gameConfiguration = [[AWSServiceConfiguration alloc] initWithRegion:[[NSString stringWithFormat:@"%@",regionStr] aws_regionTypeValue] credentialsProvider:provider];
                [AWSKinesisRecorder registerKinesisRecorderWithConfiguration:gameConfiguration forKey:@"GameRecord"];
                recorder1 = [AWSKinesisRecorder KinesisRecorderForKey:@"GameRecord"];
                
            }
            
            return recorder1;
        }
        
        if (type == IGGSDK) {
            if (nil == recorder2) {
                AWSStaticCredentialsProvider *provider = [[AWSStaticCredentialsProvider alloc] initWithAccessKey:accessKeyIdStr secretKey:secretKeyStr];
                AWSServiceConfiguration *gameConfiguration = [[AWSServiceConfiguration alloc] initWithRegion:[[NSString stringWithFormat:@"%@",regionStr] aws_regionTypeValue] credentialsProvider:provider];
                [AWSKinesisRecorder registerKinesisRecorderWithConfiguration:gameConfiguration forKey:@"IGGSDKRecord"];
                recorder2 = [AWSKinesisRecorder KinesisRecorderForKey:@"IGGSDKRecord"];
                [AWSKinesisRecorder KinesisRecorderForKey:@"IGGSDKRecord"];
            }
            
            return recorder2;
        }
        
        return nil;
    }
    
    void AWSKinessisWrapper_Flush(AWSKinesisRecorder* recorder)
    {
        [recorder submitAllRecords];
    }
    
    void AWSKinessisWrapper_PurgeBuffer(AWSKinesisRecorder* recorder)
    {
        [recorder removeAllRecords];
    }
    
    int AWSKinessisWrapper_diskBytesUsed(AWSKinesisRecorder* recorder)
    {
        return [recorder diskBytesUsed];
    }
    
    void AWSKinessisWrapper_Write(AWSKinesisRecorder* recorder, const char* data, const char* streamName)
    {
        NSString* dataStr = [NSString stringWithUTF8String:data];
        NSString* streamNameStr = [NSString stringWithUTF8String:streamName];
        NSLog(@"AWSKinessisWrapper iOS write：%@  %@", dataStr, streamNameStr);
        if (dataStr && streamNameStr) {
            [recorder saveRecord:[dataStr dataUsingEncoding:NSUTF8StringEncoding] streamName:streamNameStr];
        }
    }
}
