//
//  MelodyListViewController.m
//  
//
//  Created by Ahmed Bakir on 9/18/15.
//
//

#import "UserMelodyListViewController.h"
#import "UserMelodyCell.h"
#import "constants.h"
#import "AFHTTPRequestOperationManager.h"
#import "UIFont+FontAwesome.h"
#import "NSString+FontAwesome.h"
#import "DataManager.h"

@interface UserMelodyListViewController ()

@property (nonatomic, strong) NSArray *melodyList;
//@property NSArray *filteredList;
@property NSDateFormatter *fromDateFormatter;
@property NSDateFormatter *toDateFormatter;
@property NSArray *loopArray;
@property NSArray *cleanLoopArray;

@end

@implementation UserMelodyListViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.
    
    //[self refreshUserMelodyList];
    [self fetchMyLoops];
    
    self.fromDateFormatter = [[NSDateFormatter alloc] init];
    
    NSLocale *enUSPOSIXLocale = [NSLocale localeWithLocaleIdentifier:@"en_US_POSIX"];
    
    [self.fromDateFormatter setLocale:enUSPOSIXLocale];
    
    [self.fromDateFormatter setDateFormat:@"yyyy-MM-dd'T'HH:mm:ss.SSS"];
    [self.fromDateFormatter setTimeZone:[NSTimeZone timeZoneForSecondsFromGMT:0]];
    
    self.toDateFormatter = [[NSDateFormatter alloc] init];
    [self.toDateFormatter setDateStyle:NSDateFormatterShortStyle];
    [self.toDateFormatter setTimeStyle:NSDateFormatterShortStyle];
    
    
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
    
    return self.loopArray.count;
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    
    UserMelodyCell *cell = (UserMelodyCell *)[tableView dequeueReusableCellWithIdentifier:@"MelodyCell" forIndexPath:indexPath];
    
    NSDictionary *loopDict = self.loopArray[indexPath.row];
    
    cell.titleLabel.text = [loopDict objectForKey:@"Name"];
    
    NSString *oldDateString = [loopDict objectForKey:@"DateCreated"];
    NSDate *dateObject = [self.fromDateFormatter dateFromString:oldDateString];
    
    cell.dateLabel.text = [self.toDateFormatter stringFromDate:dateObject];
    
    NSArray *parts = [loopDict objectForKey:@"Parts"];
    
    if (parts.count > 1) {
        cell.subtitleLabel.text = [NSString stringWithFormat:@"Social loop"];
    } else {
        cell.subtitleLabel.text = [NSString stringWithFormat:@"Solo loop"];
    }
    if ([[loopDict objectForKey:@"IsChatLoopPart"] boolValue] == TRUE && self.filterControl.selectedSegmentIndex == 3) {
        cell.subtitleLabel.text = [NSString stringWithFormat:@"Chat loop"];
    }
    
    /*
    UserMelody *userMelody = (UserMelody *)[self.loopArray objectAtIndex:indexPath.row];
    
    cell.titleLabel.text = userMelody.userMelodyName;
    if (userMelody.parts.count > 1) {
     
    } else {
     
    }
     if ([userMelody.isChatLoopPart boolValue] == TRUE && self.filterControl.selectedSegmentIndex == 3) {
     cell.subtitleLabel.text = [NSString stringWithFormat:@"Chat loop"];
     }
    */
    
    cell.backgroundColor = [UIColor clearColor];

    return cell;
}

// In a storyboard-based application, you will often want to do a little preparation before navigation
- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
    // Get the new view controller using [segue destinationViewController].
    // Pass the selected object to the new view controller.
    
    UITableViewCell *selectedCell = (UITableViewCell *)sender;
    
    NSIndexPath *indexPath = [self.tableView indexPathForCell:selectedCell];
    
    NSDictionary *loopDict = [self.loopArray objectAtIndex:indexPath.row];
    
    LoopViewController *loopVC = (LoopViewController *)segue.destinationViewController;
    //loopVC.selectedUserMelody = melody;
    loopVC.selectedLoop = loopDict;
    loopVC.delegate = self;
    
    
    /*
    UserMelody *melody = [self.melodyList objectAtIndex:indexPath.row];
    
    LoopViewController *loopVC = (LoopViewController *)segue.destinationViewController;
    loopVC.selectedUserMelody = melody;
     */
    
    
}


-(void)fetchMyLoops {
    
    NSString *requestUrl = [NSString stringWithFormat:@"%@/Melody/Loop", API_BASE_URL];
    
    NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    
    NSDictionary *parameters = @{@"token": token};
    
    [manager GET:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        // NSLog(@"JSON: %@", responseObject);
        NSLog(@"loop updated");
        
        NSArray *tempArray = (NSArray *)responseObject;
        
        //NSArray *tempArray = [[DataManager sharedManager] userMelodyList];
        
        if (tempArray.count > 20) {
            tempArray = [tempArray subarrayWithRange:NSMakeRange(0, 20)];
        }
        
        NSSortDescriptor *valueDescriptor = [[NSSortDescriptor alloc] initWithKey:@"DateCreated" ascending:NO];
        NSArray *descriptors = [NSArray arrayWithObject:valueDescriptor];
        self.loopArray = [tempArray sortedArrayUsingDescriptors:descriptors];
        
        self.cleanLoopArray = [tempArray sortedArrayUsingDescriptors:descriptors];
        
        [self.tableView reloadData];
        
        
        //NSDictionary *responseDict = (NSDictionary *)responseObject;
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        NSLog(@"Error fetching loops: %@", error);
        
    }];
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
    
    NSSortDescriptor *valueDescriptor = [[NSSortDescriptor alloc] initWithKey:@"dateCreated" ascending:NO];
    NSArray *descriptors = [NSArray arrayWithObject:valueDescriptor];
    //self.filteredList = [self.melodyList sortedArrayUsingDescriptors:descriptors];

    
}

-(IBAction)change:(id)sender {
    UISegmentedControl *control = (UISegmentedControl *)sender;
    
    self.loopArray = self.cleanLoopArray;
    
    switch (control.selectedSegmentIndex) {
        case 0: {
            
            /*
            NSSortDescriptor *valueDescriptor = [[NSSortDescriptor alloc] initWithKey:@"dateCreated" ascending:NO];
            NSArray *descriptors = [NSArray arrayWithObject:valueDescriptor];
            self.filteredList = [self.melodyList sortedArrayUsingDescriptors:descriptors];
             */
            NSArray *tempArray = self.loopArray;
            NSSortDescriptor *valueDescriptor = [[NSSortDescriptor alloc] initWithKey:@"DateCreated" ascending:NO];
            NSArray *descriptors = [NSArray arrayWithObject:valueDescriptor];
            self.loopArray = [tempArray sortedArrayUsingDescriptors:descriptors];
            

            
            break;
        }
        case 1: {
            
            /*
            NSMutableArray *tempArray = [NSMutableArray new];
            for (UserMelody *melody in self.melodyList) {
                if (melody.parts.count > 1) {
                    [tempArray addObject:melody];
                }
            }
            
            NSSortDescriptor *valueDescriptor = [[NSSortDescriptor alloc] initWithKey:@"dateCreated" ascending:NO];
            NSArray *descriptors = [NSArray arrayWithObject:valueDescriptor];
            self.filteredList = [tempArray sortedArrayUsingDescriptors:descriptors];
             */
            NSMutableArray *tempArray = [NSMutableArray new];
            
            for (NSDictionary *itemDict in self.loopArray) {
                NSArray *parts = [itemDict objectForKey:@"Parts"];
                if (parts.count > 1) {
                    [tempArray addObject:itemDict];
                }
            }
            
            NSSortDescriptor *valueDescriptor = [[NSSortDescriptor alloc] initWithKey:@"DateCreated" ascending:NO];
            NSArray *descriptors = [NSArray arrayWithObject:valueDescriptor];
            self.loopArray = [tempArray sortedArrayUsingDescriptors:descriptors];

            
            break;
        }
        case 2: {
            /*
            NSMutableArray *tempArray = [NSMutableArray new];
            for (UserMelody *melody in self.melodyList) {
                if (melody.parts.count == 1) {
                    [tempArray addObject:melody];
                }
            }

            
            NSSortDescriptor *valueDescriptor = [[NSSortDescriptor alloc] initWithKey:@"dateCreated" ascending:NO];
            NSArray *descriptors = [NSArray arrayWithObject:valueDescriptor];
            self.filteredList = [tempArray sortedArrayUsingDescriptors:descriptors];
             */
            
            
            NSMutableArray *tempArray = [NSMutableArray new];
            for (NSDictionary *itemDict in self.loopArray) {
                NSArray *parts = [itemDict objectForKey:@"Parts"];
                if (parts.count == 1) {
                    [tempArray addObject:itemDict];
                }
            }
            
            
            NSSortDescriptor *valueDescriptor = [[NSSortDescriptor alloc] initWithKey:@"DateCreated" ascending:NO];
            NSArray *descriptors = [NSArray arrayWithObject:valueDescriptor];
            self.loopArray = [tempArray sortedArrayUsingDescriptors:descriptors];
            


            
            break;
        }
        case 3: {
            /*
            NSMutableArray *tempArray = [NSMutableArray new];
            for (UserMelody *melody in self.melodyList) {
                if ([melody.isChatLoopPart boolValue] == TRUE) {
                    [tempArray addObject:melody];
                }
            }
            
            NSSortDescriptor *valueDescriptor = [[NSSortDescriptor alloc] initWithKey:@"dateCreated" ascending:NO];
            NSArray *descriptors = [NSArray arrayWithObject:valueDescriptor];
            self.filteredList = [tempArray sortedArrayUsingDescriptors:descriptors];
             */
            
            NSMutableArray *tempArray = [NSMutableArray new];
            
            for (NSDictionary *itemDict in self.loopArray) {
                if ([[itemDict objectForKey:@"IsChatLoop"] boolValue] == TRUE) {
                    [tempArray addObject:itemDict];
                }
            }
            
            NSSortDescriptor *valueDescriptor = [[NSSortDescriptor alloc] initWithKey:@"DateCreated" ascending:NO];
            NSArray *descriptors = [NSArray arrayWithObject:valueDescriptor];
            self.loopArray = [tempArray sortedArrayUsingDescriptors:descriptors];
                        break;
        }
            
        default:
            break;
    }
    
    [self.tableView reloadData];
}

-(void)didFinishWithInfo:(NSDictionary *)userDict
{
    //sdfsdf
    
    if ([userDict objectForKey:@"Id"] != nil) {
        [[NetworkManager sharedManager] uploadChatUserMelody:userDict];
    } else if ([userDict objectForKey:@"LoopId"] != nil) {
        [[NetworkManager sharedManager] uploadLoopPart:userDict];
    } else {
        [[NetworkManager sharedManager] uploadUserMelody:userDict];
    }
}

@end
