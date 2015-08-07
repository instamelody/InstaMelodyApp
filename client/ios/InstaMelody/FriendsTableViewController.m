//
//  FriendsTableViewController.m
//  
//
//  Created by Ahmed Bakir on 8/7/15.
//
//

#import "FriendsTableViewController.h"
#import "FriendCell.h"
#import "constants.h"
#import "AFHTTPRequestOperationManager.h"

@interface FriendsTableViewController ()

@property (nonatomic, strong) NSArray *friendsList;
@property (nonatomic, strong) NSArray *pendingFriendsList;


@end

@implementation FriendsTableViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    
    
    UIImageView *tempImageView = [[UIImageView alloc] initWithImage:[UIImage imageNamed:@"blurBlue"]];
    [tempImageView setFrame:self.tableView.frame];
    
    self.tableView.backgroundView = tempImageView;
    
    // Uncomment the following line to preserve selection between presentations.
    // self.clearsSelectionOnViewWillAppear = NO;
    
    // Uncomment the following line to display an Edit button in the navigation bar for this view controller.
    // self.navigationItem.rightBarButtonItem = self.editButtonItem;
    
    [self refreshFriendsList];
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

#pragma mark - Table view data source

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView {
    // Return the number of sections.
    return 2;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    // Return the number of rows in the section.
    
    if (section == 0) {
        return self.friendsList.count;
    }
    return self.pendingFriendsList.count;
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    FriendCell *cell = (FriendCell *)[tableView dequeueReusableCellWithIdentifier:@"FriendCell" forIndexPath:indexPath];
    
    cell.profileImageView.layer.cornerRadius = cell.profileImageView.frame.size.height / 2;
    cell.profileImageView.layer.masksToBounds = YES;
    
    cell.backgroundColor = [UIColor clearColor];

    
    
    // Configure the cell...
    
    if (indexPath.section == 0)  {
        
        NSDictionary *friendDict = [self.friendsList objectAtIndex:indexPath.row];
        
        cell.approveButton.hidden = YES;
        
        cell.nameLabel.text = [friendDict objectForKey:@"DisplayName"];
        
        cell.profileImageView.image = [UIImage imageNamed:@"Profile"];
        
    } else {
        NSDictionary *friendDict = [self.pendingFriendsList objectAtIndex:indexPath.row];
        
        cell.nameLabel.text = [friendDict objectForKey:@"DisplayName"];
        cell.approveButton.tag = indexPath.row;
        
        cell.profileImageView.image = [UIImage imageNamed:@"Profile"];
    }
    
    return cell;
}

-(NSString *)tableView:(UITableView *)tableView titleForHeaderInSection:(NSInteger)section
{
    if (section == 0) {
        return @"Friends";
    }
    return @"Pending Friends";
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

#pragma mark - web actions

-(void)refreshFriendsList {
    [self getFriendList];
    [self getPendingFriendList];
}

-(void)getFriendList {
    //
    
    NSString *requestUrl = [NSString stringWithFormat:@"%@/User/Friends", BASE_URL];
    
    NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    
    
    NSDictionary *parameters = @{@"token": token};
    
    [manager GET:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        NSLog(@"JSON: %@", responseObject);
        
        self.friendsList = (NSArray *)responseObject;
        
        [self.tableView reloadData];
        
        //NSDictionary *responseDict = (NSDictionary *)responseObject;
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        NSLog(@"Error: %@", error);
        
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:error.description delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
        [alertView show];
    }];
    
}

-(void)getPendingFriendList {
    //
    
    NSString *requestUrl = [NSString stringWithFormat:@"%@/User/Friends/Pending", BASE_URL];
    
    NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    
    
    NSDictionary *parameters = @{@"token": token};
    
    [manager GET:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        NSLog(@"JSON: %@", responseObject);
        
        self.pendingFriendsList = (NSArray *)responseObject;
        
        [self.tableView reloadData];
        
        //NSDictionary *responseDict = (NSDictionary *)responseObject;
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        NSLog(@"Error: %@", error);
        
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:error.description delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
        [alertView show];
    }];
    
}

#pragma mark - button actions

-(IBAction)approveFriend:(id)sender {
    UIButton *sendingButton = (UIButton *)sender;
    
    NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
    
    NSDictionary *pendingFriend = [self.pendingFriendsList objectAtIndex:sendingButton.tag];
    
    NSDictionary *parameters = @{@"Token": token, @"User": @{@"EmailAddress": [pendingFriend objectForKey:@"EmailAddress"]}};

    NSString *requestUrl = [NSString stringWithFormat:@"%@/User/Friend/Approve", BASE_URL];
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    
    [manager POST:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        NSLog(@"JSON: %@", responseObject);
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Success" message:@"You have added a friend" delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
        [alertView show];
        
        [self refreshFriendsList];
        
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        NSLog(@"Error: %@", error);
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:error.description delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
        [alertView show];
    }];
}

@end
