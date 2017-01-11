//
//  MelodyPickerController.h
//  InstaMelody
//
//  Created by Ahmed Bakir on 9/26/15.
//  Copyright © 2015 InstaMelody. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "Melody.h"
#import "LoopViewController.h"

@interface MelodyPickerController : UIViewController

@property (nonatomic, strong) NSArray *melodyList;

@property (nonatomic, strong) IBOutlet UITableView *tableView;

@property (nonatomic, strong) NSNumber *groupId;

@property (weak) LoopViewController * delegate;

-(IBAction)done:(id)sender;

@end
