//
//  CreateUserViewController.m
//  
//
//  Created by Ahmed Bakir on 7/1/15.
//
//

#import "SignUpViewController.h"
#import "AFHTTPRequestOperationManager.h"

@interface SignUpViewController ()

@end

@implementation SignUpViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.
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
        
        [manager POST:@"http://104.130.230.164/api/v0.1/user/new" parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
            NSLog(@"JSON: %@", responseObject);
            UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Success" message:@"You are now a user" delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
            [alertView show];
            
            NSDictionary *responseDict = (NSDictionary *)responseObject;
            [[NSUserDefaults standardUserDefaults] setObject:[responseDict objectForKey:@"Token"] forKey:@"authToken"];
            [[NSUserDefaults standardUserDefaults] synchronize];
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


@end
