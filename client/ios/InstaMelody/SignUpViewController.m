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
#import "NetworkManager.h"

@interface SignUpViewController ()
    @property M13ProgressHUD *HUD;
    @property LTHMonthYearPickerView *monthYearPicker;
    @property UIImage *savedImage;

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
    
    self.HUD = [[M13ProgressHUD alloc] initWithProgressView:[[M13ProgressViewRing alloc] init]];
    self.HUD.progressViewSize = CGSizeMake(60.0, 60.0);
    self.HUD.animationPoint = CGPointMake([UIScreen mainScreen].bounds.size.width / 2, [UIScreen mainScreen].bounds.size.height / 2);
    UIWindow *window = [[[UIApplication sharedApplication] windows] objectAtIndex:0];
    [window addSubview:self.HUD];
    
    self.genderField.layer.cornerRadius = 4.0f;
    self.genderField.layer.masksToBounds = YES;
    
    UIFont *font = [UIFont fontWithName:@"Century Gothic" size:16.0f];
    
    NSDictionary *attributes = [NSDictionary dictionaryWithObject:font forKey:NSFontAttributeName];
    
    [self.genderField setTitleTextAttributes:attributes forState:UIControlStateNormal];
    [self.genderField setTitleTextAttributes:attributes forState:UIControlStateSelected];
    
    NSDateFormatter *dateFormatter = [NSDateFormatter new];
    [dateFormatter setDateFormat:@"MM / yyyy"];
    NSDate *minDate = [dateFormatter dateFromString:[NSString stringWithFormat: @"%i / %i", 1, 1915]];
    
    self.monthYearPicker =  [[LTHMonthYearPickerView alloc]
                        initWithDate: [NSDate date]
                        shortMonths: NO
                        numberedMonths: YES
                        andToolbar: YES
                        minDate: minDate
                        andMaxDate:[NSDate date]];
    
    self.monthYearPicker.delegate = self;
    
    self.dobField.delegate = self;
    self.dobField.inputView = self.monthYearPicker;
    
    self.profileView.layer.cornerRadius = self.profileView.frame.size.height / 2;
    self.profileView.layer.masksToBounds = YES;
    
    if (self.userInfo != nil) {
        [self loadData];
        [self.submitButton setTitle:@"Update" forState:UIControlStateNormal];
    }
    
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

-(void)loadData {
    if ([self.userInfo objectForKey:@"FirstName"] != nil) {
        self.firstNameField.text = [self.userInfo objectForKey:@"FirstName"];
    }
    
    if ([self.userInfo objectForKey:@"LastName"] != nil) {
        self.lastNameField.text = [self.userInfo objectForKey:@"LastName"];
    }
}

- (IBAction)submit:(id)sender {
    
    if (self.userInfo == nil) {
        [self createNewUser];
    } else {
        [self updateProfile];
    }
    
}

-(void)updateProfile {
    if (self.savedImage != nil) {
        [self prepareImage:self.savedImage];
    }
    
    [self.navigationController popViewControllerAnimated:YES];
}

-(void)createNewUser {
    
    if (![self.usernameField.text isEqualToString:@""] && ![self.passwordField.text isEqualToString:@""] && ![self.firstNameField.text isEqualToString:@""] && ![self.lastNameField.text isEqualToString:@""] && ![self.phoneNumberField.text isEqualToString:@""]) {
        
        NSNumber *isFemale = self.genderField.enabled ? [NSNumber numberWithBool:NO] : [NSNumber numberWithBool:YES];
        
        NSString *monthYear = self.dobField.text;
        
        NSString *encodedEmail = [self.emailAddressField.text stringByAddingPercentEncodingWithAllowedCharacters:NSCharacterSet.URLQueryAllowedCharacterSet];
        NSDictionary *parameters = @{@"DisplayName": self.usernameField.text, @"Password": self.passwordField.text, @"FirstName": self.firstNameField.text, @"LastName": self.lastNameField.text, @"PhoneNumber" : self.phoneNumberField.text, @"EmailAddress" : encodedEmail, @"IsFemale": isFemale, @"DateOfBirth": monthYear};
        
        AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
        //[manager.requestSerializer setValue:@"application/x-www-form-urlencoded" forHTTPHeaderField:@"Content-Type"];
        //manager.requestSerializer = [AFHTTPRequestSerializer serializer];
        /*
         [[AFHTTPRequestSerializer serializer] requestWithMethod:@"POST" URLString:URLString parameters:parameters];
         */
        
        self.HUD.indeterminate = YES;
        self.HUD.status = @"Signing up";
        [self.HUD show:YES];
        
        
        NSString *requestUrl = [NSString stringWithFormat:@"%@/User/New", API_BASE_URL];
        
        [manager POST:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
            NSLog(@"JSON: %@", responseObject);
            
            
            if (self.savedImage != nil) {
                [self prepareImage:self.savedImage];
            }
            
            [self.HUD hide:YES];
            
            [self dismissViewControllerAnimated:YES completion:^{
                
                [self.delegate finishedWithUserId:self.usernameField.text andPassword:self.passwordField.text];
                
            }];

            
        } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
            
            [self.HUD hide:YES];
            
            if ([operation.responseObject isKindOfClass:[NSDictionary class]]) {
                NSDictionary *errorDict = [NSJSONSerialization JSONObjectWithData:(NSData *)error.userInfo[AFNetworkingOperationFailingURLResponseDataErrorKey] options:0 error:nil];
                
                NSString *ErrorResponse = [NSString stringWithFormat:@"Error %ld: %@", operation.response.statusCode, [errorDict objectForKey:@"Message"]];
                
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

/*
- (void)textFieldDidBeginEditing:(UITextField *)textField {
    CGPoint scrollPoint = CGPointMake(0, textField.frame.origin.y);
    [self.scrollView setContentOffset:scrollPoint animated:YES];
}

- (void)textFieldDidEndEditing:(UITextField *)textField {
    [self.scrollView setContentOffset:CGPointZero animated:YES];
}*/

-(IBAction)cancel:(id)sender {
    [self.HUD hide:YES];
    
    
    
    [self dismissViewControllerAnimated:YES completion:^{
        if (self.presentingViewController != nil) {
            [self.presentingViewController dismissViewControllerAnimated:NO completion:nil];
        }
    }];
}

-(IBAction)changeProfile:(id)sender {
    UIImagePickerController *imagePicker = [[UIImagePickerController alloc] init];
    imagePicker.sourceType = UIImagePickerControllerSourceTypeSavedPhotosAlbum;
    imagePicker.delegate = self;
    
    [self presentViewController:imagePicker animated:YES completion:^{
        NSLog(@"Image picker presented!");
    }];
}

#pragma mark - LTHMonthYearPickerView Delegate
- (void)pickerDidPressCancelWithInitialValues:(NSDictionary *)initialValues {
    self.dobField.text = [NSString stringWithFormat:
                           @"%@/%@",
                           initialValues[@"month"],
                           initialValues[@"year"]];
    [self.dobField resignFirstResponder];
}


- (void)pickerDidPressDoneWithMonth:(NSString *)month andYear:(NSString *)year {
    self.dobField.text = [NSString stringWithFormat: @"%@/%@", month, year];
    [self.dobField resignFirstResponder];
}


- (void)pickerDidPressCancel {
    [self.dobField resignFirstResponder];
}


- (void)pickerDidSelectRow:(NSInteger)row inComponent:(NSInteger)component {
    NSLog(@"row: %zd in component: %zd", row, component);
}


- (void)pickerDidSelectMonth:(NSString *)month {
    NSLog(@"month: %@ ", month);
}


- (void)pickerDidSelectYear:(NSString *)year {
    NSLog(@"year: %@ ", year);
}


- (void)pickerDidSelectMonth:(NSString *)month andYear:(NSString *)year {
    self.dobField.text = [NSString stringWithFormat: @"%@/%@", month, year];
}

#pragma mark - image picker delegate


-(void)imagePickerController:(UIImagePickerController *)picker
didFinishPickingMediaWithInfo:(NSDictionary *)info
{
    UIImage *selectedImage = [info objectForKey:UIImagePickerControllerOriginalImage];
    [self.profileButton setBackgroundImage:selectedImage forState:UIControlStateNormal];
    [picker dismissViewControllerAnimated:YES completion:^{
        NSLog(@"Image selected!");
    }];
    
    self.savedImage = selectedImage;
}

-(void)imagePickerControllerDidCancel:(UIImagePickerController *)picker
{
    [picker dismissViewControllerAnimated:YES completion:^{
        NSLog(@"Picker cancelled without doing anything");
    }];
}


-(void)prepareImage:(UIImage *)image {
    
    UIImage *resizedImage = nil;
    CGSize originalImageSize = image.size;
    CGSize targetImageSize = CGSizeMake(150.0f, 150.0f);
    float scaleFactor, tempImageHeight, tempImageWidth;
    CGRect croppingRect;
    BOOL favorsX = NO;
    if (originalImageSize.width > originalImageSize.height) {
        scaleFactor = targetImageSize.height / originalImageSize.height;
        favorsX = YES;
    } else {
        scaleFactor = targetImageSize.width / originalImageSize.width;
        favorsX = NO;
    }
    
    tempImageHeight = originalImageSize.height * scaleFactor;
    tempImageWidth = originalImageSize.width * scaleFactor;
    if (favorsX) {
        float delta = (tempImageWidth - targetImageSize.width) / 2;
        croppingRect = CGRectMake(-1.0f * delta, 0, tempImageWidth, tempImageHeight);
    } else {
        float delta = (tempImageHeight - targetImageSize.height) / 2;
        croppingRect = CGRectMake(0, -1.0f * delta, tempImageWidth, tempImageHeight);
    }
    UIGraphicsBeginImageContext(targetImageSize);
    [image drawInRect:croppingRect];
    resizedImage = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    
    [[NetworkManager sharedManager] updateProfilePicture:resizedImage];
}


@end
