//
//  ImageController.m
//  Unity-iPhone
//
//  Created by Wili on 2018/1/6.
//

#import <Foundation/Foundation.h>
#import "ImagePicker.h"
#import "ImageSave.h"

extern "C" {
    
    void LoadGallery()
    {
        ImagePicker *picker = [ImagePicker sharedInstance];
        [picker OpenGallery:[NSString stringWithUTF8String:"output"] isEdit:(BOOL)false ];
    }
    
    void LoadCamera()
    {
        ImagePicker *picker = [ImagePicker sharedInstance];
        [picker OpenCamera:[NSString stringWithUTF8String:"output"]];
    }
    
    void SaveImageToAlbum(const char* path) {
        [ImageSave saveScreenshot:[NSString stringWithUTF8String:path]];
    }
    
}
