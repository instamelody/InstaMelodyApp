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
#import "StationViewController.h"

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
        if (self.friendsList.count > 0) {
            return self.friendsList.count;
        } else {
            return 1;
        }
    }
    
    if (section == 1) {
        if (self.pendingFriendsList.count > 0) {
            return self.pendingFriendsList.count;
        } else {
            return 1;
        }
    }
    return  0;
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    FriendCell *cell = (FriendCell *)[tableView dequeueReusableCellWithIdentifier:@"FriendCell" forIndexPath:indexPath];
    
    cell.delegate = self;
    cell.profileImageView.layer.cornerRadius = cell.profileImageView.frame.size.height / 2;
    cell.profileImageView.layer.masksToBounds = YES;
    
    cell.backgroundColor = [UIColor clearColor];

    cell.approveButton.titleLabel.font = [UIFont fontAwesomeFontOfSize:22.0f];
    cell.denyButton.titleLabel.font = [UIFont fontAwesomeFontOfSize:22.0f];
    
    [cell.approveButton setTitle:[NSString fontAwesomeIconStringForEnum:FACheck] forState:UIControlStateNormal];
    [cell.denyButton setTitle:[NSString fontAwesomeIconStringForEnum:FAtrash] forState:UIControlStateNormal];
    
    // Configure the cell...
    
    if (indexPath.section == 0)  {
        
        cell.approveButton.hidden = YES;
        cell.denyButton.hidden = YES;
        
        if (self.friendsList.count > 0) {
            
            Friend *friend = (Friend *)[self.friendsList objectAtIndex:indexPath.row];
            
            //NSDictionary *friendDict = [self.friendsList objectAtIndex:indexPath.row];
            
            cell.nameLabel.text = friend.displayName;
            
            cell.detailLabel.text = [NSString stringWithFormat:@"%@ %@", friend.firstName, friend.lastName];
            
            cell.profileImageView.image = [UIImage imageNamed:@"Profile"];
            
            if (friend.profileFilePath != nil && ![friend.profileFilePath isEqualToString:@""]) {
                
                NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
                
                NSString *profilePath = [documentsPath stringByAppendingPathComponent:@"Profiles"];
                NSString *imageName = [friend.profileFilePath lastPathComponent];
                
                NSString *imagePath = [profilePath stringByAppendingPathComponent:imageName];
                cell.profileImageView.image = [UIImage imageWithContentsOfFile:imagePath];
                
            }
        } else {
            cell.nameLabel.text = @"No VIPs yet";
            
            cell.detailLabel.text = @"Please request a friend";
            
            cell.profileImageView.image = [UIImage imageNamed:@"Profile"];
        }
        
        
    } else if (indexPath.section == 1) {
        if (self.pendingFriendsList.count > 0)  {
            NSDictionary *friendDict = [self.pendingFriendsList objectAtIndex:indexPath.row];
            
            cell.approveButton.hidden = NO;
            cell.denyButton.hidden = NO;
            
            cell.nameLabel.text = [friendDict objectForKey:@"DisplayName"];
            
            cell.detailLabel.text = [NSString stringWithFormat:@"%@ %@", [friendDict objectForKey:@"FirstName"], [friendDict objectForKey:@"LastName"]];
            
            cell.approveButton.tag = indexPath.row;
            
            cell.profileImageView.image = [UIImage imageNamed:@"Profile"];

        } else {
            
            cell.approveButton.hidden = YES;
            cell.denyButton.hidden = YES;
            
            cell.nameLabel.text = @"No Fans";
            
            cell.detailLabel.text = @"Go viral!";
            
            cell.profileImageView.image = [UIImage imageNamed:@"Profile"];
        }
        
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
        return @"VIPs";
    } /*else if (section == 1)  {
        return @"People who have added me";
    }
    return @"People I've added";
       */
    return @"Fans";
}

- (void)tableView:(UITableView *)tableView willDisplayHeaderView:(UIView *)view forSection:(NSInteger)section
{
    // Background color
    view.tintColor = [UIColor lightGrayColor];
    
    // Text Color
    UITableViewHeaderFooterView *header = (UITableViewHeaderFooterView *)view;
    [header.textLabel setTextColor:[UIColor whiteColor]];
    [header.textLabel setFont:[UIFont fontWithName:@"Century Gothic" size:18.0f]];
    
    // Another way to set the background color
    // Note: does not preserve gradient effect of original header
    // header.contentView.backgroundColor = [UIColor blackColor];
}

-(void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath {
    
    if (indexPath.section == 0) {
        Friend *selectedFriend = [self.friendsList objectAtIndex:indexPath.row];
        UIStoryboard *mainSB = [UIStoryboard storyboardWithName:@"Main" bundle:nil];
        
        StationViewController *vc = (StationViewController *)[mainSB instantiateViewControllerWithIdentifier:@"StationViewController"];
        vc.selectedFriend = selectedFriend;
        
        [self.navigationController pushViewController:vc animated:YES];
        
    } else {
        [self.tableView deselectRowAtIndexPath:indexPath animated:YES];
    }
    
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


#pragma mark - Navigation

// In a storyboard-based application, you will often want to do a little preparation before navigation
- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
    // Get the new view controller using [segue destinationViewController].
    // Pass the selected object to the new view controller.
    
    if ([segue.identifier isEqualToString:@"showFriendStation"]) {
        NSIndexPath *indexPath = [self.tableView indexPathForSelectedRow];
        
        if (indexPath.section == 0) {
            Friend *selectedFriend = [self.friendsList objectAtIndex:indexPath.row];
            StationViewController *stationVC = (StationViewController *)segue.destinationViewController;
            stationVC.selectedFriend = selectedFriend;
        }
    }
}

#pragma mark - web actions

-(void)refreshFriendsList {
    [self getFriendList];
    [self getPendingFriendList];
}

-(void)getFriendList {
    //
    
    //add observer for fetching friend list asynchronously
    
    NSString *requestUrl = [NSString stringWithFormat:@"%@/User/Friends", API_BASE_URL];
    
    NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    
    NSDictionary *parameters = @{@"token": token};
    
    [manager GET:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        NSLog(@"JSON: %@", responseObject);
        
        NSArray *friendsList = (NSArray *)responseObject;
        
        [Friend MR_truncateAll];
        
        for (NSDictionary *friendDict in friendsList) {
            Friend *newFriend = [Friend MR_createEntity];
            newFriend.firstName = [friendDict objectForKey:@"FirstName"];
            newFriend.lastName = [friendDict objectForKey:@"LastName"];
            newFriend.userId = [friendDict objectForKey:@"Id"];
            newFriend.displayName = [friendDict objectForKey:@"DisplayName"];
            
            if ([friendDict objectForKey:@"Image"] != nil && [[friendDict objectForKey:@"Image"] isKindOfClass:[NSDictionary class]]) {
                NSDictionary *imageDict = [friendDict objectForKey:@"Image"];
                newFriend.profileFilePath = [imageDict objectForKey:@"FilePath"];
            }
        }
        
        [[NSManagedObjectContext MR_defaultContext] MR_saveToPersistentStoreWithCompletion:^(BOOL contextDidSave, NSError *error) {
            if (error == nil) {
                NSLog(@"CORE DATA save - successful");
                
                //get friends from core data
                self.friendsList = [[DataManager sharedManager] friendList];
                [self.tableView reloadData];
                
            } else {
                NSLog(@"CORE DATA error - %@", error.description);
            }
            //
        }];
        
        
        //NSDictionary *responseDict = (NSDictionary *)responseObject;
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        NSLog(@"Error fetching friends: %@", error);
        
    }];
    
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
        if ([operation.responseObject isKindOfClass:[NSDictionary class]]) {
            NSDictionary *errorDict = [NSJSONSerialization JSONObjectWithData:(NSData *)error.userInfo[AFNetworkingOperationFailingURLResponseDataErrorKey] options:0 error:nil];
            
            NSString *ErrorResponse = [NSString stringWithFormat:@"Error %ld: %@", operation.response.statusCode, [errorDict objectForKey:@"Message"]];
            
            NSLog(@"%@",ErrorResponse);
            
            UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:ErrorResponse delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
            [alertView show];
        }
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
        if ([operation.responseObject isKindOfClass:[NSDictionary class]]) {
            NSDictionary *errorDict = [NSJSONSerialization JSONObjectWithData:(NSData *)error.userInfo[AFNetworkingOperationFailingURLResponseDataErrorKey] options:0 error:nil];
            
            NSString *ErrorResponse = [NSString stringWithFormat:@"Error %ld: %@", operation.response.statusCode, [errorDict objectForKey:@"Message"]];
            
            NSLog(@"%@",ErrorResponse);
            
            UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:ErrorResponse delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
            [alertView show];
        }
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
        if ([operation.responseObject isKindOfClass:[NSDictionary class]]) {
            NSDictionary *errorDict = [NSJSONSerialization JSONObjectWithData:(NSData *)error.userInfo[AFNetworkingOperationFailingURLResponseDataErrorKey] options:0 error:nil];
            
            NSString *ErrorResponse = [NSString stringWithFormat:@"Error %ld: %@", operation.response.statusCode, [errorDict objectForKey:@"Message"]];
            
            NSLog(@"%@",ErrorResponse);
            
            UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:ErrorResponse delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
            [alertView show];
        }
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
        if ([operation.responseObject isKindOfClass:[NSDictionary class]]) {
            NSDictionary *errorDict = [NSJSONSerialization JSONObjectWithData:(NSData *)error.userInfo[AFNetworkingOperationFailingURLResponseDataErrorKey] options:0 error:nil];
            
            NSString *ErrorResponse = [NSString stringWithFormat:@"Error %ld: %@", operation.response.statusCode, [errorDict objectForKey:@"Message"]];
            
            NSLog(@"%@",ErrorResponse);
            
            UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:ErrorResponse delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
            [alertView show];
        }
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

#pragma mark Swipe Delegate

-(BOOL) swipeTableCell:(MGSwipeTableCell*) cell canSwipe:(MGSwipeDirection) direction;
{
    return YES;
}

-(NSArray*) swipeTableCell:(MGSwipeTableCell*) cell swipeButtonsForDirection:(MGSwipeDirection)direction
             swipeSettings:(MGSwipeSettings*) swipeSettings expansionSettings:(MGSwipeExpansionSettings*) expansionSettings
{
    
    swipeSettings.transition = MGSwipeTransitionBorder;
    expansionSettings.buttonIndex = 0;
    
    if (direction == MGSwipeDirectionRightToLeft) {
        
        expansionSettings.fillOnTrigger = YES;
        expansionSettings.threshold = 1.1;
        
        CGFloat padding = 15;
        
        MGSwipeButton * fav = [MGSwipeButton buttonWithTitle:[NSString fontAwesomeIconStringForEnum:FAStar] backgroundColor:[UIColor lightGrayColor] padding:padding callback:^BOOL(MGSwipeTableCell *sender) {
            
            //do stuff
            
            return NO; //don't autohide to improve delete animation
        }];
        
        MGSwipeButton * mail = [MGSwipeButton buttonWithTitle:[NSString fontAwesomeIconStringForEnum:FAEnvelope] backgroundColor:[UIColor lightGrayColor] padding:padding callback:^BOOL(MGSwipeTableCell *sender) {
            
            //do more stuff
            [cell refreshContentView]; //needed to refresh cell contents while swipping
            return YES;
        }];
        MGSwipeButton * chat = [MGSwipeButton buttonWithTitle:[NSString fontAwesomeIconStringForEnum:FAComment] backgroundColor:[UIColor lightGrayColor] padding:padding callback:^BOOL(MGSwipeTableCell *sender) {
            
            //[cell hideSwipeAnimated:YES];
            
            return NO; //avoid autohide swipe
        }];
        
        fav.titleLabel.font = [UIFont fontAwesomeFontOfSize:20.0f];
        mail.titleLabel.font = [UIFont fontAwesomeFontOfSize:20.0f];
        chat.titleLabel.font = [UIFont fontAwesomeFontOfSize:20.0f];

        
        return @[fav, mail, chat];
    }
    
    return nil;
    
}

-(void) swipeTableCell:(MGSwipeTableCell*) cell didChangeSwipeState:(MGSwipeState)state gestureIsActive:(BOOL)gestureIsActive
{
    NSString * str;
    switch (state) {
        case MGSwipeStateNone: str = @"None"; break;
        case MGSwipeStateSwippingLeftToRight: str = @"SwippingLeftToRight"; break;
        case MGSwipeStateSwippingRightToLeft: str = @"SwippingRightToLeft"; break;
        case MGSwipeStateExpandingLeftToRight: str = @"ExpandingLeftToRight"; break;
        case MGSwipeStateExpandingRightToLeft: str = @"ExpandingRightToLeft"; break;
    }
    NSLog(@"Swipe state: %@ ::: Gesture: %@", str, gestureIsActive ? @"Active" : @"Ended");
}

@end
