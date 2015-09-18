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

@interface UserMelodyListViewController ()

@property (nonatomic, strong) NSArray *melodyList;

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
    
    return self.melodyList.count;
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    
    UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:@"MelodyCell" forIndexPath:indexPath];
    
    UserMelody *userMelody = (UserMelody *)[self.melodyList objectAtIndex:indexPath.row];
    
    //NSDictionary *friendDict = [self.friendsList objectAtIndex:indexPath.row];
    
    cell.textLabel.text = userMelody.userMelodyName;
    cell.detailTextLabel.text = [NSString stringWithFormat:@"%ld Parts", userMelody.parts.count];
    cell.backgroundColor = [UIColor clearColor];
    
    /*
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
    }*/
    
    return cell;
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
    
}

@end
