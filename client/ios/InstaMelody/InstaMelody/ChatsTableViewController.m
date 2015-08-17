//
//  ChatsTableViewController.m
//  
//
//  Created by Ahmed Bakir on 7/6/15.
//
//

#import "ChatsTableViewController.h"
#import "AFHTTPRequestOperationManager.h"
#import "DemoMessagesViewController.h"
#import "constants.h"
#import "ChatCell.h"
#import "ChatViewController.h"
#import <MagicalRecord/MagicalRecord.h>
#import "Friend.h"

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
    
    cell.profileImageView.layer.cornerRadius = cell.profileImageView.frame.size.height / 2;
    cell.profileImageView.layer.masksToBounds = YES;
    
    cell.backgroundColor = [UIColor clearColor];
    
    // Configure the cell...
    
    NSDictionary *chatDict = [self.chatsArray objectAtIndex:indexPath.row];
    
    NSArray *userArray = (NSArray *)[chatDict objectForKey:@"Users"];
    
    NSString *dateString = [chatDict objectForKey:@"DateModified"];
    
    
    NSDictionary *firstUser = userArray[0];
    Friend *friend = [Friend MR_findFirstByAttribute:@"userId" withValue:[firstUser objectForKey:@"Id"]];
    
    if (userArray.count == 2) {
        cell.nameLabel.text = [NSString stringWithFormat:@"Chat with %@", friend.displayName];
    } else {
        cell.nameLabel.text = [NSString stringWithFormat:@"Chat with %@ +%ld others", friend.displayName, userArray.count - 2];
    }
    
    
    cell.descriptionLabel.text = [NSString stringWithFormat:@"%ld users", userArray.count];
    
    cell.profileImageView.image = [UIImage imageNamed:@"Profile"];
    
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
    
    /*
     DemoMessagesViewController *vc = [DemoMessagesViewController messagesViewController];
     [self.navigationController pushViewController:vc animated:YES];
     */
    
    
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

/*
#pragma mark - Navigation

// In a storyboard-based application, you will often want to do a little preparation before navigation
- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
    // Get the new view controller using [segue destinationViewController].
    // Pass the selected object to the new view controller.
}
*/

#pragma mark - webservice actions

- (void)refreshChats {
    [self getUserChats];
}

- (void)getUserChats {
    NSString *requestUrl = [NSString stringWithFormat:@"%@/Message/Chat", BASE_URL];
    
    NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    
    NSDictionary *parameters = @{@"token": token};
    
    [manager GET:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        NSLog(@"JSON: %@", responseObject);
        
        self.chatsArray = (NSArray *)responseObject;
        
        [self.tableView reloadData];
        
        //NSDictionary *responseDict = (NSDictionary *)responseObject;
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        NSLog(@"Error: %@", error);
        
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:error.description delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
        [alertView show];
    }];
}

#pragma mark - button actions


-(IBAction)done:(id)sender {
    [self dismissViewControllerAnimated:YES completion:nil];
}


@end
