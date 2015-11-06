//
//  CreateUserViewController.h
//  
//
//  Created by Ahmed Bakir on 7/1/15.
//
//

#import <UIKit/UIKit.h>
#import "M13ProgressHUD.h"
#import "M13ProgressViewRing.h"
#import "AFHTTPRequestOperationManager.h"
#import "AFURLSessionManager.h"
#import "LTHMonthYearPickerView.h"

@protocol SignUpDelegate <NSObject>

-(void)finishedWithUserId:(NSString *)userId andPassword:(NSString *)password;

@end

@interface SignUpViewController : UIViewController <UITextFieldDelegate, LTHMonthYearPickerViewDelegate, UIImagePickerControllerDelegate, UINavigationControllerDelegate>

@property id<SignUpDelegate> delegate;

@property IBOutlet UITextField * usernameField;
@property IBOutlet UITextField * passwordField;
@property IBOutlet UITextField * phoneNumberField;
@property IBOutlet UITextField * firstNameField;
@property IBOutlet UITextField * lastNameField;
@property IBOutlet UITextField * emailAddressField;
@property IBOutlet UISegmentedControl * genderField;
@property IBOutlet UITextField *dobField;
@property IBOutlet UIButton *profileButton;
@property IBOutlet UIView *profileView;

@property IBOutlet UIScrollView *scrollView;

-(IBAction)submit:(id)sender;

-(IBAction)changeProfile:(id)sender;

@end
