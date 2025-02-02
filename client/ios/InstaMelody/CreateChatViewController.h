//
//  CreateChatViewController.h
//  
//
//  Created by Ahmed Bakir on 8/7/15.
//
//

#import <UIKit/UIKit.h>
#import "CLTokenInputView.h"

@protocol CreateChatDelegate <NSObject>

-(void)didCreateChatWithId:(NSString *)chatId;

@end

@interface CreateChatViewController : UIViewController <CLTokenInputViewDelegate, UITableViewDataSource, UITableViewDelegate>

@property (nonatomic, strong) id <CreateChatDelegate> delegate;

@property (nonatomic, strong) IBOutlet UITextField *nameField;
@property (nonatomic, strong) IBOutlet UITextField *messageField;
@property (nonatomic, strong) IBOutlet UITextField *topicField;

@property (strong, nonatomic) IBOutlet NSLayoutConstraint *tokenInputTopSpace;
@property (strong, nonatomic) IBOutlet CLTokenInputView *tokenInputView;
@property (strong, nonatomic) IBOutlet UITableView *tableView;
@property (strong, nonatomic) IBOutlet NSLayoutConstraint *tableViewTopLayoutConstraint;

-(IBAction)cancel:(id)sender;

@end
