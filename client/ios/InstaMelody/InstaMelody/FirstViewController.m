//
//  FirstViewController.m
//  InstaMelody
//
//  Created by Ahmed Bakir on 7/1/15.
//  Copyright (c) 2015 InstaMelody. All rights reserved.
//

#import "FirstViewController.h"
#import "AFHTTPRequestOperationManager.h"

@interface FirstViewController ()

@end

@implementation FirstViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view, typically from a nib.
    [self updateLoginButton];
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

- (void)updateLoginButton {
    NSString *token = [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
    if (token.length > 0) {
        
        [self.loginButton setTitle:@"Log out" forState:UIControlStateNormal];
        [self.loginButton removeTarget:self action:@selector(signIn:) forControlEvents:UIControlEventTouchUpInside];
        [self.loginButton addTarget:self action:@selector(signOut:) forControlEvents:UIControlEventTouchUpInside];
    } else {
        [self.loginButton setTitle:@"Log in" forState:UIControlStateNormal];
        [self.loginButton removeTarget:self action:@selector(signOut:) forControlEvents:UIControlEventTouchUpInside];
        [self.loginButton addTarget:self action:@selector(signIn:) forControlEvents:UIControlEventTouchUpInside];
    }
    
}

- (IBAction)signOut:(id)sender {
    [[NSUserDefaults standardUserDefaults] setObject:@"" forKey:@"authToken"];
    [[NSUserDefaults standardUserDefaults] synchronize];
    [self updateLoginButton];
}

- (IBAction)signIn:(id)sender {
    UIAlertController *alert = [UIAlertController alertControllerWithTitle:@"Welcome" message:@"Please log in" preferredStyle:UIAlertControllerStyleAlert];
    UIAlertAction *loginAction = [UIAlertAction actionWithTitle:@"Log in" style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
        if (alert.textFields.count > 2) {
            UITextField *emailField = alert.textFields[0];
            UITextField *usernameField = alert.textFields[1];
            UITextField *passwordField = alert.textFields[2];
            
            NSString *encodedEmail = [emailField.text stringByAddingPercentEncodingWithAllowedCharacters:NSCharacterSet.URLQueryAllowedCharacterSet];
            
            NSDictionary *parameters = @{@"DisplayName": usernameField.text, @"Password": passwordField.text, @"EmailAddress" : encodedEmail};
            
            AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
            
            [manager POST:@"http://104.130.230.164/api/v0.1/Auth/User" parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
                NSLog(@"JSON: %@", responseObject);
                UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Success" message:@"You are now logged in" delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
                [alertView show];
                
                NSDictionary *responseDict = (NSDictionary *)responseObject;
                [[NSUserDefaults standardUserDefaults] setObject:[responseDict objectForKey:@"Token"] forKey:@"authToken"];
                [[NSUserDefaults standardUserDefaults] synchronize];
                
                [self updateLoginButton];
            } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
                NSLog(@"Error: %@", error);
                UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:error.description delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
                [alertView show];
            }];
        }
        
    }];
    
    UIAlertAction *cancelAction = [UIAlertAction actionWithTitle:@"Cancel" style:UIAlertActionStyleDestructive handler:nil];
    
    [alert addTextFieldWithConfigurationHandler:^(UITextField *textField) {
        textField.placeholder = @"E-mail address";
    }];
    
    [alert addTextFieldWithConfigurationHandler:^(UITextField *textField) {
        textField.placeholder = @"User name";
    }];
    
    [alert addTextFieldWithConfigurationHandler:^(UITextField *textField) {
        textField.placeholder = @"Password";
        textField.secureTextEntry = true;
    }];
    
    [alert addAction:cancelAction];
    [alert addAction:loginAction];
    
    [self presentViewController:alert animated:YES completion:nil];
    
}

@end
