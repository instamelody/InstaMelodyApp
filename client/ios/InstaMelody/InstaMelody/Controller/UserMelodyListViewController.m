//
//  MelodyListViewController.m
//  
//
//  Created by Ahmed Bakir on 9/18/15.
//
//

#import "UserMelodyListViewController.h"
#import "constants.h"
#import "AFHTTPRequestOperationManager.h"
#import "UIFont+FontAwesome.h"
#import "NSString+FontAwesome.h"
#import "DataManager.h"
#import "LoopViewController.h"

@interface UserMelodyListViewController ()

@property (nonatomic, strong) NSArray *melodyList;
@property NSArray *filteredList;

@end

@implementation UserMelodyListViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.
    
    [self refreshUserMelodyList];
    
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

#pragma mark - Table view data source

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView {
    // Return the number of sections.
    return 1;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    // Return the number of rows in the section.
    
    return self.filteredList.count;
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    
    UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:@"MelodyCell" forIndexPath:indexPath];
    
    UserMelody *userMelody = (UserMelody *)[self.filteredList objectAtIndex:indexPath.row];
    
    cell.textLabel.text = userMelody.userMelodyName;
    cell.detailTextLabel.text = [NSString stringWithFormat:@"%ld Parts", userMelody.parts.count];
    cell.backgroundColor = [UIColor clearColor];

    return cell;
}

// In a storyboard-based application, you will often want to do a little preparation before navigation
- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
    // Get the new view controller using [segue destinationViewController].
    // Pass the selected object to the new view controller.
    
    UITableViewCell *selectedCell = (UITableViewCell *)sender;
    
    NSIndexPath *indexPath = [self.tableView indexPathForCell:selectedCell];
    
    UserMelody *melody = [self.melodyList objectAtIndex:indexPath.row];
    
    LoopViewController *loopVC = (LoopViewController *)segue.destinationViewController;
    loopVC.selectedUserMelody = melody;
    
    
}


#pragma mark - web actions

-(void)refreshUserMelodyList {
    [self getUserMelodyList];
}

-(void)getUserMelodyList {
    //
    
    //add observer for fetching friend list asynchronously
    
    //get friends from core data
    self.melodyList = [[DataManager sharedManager] userMelodyList];
    
    self.filteredList = self.melodyList;
    
}

-(IBAction)change:(id)sender {
    UISegmentedControl *control = (UISegmentedControl *)sender;
    switch (control.selectedSegmentIndex) {
        case 0: {
            
            self.filteredList = self.melodyList;
            
            break;
        }
        case 1: {
            
            NSMutableArray *tempArray = [NSMutableArray new];
            for (UserMelody *melody in self.melodyList) {
                if (melody.parts.count > 1) {
                    [tempArray addObject:melody];
                }
            }
            self.filteredList = tempArray;
            break;
        }
        case 2: {
            
            NSMutableArray *tempArray = [NSMutableArray new];
            for (UserMelody *melody in self.melodyList) {
                if (melody.parts.count == 1) {
                    [tempArray addObject:melody];
                }
            }
            self.filteredList = tempArray;
            break;
            
            break;
        }
            
        default:
            break;
    }
    
    [self.tableView reloadData];
}

@end
