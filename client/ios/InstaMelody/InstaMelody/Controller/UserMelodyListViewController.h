//
//  MelodyListViewController.h
//  
//
//  Created by Ahmed Bakir on 9/18/15.
//
//

#import <UIKit/UIKit.h>
#import "LoopViewController.h"
#import "NetworkManager.h"

@interface UserMelodyListViewController : UIViewController <LoopDelegate>

@property (nonatomic, strong) IBOutlet UISegmentedControl *filterControl;

@property (nonatomic, strong) IBOutlet UITableView *tableView;

-(IBAction)change:(id)sender;

@end
