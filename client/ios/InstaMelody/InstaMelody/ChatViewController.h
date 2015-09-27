//
//  ChatViewController.h
//  InstaMelody
//
//  Created by Ahmed Bakir on 8/7/15.
//  Copyright (c) 2015 InstaMelody. All rights reserved.
//

#import "JSQMessagesViewController.h"
#import "IMMessagesViewController.h"
#import "LoopViewController.h"
#import "NetworkManager.h"
#import "VolumeViewController.h"
#import <AVFoundation/AVFoundation.h>

@interface ChatViewController : IMMessagesViewController <UIActionSheetDelegate, UIImagePickerControllerDelegate, UINavigationControllerDelegate, LoopDelegate, UICollectionViewDataSource, UICollectionViewDelegate, AVAudioPlayerDelegate>

@property (nonatomic, strong) NSDictionary *chatDict;
@property (nonatomic, strong) NSDictionary *loopDict;

@end
