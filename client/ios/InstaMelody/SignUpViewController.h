//
//  CreateUserViewController.h
//  
//
//  Created by Ahmed Bakir on 7/1/15.
//
//

#import <UIKit/UIKit.h>

@interface SignUpViewController : UIViewController <UITextFieldDelegate>


@property IBOutlet UITextField * usernameField;
@property IBOutlet UITextField * passwordField;
@property IBOutlet UITextField * phoneNumberField;
@property IBOutlet UITextField * firstNameField;
@property IBOutlet UITextField * lastNameField;
@property IBOutlet UITextField * emailAddressField;

-(IBAction)submit:(id)sender;

@end
