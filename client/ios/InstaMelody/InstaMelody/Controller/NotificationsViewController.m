//
//  NotificationsViewController.m
//  InstaMelody
//
//  Created by Ahmed Bakir on 10/16/15.
//  Copyright © 2015 InstaMelody. All rights reserved.
//

#import "NotificationsViewController.h"
#import "AFHTTPRequestOperationManager.h"
#import "constants.h"
#import "NotificationCell.h"

@interface NotificationsViewController ()

@property NSArray *dataArray;
@property NSDateFormatter *dateFormatter;
@property NSDateFormatter *inputDateFormatter;

@end

@implementation NotificationsViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    
    // Uncomment the following line to preserve selection between presentations.
    // self.clearsSelectionOnViewWillAppear = NO;
    
    // Uncomment the following line to display an Edit button in the navigation bar for this view controller.
    // self.navigationItem.rightBarButtonItem = self.editButtonItem;
    
    UIImageView *tempImageView = [[UIImageView alloc] initWithImage:[UIImage imageNamed:@"blurBlue"]];
    [tempImageView setFrame:self.tableView.frame];
    
    self.dateFormatter = [[NSDateFormatter alloc] init];
    self.dateFormatter.dateStyle = NSDateFormatterShortStyle;
    
    NSLocale *enUSPOSIXLocale = [NSLocale localeWithLocaleIdentifier:@"en_US_POSIX"];
    
    self.inputDateFormatter = [[NSDateFormatter alloc] init];
    [self.inputDateFormatter setLocale:enUSPOSIXLocale];
    
    [self.inputDateFormatter setDateFormat:@"yyyy-MM-dd'T'HH:mm:ss.SSS"];
    [self.inputDateFormatter setTimeZone:[NSTimeZone timeZoneForSecondsFromGMT:0]];
    
    self.tableView.backgroundView = tempImageView;

}

-(void)viewWillAppear:(BOOL)animated {
    
    [super viewWillAppear:animated];
    [self getData];
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

-(void)getData {
    NSString *requestUrl = [NSString stringWithFormat:@"%@/Station/Newest", API_BASE_URL];
    //NSString *requestUrl = [NSString stringWithFormat:@"%@/User/Activity", API_BASE_URL];
    
    NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
    
    NSDictionary *parameters = @{@"token": token};
    
    if (self.isFeed) {
        //requestUrl should point to friends' activity
        
        //requestUrl = [NSString stringWithFormat:@"%@/User/Activity", API_BASE_URL];
        
        
        //parameters should be params for that API clal
        //parameters = @{@"token": token, @"limit": @10, @"categoryId": @1};
        //parameters = @{@"token": token, @"activityForFriends": @"true"};
    }
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];

    
    [manager GET:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        // NSLog(@"JSON: %@", responseObject);
        NSLog(@"top stations updated");
        
        NSArray *stationList = (NSArray *)responseObject;
        self.dataArray = stationList;
        
        [self.tableView reloadData];
        
        
        //NSDictionary *responseDict = (NSDictionary *)responseObject;
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        NSLog(@"Error fetching top stations: %@", error);
        
    }];
}

#pragma mark - Table view data source

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView {
    return 1;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    
    if (self.dataArray.count > 0) {
        return self.dataArray.count;
    }
    return 1;
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    NotificationCell *cell = (NotificationCell *)[tableView dequeueReusableCellWithIdentifier:@"NotificationCell" forIndexPath:indexPath];
    
    NSString *dateString = [self.dateFormatter stringFromDate:[NSDate date]];
    
    // Configure the cell...
    if (self.dataArray.count == 0) {
        cell.messageLabel.text = @"No friend activity yet";
        cell.dateLabel.text = dateString;
    } else {
        NSDictionary *infoDict = [self.dataArray objectAtIndex:indexPath.row];
        cell.messageLabel.text = [infoDict objectForKey:@"Name"];
        NSDate *date = [self.inputDateFormatter dateFromString:[infoDict objectForKey:@"DateModified"]];
        cell.dateLabel.text = [self.dateFormatter stringFromDate:date];
        
    }
    
    
    return cell;
}

/*
// Override to support conditional editing of the table view.
- (BOOL)tableView:(UITableView *)tableView canEditRowAtIndexPath:(NSIndexPath *)indexPath {
    // Return NO if you do not want the specified item to be editable.
    return YES;
}
*/

/*
// Override to support editing the table view.
- (void)tableView:(UITableView *)tableView commitEditingStyle:(UITableViewCellEditingStyle)editingStyle forRowAtIndexPath:(NSIndexPath *)indexPath {
    if (editingStyle == UITableViewCellEditingStyleDelete) {
        // Delete the row from the data source
        [tableView deleteRowsAtIndexPaths:@[indexPath] withRowAnimation:UITableViewRowAnimationFade];
    } else if (editingStyle == UITableViewCellEditingStyleInsert) {
        // Create a new instance of the appropriate class, insert it into the array, and add a new row to the table view
    }   
}
*/

/*
// Override to support rearranging the table view.
- (void)tableView:(UITableView *)tableView moveRowAtIndexPath:(NSIndexPath *)fromIndexPath toIndexPath:(NSIndexPath *)toIndexPath {
}
*/

/*
// Override to support conditional rearranging of the table view.
- (BOOL)tableView:(UITableView *)tableView canMoveRowAtIndexPath:(NSIndexPath *)indexPath {
    // Return NO if you do not want the item to be re-orderable.
    return YES;
}
*/

/*
#pragma mark - Navigation

// In a storyboard-based application, you will often want to do a little preparation before navigation
- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
    // Get the new view controller using [segue destinationViewController].
    // Pass the selected object to the new view controller.
}
*/

@end
