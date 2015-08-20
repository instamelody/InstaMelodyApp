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

@interface LoopViewController ()

@property (nonatomic, strong) NSArray *groupArray;

@end

@implementation LoopViewController

-(void)viewDidLoad {
    [super viewDidLoad];
    
    self.groupArray = [[DataManager sharedManager] melodyGroupList];
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

@end
