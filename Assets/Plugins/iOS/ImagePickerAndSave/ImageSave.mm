#import <Foundation/Foundation.h>
#import "ImageSave.h"

@implementation ImageSave

const char* SAVE_CALLBACK_OBJECT = "ImageSaveAndPick";
const char* SAVE_CALLBACK_METHOD = "OnSaveComplete";

+ (void)saveScreenshot:(NSString *)path {
	UIImage *image = [UIImage imageWithContentsOfFile:path];
    UIImageWriteToSavedPhotosAlbum(image, self,
	   @selector(image:finishedSavingWithError:contextInfo:),
	   (__bridge_retained void *) path);
   UnitySendMessage(SAVE_CALLBACK_OBJECT, SAVE_CALLBACK_METHOD, [path UTF8String]);
    
}


+ (void)image:(UIImage *)image finishedSavingWithError:(NSError *)error contextInfo:(void *)contextInfo {
    NSString* path = (__bridge_transfer NSString *)(contextInfo);
	[[NSFileManager defaultManager] removeItemAtPath:path error:nil];
}

@end

