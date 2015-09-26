//
//  VolumeViewController.m
//  InstaMelody
//
//  Created by Ahmed Bakir on 9/26/15.
//  Copyright © 2015 InstaMelody. All rights reserved.
//

#import "VolumeViewController.h"

@interface VolumeViewController ()

@end

@implementation VolumeViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.
    
    self.backgroundView.layer.cornerRadius = 8.0f;
    self.backgroundView.layer.masksToBounds = YES;
    
    UIFont *faFont = [UIFont fontAwesomeFontOfSize:24.0f];
    
    self.micLabel.font = faFont;
    self.melodyLabel.font = faFont;
    self.micVolDownLabel.font = faFont;
    self.micVolUpLabel.font = faFont;
    self.melodyVolDownLabel.font = faFont;
    self.melodyVolUpLabel.font = faFont;
    
    
    self.micLabel.text = [NSString fontAwesomeIconStringForEnum:FAMicrophone];
    self.melodyLabel.text = [NSString fontAwesomeIconStringForEnum:FAMusic];
    
    self.micVolDownLabel.text = [NSString fontAwesomeIconStringForEnum:FAVolumeDown];
    self.melodyVolDownLabel.text = [NSString fontAwesomeIconStringForEnum:FAVolumeDown];
    
    self.micVolUpLabel.text = [NSString fontAwesomeIconStringForEnum:FAVolumeUp];
    self.melodyVolUpLabel.text = [NSString fontAwesomeIconStringForEnum:FAVolumeUp];
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    if ([defaults objectForKey:@"micVolume"] != nil) {
        NSNumber *micVolume = [defaults objectForKey:@"micVolume"];
        NSNumber *melodyVolume = [defaults objectForKey:@"melodyVolume"];
        [self.micSlider setValue:micVolume.floatValue];
        [self.melodySlider setValue:melodyVolume.floatValue];
    }
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

/*
#pragma mark - Navigation

// In a storyboard-based application, you will often want to do a little preparation before navigation
- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
    // Get the new view controller using [segue destinationViewController].
    // Pass the selected object to the new view controller.
}
*/


-(IBAction)done:(id)sender {
    //save new settings to defaults'
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    [defaults setObject:[NSNumber numberWithFloat:self.micSlider.value] forKey:@"micVolume"];
    [defaults setObject:[NSNumber numberWithFloat:self.melodySlider.value] forKey:@"melodyVolume"];
    [defaults synchronize];
    
    [self dismissViewControllerAnimated:YES completion:nil];
}

@end
