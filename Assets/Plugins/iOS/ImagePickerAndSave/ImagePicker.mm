//
//  ImagePicker.mm
//  Unity-iPhone
//
//  Created by Wili on 2018/1/6.
//

#import "ImagePicker.h"

#pragma mark Config

const char* CALLBACK_OBJECT = "ImageSaveAndPick";
const char* CALLBACK_METHOD = "OnPickComplete";
const char* CALLBACK_SAVE_METHOD = "OnSaveComplete";
const char* CALLBACK_METHOD_FAILURE = "OnFailure";

const char* MESSAGE_FAILED_PICK = "Failed to pick the image";
const char* MESSAGE_FAILED_FIND = "Failed to find the image";
const char* MESSAGE_FAILED_COPY = "Failed to copy the image";

#pragma mark Picker

@implementation ImagePicker

+ (instancetype)sharedInstance {
    static ImagePicker *instance;
    static dispatch_once_t token;
    dispatch_once(&token, ^{
        instance = [[ImagePicker alloc] init];
    });
    return instance;
}


- (void)OpenCamera:(NSString *)name {
    if (self.pickerController != nil) {
           UnitySendMessage(CALLBACK_OBJECT, CALLBACK_METHOD_FAILURE, MESSAGE_FAILED_PICK);
        return;
    }
    
    self.pickerController = [[UIImagePickerController alloc] init];
    self.pickerController.delegate = self;
    
   // self.pickerController.allowsEditing = YES;
   // self.pickerController.allowsImageEditing = YES;
    
    self.pickerController.sourceType = UIImagePickerControllerSourceTypeCamera;
    
    UIViewController *unityController = UnityGetGLViewController();
    [unityController presentViewController:self.pickerController animated:YES completion:^{
        self.outputFileName =  [[ ImagePicker getCurrentTimes] stringByAppendingString:@"_output"];
    }];
}

- (void)OpenGallery:(NSString *)name isEdit:(BOOL)isEdit
{
    if (self.pickerController != nil) {
        UnitySendMessage(CALLBACK_OBJECT, CALLBACK_METHOD_FAILURE, MESSAGE_FAILED_PICK);
        return;
    }
    
    self.pickerController = [[UIImagePickerController alloc] init];
    self.pickerController.delegate = self;
    self.isNeedEdit = isEdit;
    if(isEdit)
    {
        self.pickerController.allowsEditing = YES;
    }
    
    
    self.pickerController.sourceType = UIImagePickerControllerSourceTypePhotoLibrary;
    
    UIViewController *unityController = UnityGetGLViewController();
    [unityController presentViewController:self.pickerController animated:YES completion:^{
        self.outputFileName =  [[ ImagePicker getCurrentTimes] stringByAppendingString:@"_output"];
        
    }];

}
#pragma mark UIImagePickerControllerDelegate

- (void)imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary<NSString *,id> *)info {
    
    UIImage *image;
    
    if(self.isNeedEdit)
    {
        image = info[ UIImagePickerControllerEditedImage ];
    }
    else
    {
        image = info[UIImagePickerControllerOriginalImage];
    }
    
    if (image == nil) {
        UnitySendMessage(CALLBACK_OBJECT, CALLBACK_METHOD_FAILURE, MESSAGE_FAILED_FIND);
        [self dismissPicker];
        return;
    }
    
    image = [self resizeImage:image toMaxWidthAndHeight:1024] ;
    image = [self fixOrientation:image];
    
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    if (paths.count == 0) {
        UnitySendMessage(CALLBACK_OBJECT, CALLBACK_METHOD_FAILURE, MESSAGE_FAILED_COPY);
        [self dismissPicker];
        return;
    }
    
    NSString* imageName= self.outputFileName;
    if ([imageName hasSuffix:@".png"] == NO) {
        imageName = [imageName stringByAppendingString:@".png"];
    }
    
    NSString *imageSavePath = [(NSString *)[paths objectAtIndex:0] stringByAppendingPathComponent:imageName];
    NSData *png = UIImagePNGRepresentation(image);
    
    if (png == nil) {
        UnitySendMessage(CALLBACK_OBJECT, CALLBACK_METHOD_FAILURE, MESSAGE_FAILED_COPY);
        [self dismissPicker];
        return;
    }
    
    BOOL success = [png writeToFile:imageSavePath atomically:YES];
    if (success == NO) {
        UnitySendMessage(CALLBACK_OBJECT, CALLBACK_METHOD_FAILURE, MESSAGE_FAILED_COPY);
        [self dismissPicker];
        return;
    }
    NSLog(imageSavePath);
    
    UnitySendMessage(CALLBACK_OBJECT, CALLBACK_METHOD, [imageSavePath UTF8String]);
    
    [self dismissPicker];
}

- (void)imagePickerControllerDidCancel:(UIImagePickerController *)picker {
    UnitySendMessage(CALLBACK_OBJECT, CALLBACK_METHOD_FAILURE, MESSAGE_FAILED_PICK);
    
    [self dismissPicker];
}


- (void)dismissPicker
{
    self.outputFileName = nil;
    
    if (self.pickerController != nil) {
        [self.pickerController dismissViewControllerAnimated:YES completion:^{
            self.pickerController = nil;
        }];
    }
    
}




- (UIImage *)resizeImage:(UIImage *)sourceImage toMaxWidthAndHeight:(CGFloat)maxValue {
    CGSize imageSize = sourceImage.size;
    
    CGFloat width = imageSize.width;
    CGFloat height = imageSize.height;
    
    if (width > height && width > maxValue) {
        height = height * (maxValue / width);
        width = maxValue;
    }else if (height > width && height > maxValue) {
        width = width * (maxValue / height);
        height = maxValue;
    }else {
        return sourceImage;
    }
    
    UIGraphicsBeginImageContext(CGSizeMake(width, height));
    [sourceImage drawInRect:CGRectMake(0, 0, width, height)];
    
    UIImage *newImage = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    
    return newImage;
}





- (UIImage *)fixOrientation:(UIImage *)img {
    // No-op if the orientation is already correct
    if (img.imageOrientation == UIImageOrientationUp) return img;
    
    // We need to calculate the proper transformation to make the image upright.
    // We do it in 2 steps: Rotate if Left/Right/Down, and then flip if Mirrored.
    CGAffineTransform transform = CGAffineTransformIdentity;
    
    switch (img.imageOrientation) {
        case UIImageOrientationDown:
        case UIImageOrientationDownMirrored:
            transform = CGAffineTransformTranslate(transform, img.size.width, img.size.height);
            transform = CGAffineTransformRotate(transform, M_PI);
            break;
            
        case UIImageOrientationLeft:
        case UIImageOrientationLeftMirrored:
            transform = CGAffineTransformTranslate(transform, img.size.width, 0);
            transform = CGAffineTransformRotate(transform, M_PI_2);
            break;
            
        case UIImageOrientationRight:
        case UIImageOrientationRightMirrored:
            transform = CGAffineTransformTranslate(transform, 0, img.size.height);
            transform = CGAffineTransformRotate(transform, -M_PI_2);
            break;
        case UIImageOrientationUp:
        case UIImageOrientationUpMirrored:
            break;
    }
    
    switch (img.imageOrientation) {
        case UIImageOrientationUpMirrored:
        case UIImageOrientationDownMirrored:
            transform = CGAffineTransformTranslate(transform, img.size.width, 0);
            transform = CGAffineTransformScale(transform, -1, 1);
            break;
            
        case UIImageOrientationLeftMirrored:
        case UIImageOrientationRightMirrored:
            transform = CGAffineTransformTranslate(transform, img.size.height, 0);
            transform = CGAffineTransformScale(transform, -1, 1);
            break;
        case UIImageOrientationUp:
        case UIImageOrientationDown:
        case UIImageOrientationLeft:
        case UIImageOrientationRight:
            break;
    }
    
    // Now we draw the underlying CGImage into a new context, applying the transform
    // calculated above.
    CGContextRef ctx = CGBitmapContextCreate(NULL, img.size.width, img.size.height,
                                             CGImageGetBitsPerComponent(img.CGImage), 0,
                                             CGImageGetColorSpace(img.CGImage),
                                             CGImageGetBitmapInfo(img.CGImage));
    CGContextConcatCTM(ctx, transform);
    switch (img.imageOrientation) {
        case UIImageOrientationLeft:
        case UIImageOrientationLeftMirrored:
        case UIImageOrientationRight:
        case UIImageOrientationRightMirrored:
            // Grr...
            CGContextDrawImage(ctx, CGRectMake(0,0,img.size.height,img.size.width), img.CGImage);
            break;
            
        default:
            CGContextDrawImage(ctx, CGRectMake(0,0,img.size.width,img.size.height), img.CGImage);
            break;
    }
    
    // And now we just create a new UIImage from the drawing context
    CGImageRef cgimg = CGBitmapContextCreateImage(ctx);
    UIImage *img1 = [UIImage imageWithCGImage:cgimg];
    CGContextRelease(ctx);
    CGImageRelease(cgimg);
    return img1;
    
}


+(NSString*)getCurrentTimes{
    
    NSDateFormatter *formatter = [[NSDateFormatter alloc] init];
    
    [formatter setDateFormat:@"YYYY_MM_dd_HH_mm_ss"];
    
    NSDate *datenow = [NSDate date];
    
    NSString *currentTimeString = [formatter stringFromDate:datenow];
    
    NSLog(@"currentTimeString =  %@",currentTimeString);
    
    return currentTimeString;
    
}






@end



