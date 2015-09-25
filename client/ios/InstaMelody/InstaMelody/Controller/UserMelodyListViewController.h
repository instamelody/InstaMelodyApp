//
//  MelodyListViewController.h
//  
//
//  Created by Ahmed Bakir on 9/18/15.
//
//

#import <UIKit/UIKit.h>

@interface UserMelodyListViewController : UIViewController

@property (nonatomic, strong) IBOutlet UISegmentedControl *filterControl;

@property (nonatomic, strong) IBOutlet UITableView *tableView;

@end
