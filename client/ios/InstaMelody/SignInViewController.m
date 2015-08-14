//
//  SignInViewController.m
//  
//
//  Created by Ahmed Bakir on 8/5/15.
//
//

#import "SignInViewController.h"
#import "AFHTTPRequestOperationManager.h"
#import "constants.h"

#import "UIFont+FontAwesome.h"
#import "NSString+FontAwesome.h"
#import "FAImageView.h"

@interface SignInViewController ()

@end

@implementation SignInViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.
    
    self.userLabel.font  = [UIFont fontAwesomeFontOfSize:17.0f];
    self.passLabel.font  = [UIFont fontWithName:kFontAwesomeFamilyName size:17];
    
    self.userLabel.text =  [NSString fontAwesomeIconStringForEnum:FAIconUser];
    self.passLabel.text =  [NSString fontAwesomeIconStringForEnum:FAIconLock];
    
    self.userField.delegate = self;
    self.passField.delegate = self;
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

-(IBAction)signIn:(id)sender {
    
    NSString *deviceToken =  [[NSUserDefaults standardUserDefaults] objectForKey:@"deviceToken"];
    
    if (![self.userField.text isEqualToString:@""] && ![self.passField.text isEqualToString:@""] ) {
        
        
        
        NSMutableDictionary *parameters = [NSMutableDictionary dictionaryWithDictionary:@{@"DisplayName": self.userField.text, @"Password": self.passField.text}];
        
        if (deviceToken != nil) {
            [parameters setObject:deviceToken forKey:@"DeviceToken"];
        }
        
        NSString *requestUrl = [NSString stringWithFormat:@"%@/Auth/User", BASE_URL];
        
        //add 64 char string
        
        AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
        
        [manager POST:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
            NSLog(@"JSON: %@", responseObject);
            UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Success" message:@"You are now logged in" delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
            [alertView show];
            
            NSDictionary *responseDict =
            (NSDictionary *)responseObject;
            [[NSUserDefaults standardUserDefaults] setObject:[responseDict objectForKey:@"Token"] forKey:@"authToken"];
            
            [[NSUserDefaults standardUserDefaults] synchronize];
            
            
            [self getUserDetails:self.userField.text];
            

        } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
            NSLog(@"Error: %@", error);
            UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:error.description delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
            [alertView show];
        }];
    } else {
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:@"Please fill in all fields" delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
        [alertView show];
    }
    
}

-(IBAction)cancel:(id)sender {
    [self dismissViewControllerAnimated:YES completion:nil];
}

-(void)getUserDetails:(NSString*)displayName {
    
    //https://api.instamelody.com/v1.0/User?token=9d0ab021-fcf8-4ec3-b6e3-bb1d0d03b12e&displayName=testeraccount
    
    NSString *requestUrl = [NSString stringWithFormat:@"%@/User", BASE_URL];
    
    NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    
    
    NSDictionary *parameters = @{@"token": token, @"displayName": displayName};
    
    [manager GET:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        NSLog(@"JSON: %@", responseObject);
        
        
        NSDictionary *responseDict =
        (NSDictionary *)responseObject;
        [[NSUserDefaults standardUserDefaults] setObject:[responseDict objectForKey:@"Id"] forKey:@"Id"];
        [[NSUserDefaults standardUserDefaults] setObject:[responseDict objectForKey:@"DisplayName"] forKey:@"DisplayName"];
        [[NSUserDefaults standardUserDefaults] setObject:[responseDict objectForKey:@"FirstName"] forKey:@"FirstName"];
        [[NSUserDefaults standardUserDefaults] setObject:[responseDict objectForKey:@"LastName"] forKey:@"LastName"];

        
        [[NSUserDefaults standardUserDefaults] synchronize];
        
        [self cancel:nil];
        
        //NSDictionary *responseDict = (NSDictionary *)responseObject;
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        NSLog(@"Error: %@", error);
        
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:error.description delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
        [alertView show];
    }];
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

@end
