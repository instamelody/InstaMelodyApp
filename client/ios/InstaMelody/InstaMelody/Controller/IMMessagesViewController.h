//
//  IMMessagesViewController.h
//  InstaMelody
//
//  Created by Ahmed Bakir on 9/26/15.
//  Copyright © 2015 InstaMelody. All rights reserved.
//

#import <JSQMessagesViewController/JSQMessagesViewController.h>
#import "UIFont+FontAwesome.h"
#import "NSString+FontAwesome.h"
#import "FAImageView.h"

@interface IMMessagesViewController : JSQMessagesViewController

@property IBOutlet UIView *playerView;
@property IBOutlet UIButton *playButton;
@property IBOutlet UIButton *micButton;
@property IBOutlet UIButton *loopTitleButton;
@property IBOutlet UIButton *statusButton;

@end
