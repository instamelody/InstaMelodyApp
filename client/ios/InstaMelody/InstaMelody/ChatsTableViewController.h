//
//  ChatsTableViewController.h
//  
//
//  Created by Ahmed Bakir on 7/6/15.
//
//

#import <UIKit/UIKit.h>
#import "MGSwipeTableCell.h"
#import "MGSwipeButton.h"
#import "CreateChatViewController.h"

@interface ChatsTableViewController : UITableViewController <MGSwipeTableCellDelegate, CreateChatDelegate>

@end
