//
//  ImagePicker.h
//  Unity-iPhone
//
//  Created by Wili on 2018/1/6.
//

#import <UIKit/UIKit.h>

@interface ImagePicker : NSObject<UIImagePickerControllerDelegate, UINavigationControllerDelegate>

// UnityGLViewController keeps this instance.
@property(nonatomic) UIImagePickerController* pickerController;

@property(nonatomic) NSString *outputFileName;

@property(nonatomic) BOOL isNeedEdit;
+ (instancetype)sharedInstance;

//- (void)show:(NSString *)title outputFileName:(NSString *)name maxSize:(NSInteger)maxSize;
- (void)OpenCamera:(NSString *)name;
- (void)OpenGallery:(NSString *)name isEdit:(BOOL)isEdit;
/*
- (void)saveScreenshot:(NSString *)path;
 */
@end
