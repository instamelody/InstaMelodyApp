//
//  SignInViewController.h
//  
//
//  Created by Ahmed Bakir on 8/5/15.
//
//

#import <UIKit/UIKit.h>
#import "M13ProgressHUD.h"
#import "M13ProgressViewRing.h"
#import "SignUpViewController.h"

@interface SignInViewController : UIViewController <UITextFieldDelegate, SignUpDelegate>

@property (nonatomic, strong) IBOutlet UILabel *userLabel;
@property (nonatomic, strong) IBOutlet UILabel *passLabel;

@property (nonatomic, strong) IBOutlet UITextField *userField;
@property (nonatomic, strong) IBOutlet UITextField *passField;

@property (nonatomic, strong) IBOutlet UIScrollView *scrollView;

-(IBAction)signIn:(id)sender;
-(IBAction)cancel:(id)sender;

@end
