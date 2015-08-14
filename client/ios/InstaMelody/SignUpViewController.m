//
//  CreateUserViewController.m
//  
//
//  Created by Ahmed Bakir on 7/1/15.
//
//

#import "SignUpViewController.h"
#import "AFHTTPRequestOperationManager.h"
#import "constants.h"

@interface SignUpViewController ()

@end

@implementation SignUpViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.
    
    self.usernameField.delegate = self;
    self.passwordField.delegate = self;
    self.phoneNumberField.delegate = self;
    self.firstNameField.delegate = self;
    self.lastNameField.delegate = self;
    self.emailAddressField.delegate = self;
    
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

- (IBAction)submit:(id)sender {
    
    if (![self.usernameField.text isEqualToString:@""] && ![self.passwordField.text isEqualToString:@""] && ![self.firstNameField.text isEqualToString:@""] && ![self.lastNameField.text isEqualToString:@""] && ![self.phoneNumberField.text isEqualToString:@""]) {

        NSString *encodedEmail = [self.emailAddressField.text stringByAddingPercentEncodingWithAllowedCharacters:NSCharacterSet.URLQueryAllowedCharacterSet];
        NSDictionary *parameters = @{@"DisplayName": self.usernameField.text, @"Password": self.passwordField.text, @"FirstName": self.firstNameField.text, @"LastName": self.lastNameField.text, @"PhoneNumber" : self.phoneNumberField.text, @"EmailAddress" : encodedEmail};
        
        AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
        //[manager.requestSerializer setValue:@"application/x-www-form-urlencoded" forHTTPHeaderField:@"Content-Type"];
        //manager.requestSerializer = [AFHTTPRequestSerializer serializer];
        /*
         [[AFHTTPRequestSerializer serializer] requestWithMethod:@"POST" URLString:URLString parameters:parameters];
         */
        
        NSString *requestUrl = [NSString stringWithFormat:@"%@/User/New", BASE_URL];
        
        [manager POST:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
            NSLog(@"JSON: %@", responseObject);
            UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Success" message:@"You are now a user" delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
            [alertView show];
            
            NSDictionary *responseDict = (NSDictionary *)responseObject;
            [[NSUserDefaults standardUserDefaults] setObject:[responseDict objectForKey:@"Token"] forKey:@"authToken"];
            [[NSUserDefaults standardUserDefaults] synchronize];
        } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
            //NSLog(@"Error: %@", error);
            
            NSString* ErrorResponse = [[NSString alloc] initWithData:(NSData *)error.userInfo[AFNetworkingOperationFailingURLResponseDataErrorKey] encoding:NSUTF8StringEncoding];
            NSLog(@"%@",ErrorResponse);
            
            UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:error.description delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
            [alertView show];
        }];
    } else {
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:@"Please fill in all fields" delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
        [alertView show];
    }
    
}


- (BOOL)textFieldShouldBeginEditing:(UITextField *)textField
{
    return YES;
}

// It is important for you to hide the keyboard
- (BOOL)textFieldShouldReturn:(UITextField *)textField
{
    [textField resignFirstResponder];
    return YES;
}

- (void)textFieldDidBeginEditing:(UITextField *)textField {
    
    NSInteger parentTag = textField.tag + 10;
    UIView *parentView = [self.scrollView viewWithTag:parentTag];
    
    CGPoint scrollPoint = CGPointMake(0, parentView.frame.origin.y - 20);
    [self.scrollView setContentOffset:scrollPoint animated:YES];
}

- (void)textFieldDidEndEditing:(UITextField *)textField {
    [self.scrollView setContentOffset:CGPointZero animated:YES];
}

/*
- (void)textFieldDidBeginEditing:(UITextField *)textField {
    CGPoint scrollPoint = CGPointMake(0, textField.frame.origin.y);
    [self.scrollView setContentOffset:scrollPoint animated:YES];
}

- (void)textFieldDidEndEditing:(UITextField *)textField {
    [self.scrollView setContentOffset:CGPointZero animated:YES];
}*/


@end
