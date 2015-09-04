//
//  ChatViewController.h
//  InstaMelody
//
//  Created by Ahmed Bakir on 8/7/15.
//  Copyright (c) 2015 InstaMelody. All rights reserved.
//

#import "JSQMessagesViewController.h"

@interface ChatViewController : JSQMessagesViewController <UIActionSheetDelegate, UIImagePickerControllerDelegate, UINavigationControllerDelegate>

@property (nonatomic, strong) NSDictionary *chatDict;

@end
