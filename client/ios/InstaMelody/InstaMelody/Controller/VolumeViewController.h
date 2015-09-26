//
//  VolumeViewController.h
//  InstaMelody
//
//  Created by Ahmed Bakir on 9/26/15.
//  Copyright © 2015 InstaMelody. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "UIFont+FontAwesome.h"
#import "NSString+FontAwesome.h"
#import "FAImageView.h"

@interface VolumeViewController : UIViewController

@property IBOutlet UIView *backgroundView;

@property IBOutlet UIButton *doneButton;

@property IBOutlet UILabel *micLabel;
@property IBOutlet UILabel *melodyLabel;

@property IBOutlet UILabel *micVolDownLabel;
@property IBOutlet UILabel *micVolUpLabel;

@property IBOutlet UILabel *melodyVolDownLabel;
@property IBOutlet UILabel *melodyVolUpLabel;



@property IBOutlet UISlider *micSlider;
@property IBOutlet UISlider *melodySlider;

-(IBAction)done:(id)sender;

@end
