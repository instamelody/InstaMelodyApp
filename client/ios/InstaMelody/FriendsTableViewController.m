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
#import "UIFont+FontAwesome.h"
#import "NSString+FontAwesome.h"
#import "DataManager.h"

@interface FriendsTableViewController ()

@property (nonatomic, strong) NSArray *friendsList;
@property (nonatomic, strong) NSArray *pendingFriendsList;
@property (nonatomic, strong) NSArray *otherFriendsList;


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
    //return 3;
    return 2;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    // Return the number of rows in the section.
    
    if (section == 0) {
        return self.friendsList.count;
    } /*else if (section == 1) {
        return self.pendingFriendsList.count;
    }
    return self.otherFriendsList.count;
       */
    return self.pendingFriendsList.count;
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    FriendCell *cell = (FriendCell *)[tableView dequeueReusableCellWithIdentifier:@"FriendCell" forIndexPath:indexPath];
    
    cell.profileImageView.layer.cornerRadius = cell.profileImageView.frame.size.height / 2;
    cell.profileImageView.layer.masksToBounds = YES;
    
    cell.backgroundColor = [UIColor clearColor];

    cell.approveButton.titleLabel.font = [UIFont fontAwesomeFontOfSize:22.0f];
    cell.denyButton.titleLabel.font = [UIFont fontAwesomeFontOfSize:22.0f];
    
    [cell.approveButton setTitle:[NSString fontAwesomeIconStringForEnum:FAIconCheck] forState:UIControlStateNormal];
    [cell.denyButton setTitle:[NSString fontAwesomeIconStringForEnum:FAIconRemove] forState:UIControlStateNormal];
    
    // Configure the cell...
    
    if (indexPath.section == 0)  {
        
        Friend *friend = (Friend *)[self.friendsList objectAtIndex:indexPath.row];
        
        //NSDictionary *friendDict = [self.friendsList objectAtIndex:indexPath.row];
        
        cell.approveButton.hidden = YES;
        cell.denyButton.hidden = YES;
        
        cell.nameLabel.text = friend.displayName;
        
        cell.profileImageView.image = [UIImage imageNamed:@"Profile"];
        
        if (friend.profileFilePath != nil && ![friend.profileFilePath isEqualToString:@""]) {
            
            NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
            
            NSString *profilePath = [documentsPath stringByAppendingPathComponent:@"Profiles"];
            NSString *imageName = [friend.profileFilePath lastPathComponent];
            
            NSString *imagePath = [profilePath stringByAppendingPathComponent:imageName];
            cell.profileImageView.image = [UIImage imageWithContentsOfFile:imagePath];
            
        }
        
        
    } else if (indexPath.section == 1) {
        NSDictionary *friendDict = [self.pendingFriendsList objectAtIndex:indexPath.row];
        
        cell.nameLabel.text = [friendDict objectForKey:@"DisplayName"];
        cell.approveButton.tag = indexPath.row;
        
        cell.profileImageView.image = [UIImage imageNamed:@"Profile"];
    } else {
        NSDictionary *friendDict = [self.otherFriendsList objectAtIndex:indexPath.row];

        cell.approveButton.hidden = YES;
        cell.denyButton.hidden = YES;
        cell.nameLabel.text = [friendDict objectForKey:@"DisplayName"];

        cell.profileImageView.image = [UIImage imageNamed:@"Profile"];
    }
    
    return cell;
}

-(NSString *)tableView:(UITableView *)tableView titleForHeaderInSection:(NSInteger)section
{
    if (section == 0) {
        return @"Friends";
    } /*else if (section == 1)  {
        return @"People who have added me";
    }
    return @"People I've added";
       */
    return @"Pending Friend Requests";
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
    
    //add observer for fetching friend list asynchronously
    
    //get friends from core data
    self.friendsList = [[DataManager sharedManager] friendList];
    
}

-(void)getPendingFriendList {
    //
    
    NSString *requestUrl = [NSString stringWithFormat:@"%@/User/Friends/Pending", API_BASE_URL];
    
    NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    
    
    NSDictionary *parameters = @{@"token": token};
    
    [manager GET:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        NSLog(@"JSON: %@", responseObject);
        
        NSArray *responseFriendList = (NSArray *)responseObject;
        
        NSMutableArray *pendingFriendList = [NSMutableArray new];
        NSMutableArray *otherFriendList = [NSMutableArray new];
        
        for (NSDictionary *friend in responseFriendList) {
            NSInteger isRequestor = [[friend objectForKey:@"IsRequestor"] integerValue];
            if ( isRequestor == 1) {
                [otherFriendList addObject:friend];
            } else {
                [pendingFriendList addObject:friend];
            }
        }
        
        self.pendingFriendsList = (NSArray *)pendingFriendList;
        self.otherFriendsList = (NSArray *)otherFriendList;
        
        [self.tableView reloadData];
        
        //NSDictionary *responseDict = (NSDictionary *)responseObject;
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        NSLog(@"Error: %@", error);
        
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:error.description delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
        [alertView show];
    }];
    
}

-(void)requestFriendWithEmail:(NSString *)emailString {
    NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
    
    NSDictionary *parameters = @{@"Token": token, @"User": @{@"EmailAddress": emailString}};
    
    NSString *requestUrl = [NSString stringWithFormat:@"%@/User/Friend/Request", API_BASE_URL];
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    
    [manager POST:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        NSLog(@"JSON: %@", responseObject);
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Success" message:@"You have requested a friend" delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
        [alertView show];
        
        [self refreshFriendsList];
        
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        NSLog(@"Error: %@", error);
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:error.description delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
        [alertView show];
    }];
}

-(void)requestFriendWithUsername:(NSString *)usernameString {
    NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
    
    NSDictionary *parameters = @{@"Token": token, @"User": @{@"DisplayName": usernameString}};
    
    NSString *requestUrl = [NSString stringWithFormat:@"%@/User/Friend/Request", API_BASE_URL];
    
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


#pragma mark - button actions

-(IBAction)approveFriend:(id)sender {
    UIButton *sendingButton = (UIButton *)sender;
    
    NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
    
    NSDictionary *pendingFriend = [self.pendingFriendsList objectAtIndex:sendingButton.tag];
    
    NSDictionary *parameters = @{@"Token": token, @"User": @{@"EmailAddress": [pendingFriend objectForKey:@"EmailAddress"]}};

    NSString *requestUrl = [NSString stringWithFormat:@"%@/User/Friend/Approve", API_BASE_URL];
    
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

-(IBAction)denyFriend:(id)sender {
    UIButton *sendingButton = (UIButton *)sender;
    
    NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
    
    NSDictionary *pendingFriend = [self.pendingFriendsList objectAtIndex:sendingButton.tag];
    
    NSDictionary *parameters = @{@"Token": token, @"User": @{@"EmailAddress": [pendingFriend objectForKey:@"EmailAddress"]}};
    
    NSString *requestUrl = [NSString stringWithFormat:@"%@/User/Friend/Deny", API_BASE_URL];
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    
    [manager POST:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        NSLog(@"JSON: %@", responseObject);
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Success" message:@"You have denied a friend" delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
        [alertView show];
        
        [self refreshFriendsList];
        
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        NSLog(@"Error: %@", error);
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:error.description delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
        [alertView show];
    }];
}

-(IBAction)requestFriend:(id)sender {
    UIAlertController *alert = [UIAlertController alertControllerWithTitle:@"Add a friend" message:@"Enter a friend's email address or user name" preferredStyle:UIAlertControllerStyleAlert];
    
    [alert addTextFieldWithConfigurationHandler:^(UITextField *textField) {
        textField.placeholder = @"Username or e-mail address";
        
    }];
    
    UIAlertAction *cancelAction = [UIAlertAction actionWithTitle:@"Cancel" style:UIAlertActionStyleDestructive handler:nil];
    
    UIAlertAction *submitAction = [UIAlertAction actionWithTitle:@"Submit" style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
        //
        UITextField *textField = alert.textFields[0];
        
        if ([self NSStringIsValidEmail:textField.text]) {
            [self requestFriendWithEmail:textField.text];
        } else {
            [self requestFriendWithUsername:textField.text];
        }
    }];

    [alert addAction:cancelAction];
    [alert addAction:submitAction];
    
    [self presentViewController:alert animated:YES completion:nil];
}

-(BOOL) NSStringIsValidEmail:(NSString *)checkString
{
    BOOL stricterFilter = NO; // Discussion http://blog.logichigh.com/2010/09/02/validating-an-e-mail-address/
    NSString *stricterFilterString = @"^[A-Z0-9a-z\\._%+-]+@([A-Za-z0-9-]+\\.)+[A-Za-z]{2,4}$";
    NSString *laxString = @"^.+@([A-Za-z0-9-]+\\.)+[A-Za-z]{2}[A-Za-z]*$";
    NSString *emailRegex = stricterFilter ? stricterFilterString : laxString;
    NSPredicate *emailTest = [NSPredicate predicateWithFormat:@"SELF MATCHES %@", emailRegex];
    return [emailTest evaluateWithObject:checkString];
}

@end
