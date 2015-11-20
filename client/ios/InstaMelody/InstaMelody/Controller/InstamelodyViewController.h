//
//  InstamelodyViewController.h
//  InstaMelody
//
//  Created by Ahmed Bakir on 10/23/15.
//  Copyright © 2015 InstaMelody. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "AwesomeMenu.h"
#import "LoopViewController.h"
#import "NetworkManager.h"
#import "UIFont+FontAwesome.h"
#import "NSString+FontAwesome.h"
#import "constants.h"
#import "CreateChatViewController.h"

@interface InstamelodyViewController : UIViewController <AwesomeMenuDelegate>

@property NSDateFormatter *dateFormatter;

@end
