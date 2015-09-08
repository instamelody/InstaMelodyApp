//
//  ChatViewController.h
//  InstaMelody
//
//  Created by Ahmed Bakir on 8/7/15.
//  Copyright (c) 2015 InstaMelody. All rights reserved.
//

#import "JSQMessagesViewController.h"
#import "LoopViewController.h"

@interface ChatViewController : JSQMessagesViewController <UIActionSheetDelegate, UIImagePickerControllerDelegate, UINavigationControllerDelegate, LoopDelegate>

@property (nonatomic, strong) NSDictionary *chatDict;

@end
