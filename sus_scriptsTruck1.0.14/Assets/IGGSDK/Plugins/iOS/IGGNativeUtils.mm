#include "UnityAppController.h"

extern "C"
{
    typedef void(*MsgBoxReturnListener)(bool bSure);
    void IGGNativeUtils_ShowToast(const char* message, const char* title, const char*ok, MsgBoxReturnListener listener)
    {
        UIAlertController *alert = [UIAlertController alertControllerWithTitle:[NSString stringWithUTF8String:title] message:[NSString stringWithUTF8String:message] preferredStyle:UIAlertControllerStyleAlert];
        [alert addAction:[UIAlertAction actionWithTitle:[NSString stringWithUTF8String:ok] style:UIAlertActionStyleDefault handler:^(UIAlertAction * _Nonnull action) {
            listener(true);
        }]];
        UnityAppController *controller = GetAppController();
        [controller.rootViewController presentViewController:alert animated:true completion:nil];
    }
    void IGGNativeUtils_OpenBrowser(const char* url)
    {
		[[UIApplication sharedApplication] openURL:[NSURL URLWithString:[NSString stringWithUTF8String:url]]];  
	}
}

