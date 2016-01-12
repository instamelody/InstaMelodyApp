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
#import <FBSDKCoreKit.h>

#import "UIFont+FontAwesome.h"
#import "NSString+FontAwesome.h"
#import "FAImageView.h"
#import "NetworkManager.h"

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
    
    self.fbButton.readPermissions = @[@"public_profile", @"email"]; // , @"user_friends"];
    self.fbButton.delegate = self;
    self.fbButton.layer.cornerRadius = 4.0f;
    self.fbButton.layer.masksToBounds = YES;
    
    FBSDKLoginManager *loginManager = [[FBSDKLoginManager alloc] init];
    
    [loginManager logOut];
    
    // setup twitter login button and callback
    /*
    self.twitterButton = [TWTRLogInButton buttonWithLogInCompletion:^(TWTRSession* session, NSError* error) {
        if (session) {
            NSLog(@"signed in as %@", [session userName]);
            [self signUp:nil];
        } else {
            NSLog(@"error: %@", [error localizedDescription]);
        }
    }];*/
    

    //Note: once the user has logged in once, a logout will end their session but the next login will just
    //log them straight back in.
    
    [self.twitterButton setLogInCompletion:^(TWTRSession *session, NSError *error) {
        
        [self doTwitterLogin:session withError:error];
        
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
        
        //NSString *fbToken = result.token.tokenString;
        NSString *userId = result.token.userID;
        
        [self doFBLoginWithUserID:userId result:(FBSDKLoginManagerLoginResult *) result];
    } else {
        NSLog(@"Facebook error: %@", error.description);
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:@"Error connecting to Facebook" delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
        [alertView show];
    }
}

-(void)loginButtonDidLogOut:(FBSDKLoginButton *)loginButton {
    
}

-(void)doFBLoginWithUserID:(NSString *)userId result:(FBSDKLoginManagerLoginResult *) result {
    
    //[self signUp:nil];
    
    NSString *deviceToken =  [[NSUserDefaults standardUserDefaults] objectForKey:@"deviceToken"];
    //This is the push notification device token, if there is one. If not, it is nil.
    
    if (!userId)
    {
        //User cancelled out of Facebook login process
        return;
    }
    
    NSMutableDictionary *parameters = [NSMutableDictionary dictionaryWithDictionary:@{@"FacebookUserId": userId}];
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
            
            NSString *ErrorResponse = [NSString stringWithFormat:@"Error %td: %@", operation.response.statusCode, [errorDict objectForKey:@"Message"]];
            /*
            [self.HUD hide:YES];
            NSLog(@"%@",ErrorResponse);
            
            UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:@"Not connected to an InstaMelody account, please create a new one" delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
            [alertView show];
            
            [self signUpWithToken:token];
             */

            //Going to get user info now for account creation.
            FBSDKGraphRequest * thisRequest = [[FBSDKGraphRequest alloc] initWithGraphPath:@"me" parameters:@{@"fields" : @"id,email,first_name,last_name,picture.width(100).height(100)"}];
            
            [thisRequest startWithCompletionHandler:^(FBSDKGraphRequestConnection *connection, id result, NSError *error) {
                
                UIAlertController * alert = [self promptUserForUsernameWithID:userId result:result];
                    
                [self presentViewController:alert animated:YES completion:nil];
                
            }];
            
        }
    }];
}

-(UIAlertController *)promptUserForUsernameWithID:(NSString *)userId result:(NSDictionary *)result
{
    
    UIAlertController *alert = [UIAlertController alertControllerWithTitle:@"Please enter a username" message:@"Enter a username for your InstaMelody account" preferredStyle:UIAlertControllerStyleAlert];
    
    [alert addTextFieldWithConfigurationHandler:^(UITextField *textField) {
        textField.placeholder = @"Username";
        
    }];
    
    UIAlertAction *cancelAction = [UIAlertAction actionWithTitle:@"Cancel" style:UIAlertActionStyleDestructive handler:nil];
    
    UIAlertAction *submitAction = [UIAlertAction actionWithTitle:@"Submit" style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
        
        UITextField *textField = alert.textFields[0];
        NSString * userName = textField.text;
        NSString * password = @"doesnt matter";
        NSString * firstName = [self handleBlank:[result objectForKey:@"first_name"]];
        NSString * lastName = [self handleBlank:[result objectForKey:@"last_name"]];
        NSString * email = [self handleBlank:[result objectForKey:@"email"]];
        
        UIImage * profImage;
        if ([result objectForKey:@"picture"]) {
            NSURL * profilePicURL = [NSURL URLWithString:[[[result objectForKey:@"picture"] objectForKey:@"data"] objectForKey:@"url"]];
            NSData *pullFBPP = [[NSData alloc]initWithContentsOfURL:profilePicURL];
            
            profImage = [UIImage imageWithData:pullFBPP];
        }
        
        [self doSignUpForUser:userName
                 withPassword:password
                    withFirst:firstName
                     withLast:lastName
                    withEmail:email
                     withFBID:userId
               withProfilePic:profImage];
        
    }];

    [alert addAction:cancelAction];
    [alert addAction:submitAction];

    return alert;

}

#pragma mark - Twitter sdk handling

-(void)doTwitterLogin:(TWTRSession *)session withError:(NSError *)error
{
    //If the user clicks Twitter login button...
    if (session) {
        NSLog(@"signed in as %@", [session userName]);
        //First, check to see if this user has an account already
        __weak typeof(self) weakSelf = self;
        
        weakSelf.userField.text = session.userName;
        weakSelf.passField.text = [NSString stringWithFormat:@"%@xRT67q1a",session.userID];
        //Making up a password based on the Twitter user's ID
        //Not CIA level security, but good enough
        
        [self signIn:nil withFailureBlock:^(void){
        
            //They don't already have an account, so need to get info to create it
            //Now ask for email address
            TWTRShareEmailViewController* shareEmailViewController = [[TWTRShareEmailViewController alloc] initWithCompletion:^(NSString* email, NSError* error) {
                
                NSLog(@"Email %@, Error: %@", email, error);
                
                //Finally get their first/last name and profile pic
                [[[Twitter sharedInstance] APIClient] loadUserWithID:[session userID]
                                                          completion:^(TWTRUser *user,
                                                                       NSError *error)
                 {
                     // handle the response or error
                     if (![error isEqual:nil]) {
                         NSLog(@"Twitter info   -> user = %@ ",user);
                         NSString *urlString = [[NSString alloc]initWithString:user.profileImageLargeURL];
                         NSURL *url = [[NSURL alloc]initWithString:urlString];
                         NSData *pullTwitterPP = [[NSData alloc]initWithContentsOfURL:url];
                         
                         UIImage *profImage = [UIImage imageWithData:pullTwitterPP];
                         
                         //Split whole name into first and last
                         NSArray *nameArray = [user.name componentsSeparatedByCharactersInSet:[NSCharacterSet whitespaceCharacterSet]];
                         NSString * lastName = [nameArray lastObject];
                         NSString * firstName = [user.name stringByReplacingOccurrencesOfString:lastName withString:@""];
                         
                         [self doSignUpForUser:user.screenName
                                  withPassword:weakSelf.passField.text
                                     withFirst:firstName
                                      withLast:lastName
                                     withEmail:email
                                      withFBID:nil
                                withProfilePic:profImage];
                         
                     } else {
                         NSLog(@"Twitter error getting profile : %@", [error localizedDescription]);
                     }
                 }];

            }];
            [self presentViewController:shareEmailViewController animated:YES completion:nil];
        
        }];
        
        
    } else {
        NSLog(@"error: %@", [error localizedDescription]);
    }
}


#pragma mark - new login methods

-(IBAction)signIn:(id)sender {
 
    [self signIn:sender withFailureBlock:nil];
    
}

-(void)signIn:(id)sender withFailureBlock:(void (^) (void))failureBlock {
 
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
                
                if (failureBlock)
                {
                    failureBlock();
                } else {

                    NSDictionary *errorDict = [NSJSONSerialization JSONObjectWithData:(NSData *)error.userInfo[AFNetworkingOperationFailingURLResponseDataErrorKey] options:0 error:nil];
                    
                    NSString *ErrorResponse = [NSString stringWithFormat:@"Error %ld: %@", operation.response.statusCode, [errorDict objectForKey:@"Message"]];
                    
                    [self.HUD hide:YES];
                    NSLog(@"%@",ErrorResponse);
                    
                    UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:ErrorResponse delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
                    [alertView show];
                }
            }
        }];
    } else {
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:@"Please fill in all fields" delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
        [alertView show];
    }
    
}

-(NSString *)handleBlank:(NSString *)input
{
    if (!input)
    {
        return @"X";
    }
    
    if (input.length == 0)
    {
        return @"X";
    }
    
    return input;
    
}

-(void)doSignUpForUser:(NSString *)userName withPassword:(NSString *)password
             withFirst:(NSString *)firstName withLast:(NSString *)lastName
             withEmail:(NSString *)email withFBID:(NSString *)FBID
        withProfilePic:(UIImage *)profileImage
{
    //Do something to trap/handle blank or null params
    
    if (!userName || userName.length == 0)
    {
        NSLog(@"nil username");
        return;
    }

    if (!password || password.length == 0)
    {
        NSLog(@"nil password");
        return;
    }
    
    firstName = [self handleBlank:firstName];
    lastName = [self handleBlank:lastName];
    email = [self handleBlank:email];
    if ([email isEqualToString:@"X"])
        email = [NSString stringWithFormat:@"%@@unknown.com", userName];
    
    NSNumber *isFemale = [NSNumber numberWithBool:NO];
    
    NSString *monthYear = @"01/2000";
    
    NSString *encodedEmail = [email stringByAddingPercentEncodingWithAllowedCharacters:NSCharacterSet.URLQueryAllowedCharacterSet];
    NSMutableDictionary *parameters = [NSMutableDictionary dictionaryWithDictionary:
                                       @{@"DisplayName": userName,
                                         @"Password": password,
                                         @"FirstName": firstName,
                                         @"LastName": lastName,
                                         @"PhoneNumber" : @"",
                                         @"EmailAddress" : encodedEmail,
                                         @"IsFemale": isFemale,
                                         @"DateOfBirth": monthYear}];
    
    if (FBID) {
        [parameters setObject:FBID forKey:@"FacebookUserId"];
    }
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    
    //self.HUD.indeterminate = YES;
    //self.HUD.status = @"Signing up";
    //[self.HUD show:YES];
    
    NSString *requestUrl = [NSString stringWithFormat:@"%@/User/New", API_BASE_URL];
    
    [manager POST:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        NSLog(@"JSON: %@", responseObject);
        
        //NSDictionary *responseDict = (NSDictionary *)responseObject;
        
        NSMutableDictionary *parameters = [NSMutableDictionary dictionaryWithDictionary:@{@"DisplayName": userName, @"Password": password}];
        
        NSString *requestUrl = [NSString stringWithFormat:@"%@/Auth/User", API_BASE_URL];
        
        //add 64 char string
        
        AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
        
        [manager POST:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
            NSLog(@"JSON: %@", responseObject);
            
            dispatch_async(dispatch_get_main_queue(), ^{
                UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Success" message:@"You are now logged in" delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
                [alertView show];
                
            });
            
            NSDictionary *responseDict =
            (NSDictionary *)responseObject;
            [[NSUserDefaults standardUserDefaults] setObject:[responseDict objectForKey:@"Token"] forKey:@"authToken"];
            
            [[NSUserDefaults standardUserDefaults] synchronize];
            
            if (profileImage != nil) {
                [[NetworkManager sharedManager] prepareImage:profileImage];
            }
            
            [self.HUD hide:YES];
            
            self.userField.text = userName;
            self.passField.text = password;
            
            [self getUserDetails:userName];
            
        } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
            if ([operation.responseObject isKindOfClass:[NSDictionary class]]) {
                NSDictionary *errorDict = [NSJSONSerialization JSONObjectWithData:(NSData *)error.userInfo[AFNetworkingOperationFailingURLResponseDataErrorKey] options:0 error:nil];
                
                NSString *ErrorResponse = [NSString stringWithFormat:@"Error %td: %@", operation.response.statusCode, [errorDict objectForKey:@"Message"]];
                
                [self.HUD hide:YES];
                NSLog(@"%@",ErrorResponse);
                
                dispatch_async(dispatch_get_main_queue(), ^{
                    UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:ErrorResponse delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
                    [alertView show];
                });
            }
        }];
        
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        
        [self.HUD hide:YES];
        
        if ([operation.responseObject isKindOfClass:[NSDictionary class]]) {
            NSDictionary *errorDict = [NSJSONSerialization JSONObjectWithData:(NSData *)error.userInfo[AFNetworkingOperationFailingURLResponseDataErrorKey] options:0 error:nil];
            
            NSString *ErrorResponse = [NSString stringWithFormat:@"Error %td: %@", operation.response.statusCode, [errorDict objectForKey:@"Message"]];
            
            NSLog(@"%@",ErrorResponse);
            
            dispatch_async(dispatch_get_main_queue(), ^{
                UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:ErrorResponse delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
                [alertView show];
            });
            
        }
    }];
   
}

#pragma mark - old login methods

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
