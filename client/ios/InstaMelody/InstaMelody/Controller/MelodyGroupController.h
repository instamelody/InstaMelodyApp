//
//  MelodyGroupController.h
//  InstaMelody
//
//  Created by Ahmed Bakir on 9/26/15.
//  Copyright © 2015 InstaMelody. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "MelodyGroup.h"
#import "DataManager.h"
#import "MelodyPickerController.h"

@interface MelodyGroupController : UIViewController

@property (nonatomic, strong) IBOutlet UITableView *tableView;

@property (nonatomic, strong) NSNumber *groupId;

@property (weak) id delegate;

-(IBAction)cancel:(id)sender;

@end
