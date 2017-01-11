//
//  FriendsTableViewController.h
//  
//
//  Created by Ahmed Bakir on 8/7/15.
//
//

#import <UIKit/UIKit.h>
#import "MGSwipeTableCell.h"
#import "MGSwipeButton.h"
@import ContactsUI;

@interface FriendsTableViewController : UITableViewController <MGSwipeTableCellDelegate, CNContactPickerDelegate>

-(IBAction)requestFriend:(id)sender;

@end
