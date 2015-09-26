//
//  IMMessagesViewController.m
//  InstaMelody
//
//  Created by Ahmed Bakir on 9/26/15.
//  Copyright © 2015 InstaMelody. All rights reserved.
//

#import "IMMessagesViewController.h"

@interface IMMessagesViewController ()

@end

@implementation IMMessagesViewController

#pragma mark - Class methods

+ (UINib *)nib
{
    return [UINib nibWithNibName:NSStringFromClass([IMMessagesViewController class])
                          bundle:[NSBundle bundleForClass:[IMMessagesViewController class]]];
}

+ (instancetype)messagesViewController
{
    return [[[self class] alloc] initWithNibName:NSStringFromClass([IMMessagesViewController class])
                                          bundle:[NSBundle bundleForClass:[IMMessagesViewController class]]];
}

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view from its nib.
    
    UIFont *faFont = [UIFont fontAwesomeFontOfSize:24.0f];
    
    self.playButton.titleLabel.font = faFont;
    self.micButton.titleLabel.font  = faFont;
    
    [self.playButton setTitle:[NSString fontAwesomeIconStringForEnum:FAPlayCircle] forState:UIControlStateNormal];
    [self.micButton setTitle:[NSString fontAwesomeIconStringForEnum:FAMicrophone] forState:UIControlStateNormal];
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

@end
