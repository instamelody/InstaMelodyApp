//
//  LoopViewController.m
//  InstaMelody
//
//  Created by Ahmed Bakir on 8/17/15.
//  Copyright (c) 2015 InstaMelody. All rights reserved.
//

#import "LoopViewController.h"
#import "DataManager.h"
#import "MelodyGroup.h"
#import "NSString+FontAwesome.h"
#import "UIFont+FontAwesome.h"

@interface LoopViewController ()

@property (nonatomic, strong) NSArray *groupArray;

@end

@implementation LoopViewController

-(void)viewDidLoad {
    [super viewDidLoad];
    
    self.groupArray = [[DataManager sharedManager] melodyGroupList];
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
    UIAlertController *alert = [UIAlertController alertControllerWithTitle:@"Loops" message:@"Choose a loop" preferredStyle:UIAlertControllerStyleActionSheet];
    
    MelodyGroup *group = self.groupArray[0];
    
    if (group != nil) {
        
        for (Melody *melody in group.melodies) {
            UIAlertAction *action = [UIAlertAction actionWithTitle:melody.melodyName style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
                //make the button do something
                
            }];
            [alert addAction:action];
            
        }
        
        UIAlertAction *cancelAction = [UIAlertAction actionWithTitle:@"Cancel" style:UIAlertActionStyleDestructive handler:nil];
        [alert addAction:cancelAction];
    }
    
    [self presentViewController:alert animated:YES completion:nil];
}

-(IBAction)share:(id)sender {
    UIAlertController *alert = [UIAlertController alertControllerWithTitle:@"Share to" message:nil preferredStyle:UIAlertControllerStyleActionSheet];
    UIAlertAction *fbAction = [UIAlertAction actionWithTitle:@"Facebook" style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
        //sdf
    }];
    
    UIAlertAction *twAction = [UIAlertAction actionWithTitle:@"Twitter" style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
        //sdf
    }];
    
    UIAlertAction *scAction = [UIAlertAction actionWithTitle:@"SoundCloud" style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
        //sdf
    }];
    
    UIAlertAction *emailAction = [UIAlertAction actionWithTitle:@"E-mail" style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
        //sdf
    }];
    
    UIAlertAction *textAction = [UIAlertAction actionWithTitle:@"Text" style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
        //sdf
    }];
    
    UIAlertAction *cancelAction = [UIAlertAction actionWithTitle:@"Cancel" style:UIAlertActionStyleDestructive handler:nil];
    
    [alert addAction:fbAction];
    [alert addAction:twAction];
    [alert addAction:scAction];
    [alert addAction:emailAction];
    [alert addAction:textAction];
    [alert addAction:cancelAction];
    [self presentViewController:alert animated:YES completion:nil];
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
