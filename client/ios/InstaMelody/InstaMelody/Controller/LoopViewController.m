//
//  LoopViewController.m
//  InstaMelody
//
//  Created by Ahmed Bakir on 8/17/15.
//  Copyright (c) 2015 InstaMelody. All rights reserved.
//

#import "LoopViewController.h"
#import "NSString+FontAwesome.h"
#import "UIFont+FontAwesome.h"

@implementation LoopViewController

-(void)viewDidLoad {
    [super viewDidLoad];
    
    [self roundView:self.profileImageView];
    [self roundView:self.centerConsoleView];
    [self roundView:self.consoleView];
    
    [self applyFontAwesome];
    
}

-(void)roundView:(UIView *)view {
    view.layer.cornerRadius = view.frame.size.height / 2;
    view.layer.masksToBounds = YES;
}

-(void)applyFontAwesome {
    self.playButton.titleLabel.font = [UIFont fontAwesomeFontOfSize:self.playButton.titleLabel.frame.size.width - 10];
    
    self.shareButton.titleLabel.font = [UIFont fontAwesomeFontOfSize:35.0f];
    self.playLoopButton.titleLabel.font = [UIFont fontAwesomeFontOfSize:25.0f];
    
    [self.playButton setTitle:[NSString fontAwesomeIconStringForEnum:FAIconPlay] forState:UIControlStateNormal];
    [self.playLoopButton setTitle:[NSString fontAwesomeIconStringForEnum:FAIconRefresh] forState:UIControlStateNormal];
    [self.shareButton setTitle:[NSString fontAwesomeIconStringForEnum:FAIconShareAlt] forState:UIControlStateNormal];
    
    [self.volumeBarButtonItem setTitleTextAttributes:@{
                                         NSFontAttributeName: [UIFont fontAwesomeFontOfSize:20.0f]
                                         } forState:UIControlStateNormal];
    [self.volumeBarButtonItem setTitle:[NSString fontAwesomeIconStringForEnum:FAIconVolumeUp]];
    
    
}

-(IBAction)chooseLoop:(id)sender {
    
}

-(IBAction)share:(id)sender {
    
}

-(IBAction)showVolumeSettings:(id)sender {
    
}

-(IBAction)playLoop:(id)sender {
    
}

-(IBAction)toggleRecording:(id)sender {
    
}

-(IBAction)togglePlayback:(id)sender {
    
}

@end
