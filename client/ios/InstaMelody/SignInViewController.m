//
//  SignInViewController.m
//  
//
//  Created by Ahmed Bakir on 8/5/15.
//
//

#import "SignInViewController.h"
#import "AFHTTPRequestOperationManager.h"
#import "AFURLSessionManager.h"
#import "constants.h"
#import "FBSDKAccessToken.h"

#import "UIFont+FontAwesome.h"
#import "NSString+FontAwesome.h"
#import "FAImageView.h"

@interface SignInViewController ()
    @property M13ProgressHUD *HUD;
@end

@implementation SignInViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.
    
    //Remove cancel button for now
    self.navigationItem.leftBarButtonItem = nil;
    ////
    
    self.userLabel.font  = [UIFont fontAwesomeFontOfSize:17.0f];
    self.passLabel.font  = [UIFont fontWithName:kFontAwesomeFamilyName size:17];
    
    self.userLabel.text =  [NSString fontAwesomeIconStringForEnum:FAUser];
    self.passLabel.text =  [NSString fontAwesomeIconStringForEnum:FALock];
    
    self.userField.delegate = self;
    self.passField.delegate = self;
    
    self.HUD = [[M13ProgressHUD alloc] initWithProgressView:[[M13ProgressViewRing alloc] init]];
    self.HUD.progressViewSize = CGSizeMake(60.0, 60.0);
    self.HUD.animationPoint = CGPointMake([UIScreen mainScreen].bounds.size.width / 2, [UIScreen mainScreen].bounds.size.height / 2);
    UIWindow *window = [[[UIApplication sharedApplication] windows] objectAtIndex:0];
    [window addSubview:self.HUD];
    
    self.fbButton.readPermissions = @[@"public_profile", @"email", @"user_friends"];
    self.fbButton.delegate = self;
    self.fbButton.layer.cornerRadius = 4.0f;
    self.fbButton.layer.masksToBounds = YES;
    
    FBSDKLoginManager *loginManager = [[FBSDKLoginManager alloc] init];
    
    [loginManager logOut];
    
    // setup twitter login button and callback
    self.twitterButton = [TWTRLogInButton buttonWithLogInCompletion:^(TWTRSession* session, NSError* error) {
        if (session) {
            NSLog(@"signed in as %@", [session userName]);
            [self signUp:nil];
        } else {
            NSLog(@"error: %@", [error localizedDescription]);
        }
    }];
    //self.twitterButton.titleLabel.text = @"Log in";
    self.twitterButton.layer.cornerRadius = 4.0;
    self.twitterButton.layer.masksToBounds = true;
    
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

#pragma mark - Navigation

// In a storyboard-based application, you will often want to do a little preparation before navigation
- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
    // Get the new view controller using [segue destinationViewController].
    // Pass the selected object to the new view controller.
    if ([segue.identifier isEqualToString:@"presentSignupSegue"]) {
        SignUpViewController *signupVC = (SignUpViewController *)segue.destinationViewController;
        signupVC.delegate = self;
    }
}

#pragma mark - fb sdk delegate

-(void)loginButton:(FBSDKLoginButton *)loginButton didCompleteWithResult:(FBSDKLoginManagerLoginResult *)result error:(NSError *)error {
    if (error == nil && result != nil) {
        
        NSString *fbToken = result.token.tokenString;
        NSString *userId = result.token.userID;
        
        [self loginWithToken:fbToken andId:userId];
    } else {
        NSLog(@"Facebook error: %@", error.description);
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:@"Error connecting to Facebook" delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
        [alertView show];
    }
}

-(void)loginButtonDidLogOut:(FBSDKLoginButton *)loginButton {
    
}

-(void)loginWithToken:(NSString *)token andId:(NSString *)userId {
    
    //[self signUp:nil];
    
    NSString *deviceToken =  [[NSUserDefaults standardUserDefaults] objectForKey:@"deviceToken"];
    //This is the push notification device token, if there is one. If not, it is nil.
    
    if (!token)
    {
        //User cancelled out of Facebook login process
        return;
    }
    
    NSMutableDictionary *parameters = [NSMutableDictionary dictionaryWithDictionary:@{@"FacebookToken": token}];
    //Crash here if token is nil.
    
    if (deviceToken != nil) {
        [parameters setObject:deviceToken forKey:@"DeviceToken"];
    }
    
    NSString *requestUrl = [NSString stringWithFormat:@"%@/Auth/User", API_BASE_URL];
    
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
        if ([operation.responseObject isKindOfClass:[NSDictionary class]]) {
            NSDictionary *errorDict = [NSJSONSerialization JSONObjectWithData:(NSData *)error.userInfo[AFNetworkingOperationFailingURLResponseDataErrorKey] options:0 error:nil];
            
            NSString *ErrorResponse = [NSString stringWithFormat:@"Error %ld: %@", operation.response.statusCode, [errorDict objectForKey:@"Message"]];
            
            [self.HUD hide:YES];
            NSLog(@"%@",ErrorResponse);
            
            UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:@"Not connected to an InstaMelody account, please create a new one" delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
            [alertView show];
            
            [self signUpWithToken:token];
        }
    }];
}

/*
 
 //facebook delegate methods
 
 func loginButton(loginButton: FBSDKLoginButton!, didCompleteWithResult result: FBSDKLoginManagerLoginResult!, error: NSError!) {
 
 Flurry.logEvent("Login Facebook");
 
 if result != nil {
 if (!result!.isCancelled) {
 let fbToken = result!.token
 self.facebookSuccess(fbToken.tokenString)
 } else {
 let alert = UIAlertController(title: defaultAlertTitle, message: "Error linking Facebook", preferredStyle: UIAlertControllerStyle.Alert)
 alert.addAction(UIAlertAction(title: okButtonText, style: UIAlertActionStyle.Default, handler: nil))
 self.presentViewController(alert, animated: true, completion: nil)
 }
 }
 }
 
 */

-(IBAction)signUp:(id)sender {
    UIStoryboard *mainStoryboard = [UIStoryboard storyboardWithName:@"Main" bundle:[NSBundle mainBundle]];
    SignUpViewController *signupVC = [mainStoryboard instantiateViewControllerWithIdentifier:@"SignUpViewController"];
    [self.navigationController pushViewController:signupVC animated:YES];
}


-(void)signUpWithToken:(NSString *)token {
    UIStoryboard *mainStoryboard = [UIStoryboard storyboardWithName:@"Main" bundle:[NSBundle mainBundle]];
    SignUpViewController *signupVC = [mainStoryboard instantiateViewControllerWithIdentifier:@"SignUpViewController"];
    signupVC.fbToken = token;
    [self.navigationController pushViewController:signupVC animated:YES];
}

-(IBAction)signIn:(id)sender {
 
    NSString *deviceToken =  [[NSUserDefaults standardUserDefaults] objectForKey:@"deviceToken"];
 
    if (![self.userField.text isEqualToString:@""] && ![self.passField.text isEqualToString:@""] ) {
        
        self.HUD.indeterminate = YES;
        self.HUD.status = @"Logging in";
        [self.HUD show:YES];
        
        NSMutableDictionary *parameters = [NSMutableDictionary dictionaryWithDictionary:@{@"DisplayName": self.userField.text, @"Password": self.passField.text}];
        
        if (deviceToken != nil) {
            [parameters setObject:deviceToken forKey:@"DeviceToken"];
        }
        
        NSString *requestUrl = [NSString stringWithFormat:@"%@/Auth/User", API_BASE_URL];
        
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
            if ([operation.responseObject isKindOfClass:[NSDictionary class]]) {
                NSDictionary *errorDict = [NSJSONSerialization JSONObjectWithData:(NSData *)error.userInfo[AFNetworkingOperationFailingURLResponseDataErrorKey] options:0 error:nil];
                
                NSString *ErrorResponse = [NSString stringWithFormat:@"Error %ld: %@", operation.response.statusCode, [errorDict objectForKey:@"Message"]];
                
                [self.HUD hide:YES];
                NSLog(@"%@",ErrorResponse);
                
                UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:ErrorResponse delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
                [alertView show];
            }
        }];
    } else {
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:@"Please fill in all fields" delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
        [alertView show];
    }
    
}

-(IBAction)cancel:(id)sender {
    [self.HUD hide:YES];
    [self dismissViewControllerAnimated:YES completion:nil];
}

#pragma mark - SIGN UP DELEGATE
-(void)finishedWithUserId:(NSString *)userId andPassword:(NSString *)password {
    self.userField.text = userId;
    self.passField.text = password;
    
    //[self signIn:nil];
    [self getUserDetails:self.userField.text];
    
}

-(void)getUserDetails:(NSString*)displayName {
    
    //https://api.instamelody.com/v1.0/User?token=9d0ab021-fcf8-4ec3-b6e3-bb1d0d03b12e&displayName=testeraccount
    
    NSString *requestUrl = [NSString stringWithFormat:@"%@/User", API_BASE_URL];
    
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

        if ([responseDict objectForKey:@"Image"] != nil && [[responseDict objectForKey:@"Image"] isKindOfClass:[NSDictionary class]]) {
            NSDictionary *imageDict = [responseDict objectForKey:@"Image"];
            [[NSUserDefaults standardUserDefaults] setObject:[imageDict objectForKey:@"FilePath"] forKey:@"ProfileFilePath"];
        }
            
        [[NSNotificationCenter defaultCenter] postNotificationName:@"infoUpdated" object:nil];
        [[NSUserDefaults standardUserDefaults] synchronize];
        
        [self downloadProfilePhoto];
        
        [self cancel:nil];
        
        //NSDictionary *responseDict = (NSDictionary *)responseObject;
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        if ([operation.responseObject isKindOfClass:[NSDictionary class]]) {
            
            [self.HUD hide:YES];
            
            NSDictionary *errorDict = [NSJSONSerialization JSONObjectWithData:(NSData *)error.userInfo[AFNetworkingOperationFailingURLResponseDataErrorKey] options:0 error:nil];
            
            NSString *ErrorResponse = [NSString stringWithFormat:@"Error %ld: %@", operation.response.statusCode, [errorDict objectForKey:@"Message"]];
            
            NSLog(@"%@",ErrorResponse);
            
            UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:ErrorResponse delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
            [alertView show];
        }
    }];
}

-(void)downloadProfilePhoto {
    
    NSFileManager *fileManager = [NSFileManager defaultManager];
    
    NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
    
    NSString *profilePath = [documentsPath stringByAppendingPathComponent:@"Profiles"];
    
    NSString *remotePath =  [[NSUserDefaults standardUserDefaults] objectForKey:@"ProfileFilePath"];
    
    NSString *fileName = [remotePath lastPathComponent];
    NSString *pathString = [profilePath stringByAppendingPathComponent:fileName];
    
    if (![fileManager fileExistsAtPath:profilePath]) {
        NSError* error;
        if(  [[NSFileManager defaultManager] createDirectoryAtPath:profilePath withIntermediateDirectories:NO attributes:nil error:&error]) {
            
            NSLog(@"success creating folder");
            
        } else {
            NSLog(@"[%@] ERROR: attempting to write create MyFolder directory", [self class]);
            NSAssert( FALSE, @"Failed to create directory maybe out of disk space?");
        }
    }
    
    if (![fileManager fileExistsAtPath:pathString]) {
        
        NSURLSessionConfiguration *configuration = [NSURLSessionConfiguration defaultSessionConfiguration];
        AFURLSessionManager *manager = [[AFURLSessionManager alloc] initWithSessionConfiguration:configuration];
        
        NSString *fullUrlString = [NSString stringWithFormat:@"%@/%@", DOWNLOAD_BASE_URL, remotePath];
        fullUrlString = [fullUrlString stringByAddingPercentEncodingWithAllowedCharacters:[NSCharacterSet URLQueryAllowedCharacterSet]];
        
        NSURL *URL = [NSURL URLWithString:fullUrlString];
        NSURLRequest *request = [NSURLRequest requestWithURL:URL];
        
        NSURLSessionDownloadTask *downloadTask = [manager downloadTaskWithRequest:request progress:nil destination:^NSURL *(NSURL *targetPath, NSURLResponse *response) {
            
            //NSString *fileString = [NSString stringWithFormat:@"file://%@", destinationFilePath];
            NSURL *fileURL = [NSURL fileURLWithPath:pathString];
            return fileURL;
            
            //NSURL *documentsDirectoryURL = [[NSFileManager defaultManager] URLForDirectory:NSDocumentDirectory inDomain:NSUserDomainMask appropriateForURL:nil create:NO error:nil];
            //return [documentsDirectoryURL URLByAppendingPathComponent:[response suggestedFilename]];
        } completionHandler:^(NSURLResponse *response, NSURL *filePath, NSError *error) {
            
            if (error == nil) {
                NSLog(@"File downloaded to: %@", filePath);
                NSError *error = nil;
                BOOL success = [filePath setResourceValue:[NSNumber numberWithBool:YES] forKey:NSURLIsExcludedFromBackupKey error:&error];
                if(!success){
                    NSLog(@"Error excluding %@ from backup %@", [filePath lastPathComponent], error);
                }
                [[NSNotificationCenter defaultCenter] postNotificationName:@"downloadedProfile" object:nil];
                
            } else {
                NSLog(@"Download error: %@", error.description);
            }
        }];
        [downloadTask resume];
        
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
    
    CGPoint scrollPoint = CGPointMake(0, parentView.frame.origin.y - VERTICAL_SHIFT);
    [self.scrollView setContentOffset:scrollPoint animated:YES];
}

- (void)textFieldDidEndEditing:(UITextField *)textField {
    [self.scrollView setContentOffset:CGPointZero animated:YES];
}

@end
