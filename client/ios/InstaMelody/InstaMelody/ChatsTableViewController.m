//
//  ChatsTableViewController.m
//  
//
//  Created by Ahmed Bakir on 7/6/15.
//
//

#import "ChatsTableViewController.h"
#import "AFHTTPRequestOperationManager.h"
#import "constants.h"
#import "ChatCell.h"
#import "ChatViewController.h"
#import <MagicalRecord/MagicalRecord.h>
#import "Friend.h"

#import "UIFont+FontAwesome.h"
#import "NSString+FontAwesome.h"
#import "DataManager.h"

@interface ChatsTableViewController ()

@property (nonatomic, strong) NSArray *chatsArray;
@property (nonatomic, strong) NSDateFormatter *dateFormatter;

@property (nonatomic, strong) NSArray *friendsList;

@end

@implementation ChatsTableViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    
    // Uncomment the following line to preserve selection between presentations.
    // self.clearsSelectionOnViewWillAppear = NO;
    
    // Uncomment the following line to display an Edit button in the navigation bar for this view controller.
    // self.navigationItem.rightBarButtonItem = self.editButtonItem;
    
    UIImageView *tempImageView = [[UIImageView alloc] initWithImage:[UIImage imageNamed:@"blurBlue"]];
    [tempImageView setFrame:self.tableView.frame];
    
    self.tableView.backgroundView = tempImageView;
    
    //2015-07-07T16:52:02.217
    
    self.dateFormatter = [[NSDateFormatter alloc] init];
    
    self.friendsList = [Friend MR_findAll];
    
}

- (void)viewWillAppear:(BOOL)animated  {
    [super viewWillAppear:animated];
    [self refreshChats];
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

#pragma mark - Table view data source

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView {
    // Return the number of sections.
    return 1;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    // Return the number of rows in the section.
    return [self.chatsArray count];
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {
 
    ChatCell *cell = (ChatCell *)[tableView dequeueReusableCellWithIdentifier:@"ChatCell" forIndexPath:indexPath];
    
    cell.delegate = self;
    cell.profileImageView.layer.cornerRadius = cell.profileImageView.frame.size.height / 2;
    cell.profileImageView.layer.masksToBounds = YES;
    
    cell.backgroundColor = [UIColor clearColor];
    
    // Configure the cell...
    
    NSDictionary *chatDict = [self.chatsArray objectAtIndex:indexPath.row];
    
    NSArray *userArray = (NSArray *)[chatDict objectForKey:@"Users"];
    
    NSString *dateString = [chatDict objectForKey:@"DateModified"];
    
    cell.profileImageView.image = [UIImage imageNamed:@"Profile"];
    
    
    NSDictionary *firstUser = userArray[0];
    Friend *friend = [Friend MR_findFirstByAttribute:@"userId" withValue:[firstUser objectForKey:@"Id"]];
    
    if (friend.profileFilePath != nil && ![friend.profileFilePath isEqualToString:@""]) {
        
        NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
        
        NSString *profilePath = [documentsPath stringByAppendingPathComponent:@"Profiles"];
        NSString *imageName = [friend.profileFilePath lastPathComponent];
        
        NSString *imagePath = [profilePath stringByAppendingPathComponent:imageName];
        cell.profileImageView.image = [UIImage imageWithContentsOfFile:imagePath];
        
    }
    
    
    if (userArray.count == 2) {
        cell.nameLabel.text = friend.displayName;
        cell.descriptionLabel.text = @"1:1 chat";
        if ([chatDict objectForKey:@"Name"] != nil) {
            NSString *nameString = (NSString *)[chatDict objectForKey:@"Name"];
            if ([nameString isKindOfClass:[NSString class]] && ![nameString containsString:@"ChatLoop_"]) {
                    cell.descriptionLabel.text = [chatDict objectForKey:@"Name"];
            }
        }
    } else if (userArray.count > 2) {
        
        NSString *myName = [[NSUserDefaults standardUserDefaults] objectForKey:@"DisplayName"];
        
        NSMutableString *names = [NSMutableString new];
        for (NSDictionary *user in userArray) {
            NSString *name = [user objectForKey:@"DisplayName"];
            
            if (![name isEqualToString:myName]) {
                [names appendFormat:@"%@,", name];
            }
        }
        //cell.nameLabel.text = [NSString stringWithFormat:@"Chat with %@ +%ld others", friend.displayName, userArray.count - 2];
        cell.nameLabel.text = names;
        cell.descriptionLabel.text = [NSString stringWithFormat:@"%ld users", userArray.count];
        
        if ([chatDict objectForKey:@"Name"] != nil) {
            NSString *nameString = [chatDict objectForKey:@"Name"];
            if ([nameString isKindOfClass:[NSString class]] && ![nameString containsString:@"ChatLoop_"]) {
                cell.descriptionLabel.text = [chatDict objectForKey:@"Name"];
            }
        }
    } else {
        cell.nameLabel.text = @"Group chat";
        cell.descriptionLabel.text = @"Click to join";
        
        if ([chatDict objectForKey:@"Name"] != nil) {
            NSString *nameString = [chatDict objectForKey:@"Name"];
            if ([nameString isKindOfClass:[NSString class]] && ![nameString containsString:@"ChatLoop_"]) {
                cell.descriptionLabel.text = [chatDict objectForKey:@"Name"];
            }
        }
    }
    
    
    cell.timeLabel.text = [dateString substringToIndex:10];
    
    return cell;
}

-(void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath {
    
    //chat id = indexPath.row
    
    ChatViewController *vc = [ChatViewController messagesViewController];
    
    ChatCell *cell = (ChatCell *)[self.tableView cellForRowAtIndexPath:indexPath];
    
    vc.title = cell.nameLabel.text;
    vc.chatDict = [self.chatsArray objectAtIndex:indexPath.row];
    
    [self.navigationController pushViewController:vc animated:YES];
    
    
    
     //UINavigationController *navCon = [[UINavigationController alloc] initWithRootViewController:vc];
     //vc.navigationItem.leftBarButtonItem = [[UIBarButtonItem alloc] initWithTitle:@"Cancel" style:UIBarButtonItemStyleDone target:self action:@selector(done:)];
     
     
     //[self.navigationController presentViewController:navCon animated:YES completion:nil];
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
    if ([segue.identifier isEqualToString:@"presentCreateChat"]) {
        UINavigationController *navVC = segue.destinationViewController;
        CreateChatViewController *chatVC = (CreateChatViewController *)navVC.topViewController;
        chatVC.delegate = self;
    }
    
}


-(void)didCreateChatWithId:(NSString *)chatId {
    
    [self getChat:chatId];

}

#pragma mark - webservice actions

- (void)refreshChats {
    [self getUserChats];
}


- (void)getChat:(NSString *)chatId {
    NSString *requestUrl = [NSString stringWithFormat:@"%@/Message/Chat", API_BASE_URL];
    
    NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    
    NSDictionary *parameters = @{@"token": token, @"id": chatId};
    
    [manager GET:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        NSLog(@"JSON: %@", responseObject);
        
        NSDictionary *chatDict = (NSDictionary *)responseObject;
        
        ChatViewController *vc = [ChatViewController messagesViewController];
        
        //get chat
         
         vc.title = @"New Chat";
         vc.chatDict = chatDict;
        
        [self.navigationController pushViewController:vc animated:YES];
        
        //NSDictionary *responseDict = (NSDictionary *)responseObject;
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


- (void)getUserChats {
    NSString *requestUrl = [NSString stringWithFormat:@"%@/Message/Chat", API_BASE_URL];
    
    NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    
    NSDictionary *parameters = @{@"token": token};
    
    [manager GET:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        NSLog(@"JSON: %@", responseObject);
        
        NSArray *tempChats = (NSArray *)responseObject;
        
        NSSortDescriptor *valueDescriptor = [[NSSortDescriptor alloc] initWithKey:@"DateModified" ascending:NO];
        NSArray *descriptors = [NSArray arrayWithObject:valueDescriptor];
        NSArray *sortedArray = [tempChats sortedArrayUsingDescriptors:descriptors];
        
        self.chatsArray = sortedArray;
        
        [self.tableView reloadData];
        
        //NSDictionary *responseDict = (NSDictionary *)responseObject;
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

-(void)deleteChatWithIndex:(NSIndexPath *)indexPath {
    
    NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
    
    NSDictionary *chatDict = [self.chatsArray objectAtIndex:indexPath.row];
    
    NSString *chatId = [chatDict objectForKey:@"Id"];
    
    NSDictionary *parameters = @{@"Token": token, @"Chat": @{@"Id": chatId}};
    
    NSString *requestUrl = [NSString stringWithFormat:@"%@/Message/Chat/Delete", API_BASE_URL];
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    
    [manager POST:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        NSLog(@"JSON: %@", responseObject);
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Success" message:@"You have removed yourself from a chat" delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
        [alertView show];
        
        [self refreshChats];
        
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


-(IBAction)done:(id)sender {
    [self dismissViewControllerAnimated:YES completion:nil];
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
        
        MGSwipeButton * del = [MGSwipeButton buttonWithTitle:[NSString fontAwesomeIconStringForEnum:FAtrash] backgroundColor:[UIColor lightGrayColor] padding:padding callback:^BOOL(MGSwipeTableCell *sender) {
            
            UIAlertController *alert = [UIAlertController alertControllerWithTitle:@"Delete chat" message:@"Are you sure you want to delete this chat?" preferredStyle:UIAlertControllerStyleAlert];
            UIAlertAction *cancelAction = [UIAlertAction actionWithTitle:@"Cancel" style:UIAlertActionStyleCancel handler:^(UIAlertAction * _Nonnull action) {
                //do stuff
                [self.tableView reloadData];
            }];
            
            UIAlertAction *okAction = [UIAlertAction actionWithTitle:@"OK" style:UIAlertActionStyleDefault handler:^(UIAlertAction * _Nonnull action) {
                //do stuff
                [self deleteChatWithIndex:[self.tableView indexPathForCell:sender]];
            }];
            
            [alert addAction:cancelAction];
            [alert addAction:okAction];
            [self presentViewController:alert animated:YES completion:nil];
            
            return NO; //don't autohide to improve delete animation
        }];
        
        del.titleLabel.font = [UIFont fontAwesomeFontOfSize:20.0f];
        
        
        return @[del];
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
