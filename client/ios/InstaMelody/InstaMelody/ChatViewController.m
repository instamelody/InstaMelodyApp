//
//  ChatViewController.m
//  InstaMelody
//
//  Created by Ahmed Bakir on 8/7/15.
//  Copyright (c) 2015 InstaMelody. All rights reserved.
//

#import "ChatViewController.h"

@implementation ChatViewController

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    self.title = [self.chatDict objectForKey:@"Id"];
    
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    
    /**
     *  You MUST set your senderId and display name
     */
    self.senderId = [defaults objectForKey:@"Id"];
    self.senderDisplayName = [defaults objectForKey:@"DisplayName"];
    
    UIImageView *tempImageView = [[UIImageView alloc] initWithImage:[UIImage imageNamed:@"blurBlue"]];
    [tempImageView setFrame:self.collectionView.frame];
    
    self.collectionView.backgroundView = tempImageView;
    
}

@end
