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
#import <FBSDKLoginKit/FBSDKLoginKit.h>

@interface SignInViewController : UIViewController <UITextFieldDelegate, SignUpDelegate, FBSDKLoginButtonDelegate>

@property (nonatomic, strong) IBOutlet UILabel *userLabel;
@property (nonatomic, strong) IBOutlet UILabel *passLabel;

@property (nonatomic, strong) IBOutlet UITextField *userField;
@property (nonatomic, strong) IBOutlet UITextField *passField;

@property (nonatomic, strong) IBOutlet UIScrollView *scrollView;

@property (weak, nonatomic) IBOutlet FBSDKLoginButton *fbButton;

-(IBAction)signIn:(id)sender;
-(IBAction)cancel:(id)sender;

@end
